using System;
using System.Linq;
using Resat.Cameras;
using Resat.Models;
using UnityEngine;

namespace Resat.Colors
{
    // Wrapper for compute shader
    public class OKHSLController : MonoBehaviour
    {
        public Vector2Int OkhslArraySize => _okhslArraySize;
        public RenderTexture? OutputArrayTexture => _outputArrayTexture;
        
        [Header("Dependencies")]
        [SerializeField]
        private ComputeShader _computeShader = null!;

        [Header("Settings")]
        [SerializeField]
        private Vector2Int _okhslArraySize = new(64, 32);

        [SerializeField]
        private uint _topColorsCount = 8;

        [SerializeField]
        public bool PostProcessing = true;
        
        [SerializeField]
        public OKHSLArrayType PreviewImageArrayType;
        
        // TODO: Make texture support better, and support multiple types of texture being rendered to at the same time
        private RenderTexture? _outputArrayTexture;
        
        private ComputeBuffer? _okhslArrayBuffer;
        private ComputeBuffer? _globalOkhslArrayBuffer;
        private ComputeBuffer? _okhslPostProcessBuffer;

        private int _calculateArrayIndex;
        private int _postProcessIndex;
        private int _getPreviewTextureIndex;
        private int _getGlobalTextureIndex;
        private int _getMergedTextureIndex;
        private int _mergeArrayIndex;
        
        // arrays reused to avoid allocating every capture
        private int[]? _outputArray;
        private Color[]? _postProcessArray;
        private int[]? _emptyIntArray;
        private Color[]? _emptyColorArray;

        // when a picture is taken, the data is serialized here
        private int[]? _globalOkhslArray;

        private void OnEnable()
        {
            // int is 4 bytes and color is four 4-byte floats (4*4)
            _okhslArrayBuffer = new ComputeBuffer(_okhslArraySize.x * _okhslArraySize.y, 4);
            _globalOkhslArrayBuffer = new ComputeBuffer(_okhslArraySize.x * _okhslArraySize.y, 4);
            _okhslPostProcessBuffer = new ComputeBuffer(GetPostProcessDataLength(), 4 * 4);

            _outputArrayTexture = new RenderTexture(_okhslArraySize.x, _okhslArraySize.y, 0, RenderTextureFormat.R8, RenderTextureReadWrite.Linear);
            _outputArrayTexture.enableRandomWrite = true;
            _outputArrayTexture.filterMode = FilterMode.Point;
            _outputArrayTexture.Create();
            
            _outputArray = new int[_okhslArraySize.x * _okhslArraySize.y];
            _postProcessArray = new Color[GetPostProcessDataLength()];
            ResetGlobalOkhslArray();
            
            // set indexes used for setting variables
            _calculateArrayIndex = _computeShader.FindKernel("CSCalculateArray");
            _postProcessIndex = _computeShader.FindKernel("CSPostProcess");
            _getPreviewTextureIndex = _computeShader.FindKernel("CSGetPreviewTexture");
            _getGlobalTextureIndex = _computeShader.FindKernel("CSGetGlobalTexture");
            _getMergedTextureIndex = _computeShader.FindKernel("CSGetMergedTexture");
            _mergeArrayIndex = _computeShader.FindKernel("CSMergeArrays");
            
            // set empty arrays
            _emptyIntArray ??= new int[_okhslArraySize.x * _okhslArraySize.y];
            _emptyColorArray ??= new Color[GetPostProcessDataLength()];
        }
        
        private void OnDisable()
        {
            ReleaseResource(ref _okhslArrayBuffer);
            ReleaseResource(ref _okhslPostProcessBuffer);
            ReleaseResource(ref _outputArrayTexture);
            
            _outputArray = null;
            _postProcessArray = null;
            _globalOkhslArray = null;
        }

        private void ResetGlobalOkhslArray()
        {
            if (_globalOkhslArray == null)
            {
                _globalOkhslArray ??= new int[_okhslArraySize.x * _okhslArraySize.y];
                return;
            }

            Array.Clear(_globalOkhslArray, 0, _globalOkhslArray.Length);
        }
        
        private void AddToGlobalOkhslArray(int[]? intArray = null)
        {
            if (_globalOkhslArray == null || intArray == null) 
                return;

            if (_globalOkhslArray.Length != intArray.Length)
            {
                Debug.LogWarning("Attempting to add array of different size to the global OKHSL array, returning...");
            }

            Debug.Log("Adding to global array");
            
            for (int i = 0; i < _globalOkhslArray.Length; i++)
            {
                _globalOkhslArray[i] += intArray[i];
            }
        }
        
        // used after a screenshot to add things to the Big Array
        public void SerializeLastRender()
        {
            _computeShader.SetBuffer(_mergeArrayIndex, "_OKHSLArray", _okhslArrayBuffer);
            _computeShader.SetBuffer(_mergeArrayIndex, "_GlobalOKHSLArray", _globalOkhslArrayBuffer);
            _computeShader.Dispatch(_mergeArrayIndex, _okhslArraySize.x / 8, _okhslArraySize.y / 8, 1);
        }
        
        // public void RunComputeShader(CameraResolutionData cameraResolutionData, ResatCamera resatCamera)
        public OKHSLData? RunComputeShader(RenderTexture renderTexture)
        {
            if (_okhslArrayBuffer == null || _outputArrayTexture == null)
                return null;

            if (renderTexture.width % 8 != 0 || renderTexture.height % 8 != 0)
            {
                Debug.LogWarning("Passed RenderTexture was not a power of eight! Image will be truncated!");
                // TODO: See if non-power of eights actually break anything, or if we can just continue fine
            }
            
            // Reset arrays to avoid data leaking from old textures
            _okhslArrayBuffer.SetData(_emptyIntArray);
            // RenderTexture.active = _outputArrayTexture;
            // GL.Clear(false, true, Color.black);
            
            // Setup compute shader
            _computeShader.SetTexture(_calculateArrayIndex, "_InputTexture", renderTexture);
            _computeShader.SetBuffer(_calculateArrayIndex, "_OKHSLArray", _okhslArrayBuffer);
            _computeShader.SetInts("_OKHSLArrayResolution", _okhslArraySize.x, _okhslArraySize.y);

            // Dispatch initial patch to get OKHSL array info
            _computeShader.Dispatch(_calculateArrayIndex, renderTexture.width / 8, renderTexture.height / 8, 1);
            _okhslArrayBuffer.GetData(_outputArray);

            // Get preview texture now that array is filled
            GetArrayTexture(_outputArrayTexture, PreviewImageArrayType);

            return PostProcessing ? PostProcess() : null;
        }

        private void GetArrayTexture(RenderTexture renderTexture, OKHSLArrayType arrayType)
        {
            int index = arrayType switch
            {
                OKHSLArrayType.Merged => _getMergedTextureIndex,
                OKHSLArrayType.Preview => _getPreviewTextureIndex,
                OKHSLArrayType.Global => _getGlobalTextureIndex,
                _ => _getMergedTextureIndex
            };

            if (arrayType != OKHSLArrayType.Global)
                _computeShader.SetBuffer(index, "_OKHSLArray", _okhslArrayBuffer);
            if (arrayType != OKHSLArrayType.Preview)
                _computeShader.SetBuffer(index, "_GlobalOKHSLArray", _globalOkhslArrayBuffer);
            _computeShader.SetTexture(index, "_OutputArrayTexture", renderTexture);
            
            _computeShader.Dispatch(index, _okhslArraySize.x / 8, _okhslArraySize.y / 8, 1);
        }
        
        // TODO: Combine this all into one pass. it's probably possible (maybe just minus the sorting?)
        // could just remove the sorting, or add a seperate dedicated compute shader/shader for that
        private OKHSLData? PostProcess()
        {
            if (_okhslPostProcessBuffer == null || _postProcessArray == null) 
                return null;

            // Reset arrays to avoid data leaking from old textures
            _okhslPostProcessBuffer.SetData(_emptyColorArray);
            
            // Setup compute shader
            _computeShader.SetBuffer(_postProcessIndex, "_OKHSLArray", _okhslArrayBuffer);
            _computeShader.SetBuffer(_postProcessIndex, "_PostProcessArray", _okhslPostProcessBuffer);
            _computeShader.SetInts("_OKHSLArrayResolution", _okhslArraySize.x, _okhslArraySize.y);
            _computeShader.SetInt("_TopColorsCount", (int)_topColorsCount);

            _computeShader.Dispatch(_postProcessIndex, 1, 1, 1);
            _okhslPostProcessBuffer.GetData(_postProcessArray);

            // Get top colors
            TopColor[] topColors = new TopColor[_topColorsCount];
            for (int i = 0; i < (int) _topColorsCount; i++)
            {
                var rgb = _postProcessArray[i * 2];
                var okhsl = _postProcessArray[(i * 2) + 1];
                
                var topColor = new TopColor(new Color(rgb.r, rgb.g, rgb.b, 1), new Color(okhsl.r, okhsl.g, okhsl.b, 1), (int)okhsl.a, (int)rgb.a);
                topColors[i] = topColor;
            }

            // handle other metadata
            var otherMetadata = _postProcessArray[GetPostProcessDataLength() - 1];
            int totalColorCount = (int)otherMetadata.r;
            float totalColorCoverage = otherMetadata.g;
            
            // vibe check
            // var vibe = _vibeUtilites.GetVibe(GetOKHSLTopColorsFromPostProcessData(_postProcessArray, _topColorsCount));

            return new OKHSLData(topColors, totalColorCount, totalColorCoverage);
        }

        private void ReleaseResource(ref ComputeBuffer? buffer)
        {
            if (buffer == null)
                return;
            
            buffer.Release();
            buffer = null;
        }
        
        private void ReleaseResource(ref RenderTexture? renderTexture)
        {
            if (renderTexture == null)
                return;
            
            renderTexture.Release();
            renderTexture = null;
        }

        private int GetPostProcessDataLength()
        {
            return ((int)_topColorsCount * 2) + 1;
        }
    }
}