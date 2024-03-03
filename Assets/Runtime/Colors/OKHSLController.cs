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
        
        private RenderTexture? _outputArrayTexture;
        private ComputeBuffer? _okhslArrayBuffer;
        private ComputeBuffer? _okhslPostProcessBuffer;

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
            _okhslPostProcessBuffer = new ComputeBuffer(GetPostProcessDataLength(), 4 * 4);

            _outputArrayTexture = new RenderTexture(_okhslArraySize.x, _okhslArraySize.y, 0, RenderTextureFormat.R8, RenderTextureReadWrite.Linear);
            _outputArrayTexture.enableRandomWrite = true;
            _outputArrayTexture.filterMode = FilterMode.Point;
            _outputArrayTexture.Create();
            
            _outputArray = new int[_okhslArraySize.x * _okhslArraySize.y];
            _postProcessArray = new Color[GetPostProcessDataLength()];
            ResetGlobalOkhslArray();
            
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
            AddToGlobalOkhslArray(_outputArray);
            Debug.Log(_globalOkhslArray?.Sum());
        }
        
        // public void RunComputeShader(CameraResolutionData cameraResolutionData, ResatCamera resatCamera)
        public OKHSLData? RunComputeShader(RenderTexture renderTexture)
        {
            if (_okhslArrayBuffer == null)
                return null;

            if (renderTexture.width % 8 != 0 || renderTexture.height % 8 != 0)
            {
                Debug.LogError("Passed RenderTexture was not a power of eight!");
                // TODO: See if non-power of eights actually break anything, or if we can just continue fine
                return null;
            }
            
            // Reset arrays to avoid data leaking from old textures
            _okhslArrayBuffer.SetData(_emptyIntArray);
            RenderTexture.active = _outputArrayTexture;
            GL.Clear(false, true, Color.black);

            // Setup compute shader
            _computeShader.SetTexture(0, "_InputTexture", renderTexture);
            _computeShader.SetTexture(0, "_OutputArrayTexture", _outputArrayTexture);
            _computeShader.SetBuffer(0, "_OKHSLArray", _okhslArrayBuffer);
            _computeShader.SetInts("_OKHSLArrayResolution", _okhslArraySize.x, _okhslArraySize.y);

            // Dispatch initial patch to get OKHSL array info
            _computeShader.Dispatch(0, renderTexture.width / 8, renderTexture.height / 8, 1);
            _okhslArrayBuffer.GetData(_outputArray);

            return PostProcessing ? PostProcess() : null;
        }
        
        // TODO: Combine this all into one pass. it's probably possible (maybe just minus the sorting?)
        // could just remove the sorting, or add a seperate dedicated compute shader/shader for that
        private OKHSLData? PostProcess()
        {
            if (_okhslPostProcessBuffer == null || _postProcessArray == null) 
                return null;

            // Reset arrays to avoid data leaking from old textures
            _okhslPostProcessBuffer.SetData(_emptyColorArray);
            
            _computeShader.SetBuffer(1, "_OKHSLArray", _okhslArrayBuffer);
            _computeShader.SetBuffer(1, "_PostProcessArray", _okhslPostProcessBuffer);
            _computeShader.SetInts("_OKHSLArrayResolution", _okhslArraySize.x, _okhslArraySize.y);
            _computeShader.SetInt("_TopColorsCount", (int)_topColorsCount);

            _computeShader.Dispatch(1, 1, 1, 1);
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