using System;
using System.Collections.Generic;
using System.Linq;
using Resat.Cameras;
using Resat.Models;
using UnityEngine;

namespace Resat.Colors
{
    // Wrapper for compute shader
    public class OKHSLController : MonoBehaviour
    {
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
        private ComputeBuffer? _topColorArrayBuffer;
        private ComputeBuffer? _metadataBuffer;

        private int _calculateArrayIndex;
        private int _postProcessIndex;
        private int _getPreviewTextureIndex;
        private int _getGlobalTextureIndex;
        private int _getMergedTextureIndex;
        private int _mergeArrayIndex;
        private int _clearGlobalArrayIndex;
        private int _clearPreviewArrayIndex;
        private int _clearMetadataArrayIndex;
        
        // arrays reused to avoid allocating every capture
        private TopColor[]? _topColorArray;
        private uint[]? _otherMetadataArray;
        private TopColor[]? _sortingArray;
        
        private void OnEnable()
        {
            if (_topColorsCount > _okhslArraySize.y)
            {
                Debug.LogError("Top color count must be less than the OKHSL array Y size.");
                return;
            }
            
            // int is 4 bytes and color is four 4-byte floats (4*4)
            _okhslArrayBuffer = new ComputeBuffer(_okhslArraySize.x * _okhslArraySize.y, 4);
            _globalOkhslArrayBuffer = new ComputeBuffer(_okhslArraySize.x * _okhslArraySize.y, 4);
            _topColorArrayBuffer = new ComputeBuffer(GetPostProcessDataLength(), 4 * 4);
            _metadataBuffer = new ComputeBuffer(2, 4); // almost certainly an unnecessary thing for 2 ints

            // used for desat
            Shader.SetGlobalBuffer("_GlobalOKHSLBuffer", _globalOkhslArrayBuffer);
            Shader.SetGlobalVector("_OKHSLArrayResolution", new Vector4(_okhslArraySize.x, _okhslArraySize.y, 1, 1));
            
            _outputArrayTexture = new RenderTexture(_okhslArraySize.x, _okhslArraySize.y, 0, RenderTextureFormat.R8, RenderTextureReadWrite.Linear);
            _outputArrayTexture.enableRandomWrite = true;
            _outputArrayTexture.filterMode = FilterMode.Point;
            _outputArrayTexture.Create();
            
            // _outputArray = new uint[_okhslArraySize.x * _okhslArraySize.y];
            _topColorArray = new TopColor[GetPostProcessDataLength() / 2];
            _otherMetadataArray = new uint[2];
            _sortingArray = new TopColor[_topColorsCount];

            // set indexes used for setting variables
            _calculateArrayIndex = _computeShader.FindKernel("CSCalculateArray");
            _postProcessIndex = _computeShader.FindKernel("CSPostProcess");
            _getPreviewTextureIndex = _computeShader.FindKernel("CSGetPreviewTexture");
            _getGlobalTextureIndex = _computeShader.FindKernel("CSGetGlobalTexture");
            _getMergedTextureIndex = _computeShader.FindKernel("CSGetMergedTexture");
            _mergeArrayIndex = _computeShader.FindKernel("CSMergeArrays");
            _clearGlobalArrayIndex = _computeShader.FindKernel("CSClearGlobalArray");
            _clearPreviewArrayIndex = _computeShader.FindKernel("CSClearPreviewArray");
            _clearMetadataArrayIndex = _computeShader.FindKernel("CSClearMetadataArray");
            
            // reset global array to be empty
            ClearGlobalArray();
        }
        
        private void OnDisable()
        {
            ReleaseResource(ref _okhslArrayBuffer);
            ReleaseResource(ref _globalOkhslArrayBuffer);
            ReleaseResource(ref _topColorArrayBuffer);
            ReleaseResource(ref _metadataBuffer);
            ReleaseResource(ref _outputArrayTexture);
            
            _topColorArray = null;
        }

        private void ClearGlobalArray()
        {
            _computeShader.SetBuffer(_clearGlobalArrayIndex, "_GlobalOKHSLArray", _globalOkhslArrayBuffer);
            _computeShader.Dispatch(_clearGlobalArrayIndex, _okhslArraySize.x / 8, _okhslArraySize.y / 8, 1);
        }
        
        private void ClearPreviewArray()
        {
            _computeShader.SetBuffer(_clearPreviewArrayIndex, "_OKHSLArray", _okhslArrayBuffer);
            _computeShader.Dispatch(_clearPreviewArrayIndex, _okhslArraySize.x / 8, _okhslArraySize.y / 8, 1);
        }
        
        // idk if this is faster than setdata
        private void ClearPostProcessingArrays()
        {
            _computeShader.SetBuffer(_clearMetadataArrayIndex, "_OtherMetadataArray", _metadataBuffer);
            _computeShader.Dispatch(_clearMetadataArrayIndex, 1, 1, 1);
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
            ClearPreviewArray();
            
            // Setup compute shader
            _computeShader.SetTexture(_calculateArrayIndex, "_InputTexture", renderTexture);
            _computeShader.SetBuffer(_calculateArrayIndex, "_OKHSLArray", _okhslArrayBuffer);
            _computeShader.SetInts("_OKHSLArrayResolution", _okhslArraySize.x, _okhslArraySize.y);

            // Dispatch initial patch to get OKHSL array info
            _computeShader.Dispatch(_calculateArrayIndex, renderTexture.width / 8, renderTexture.height / 8, 1);
            // _okhslArrayBuffer.GetData(_outputArray);

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
            {
                _computeShader.SetBuffer(index, "_OKHSLArray", _okhslArrayBuffer);
            }
            if (arrayType != OKHSLArrayType.Preview)
            {
                _computeShader.SetBuffer(index, "_GlobalOKHSLArray", _globalOkhslArrayBuffer);
            }
            
            _computeShader.SetTexture(index, "_OutputArrayTexture", renderTexture);
            
            _computeShader.Dispatch(index, _okhslArraySize.x / 8, _okhslArraySize.y / 8, 1);
        }
        
        // TODO: Combine this all into one pass. it's probably possible (maybe just minus the sorting?)
        // could just remove the sorting, or add a seperate dedicated compute shader/shader for that
        private OKHSLData? PostProcess()
        {
            // Sort to Length.Y largest elements

            if (_topColorArrayBuffer == null || _metadataBuffer == null || _topColorArray == null || _otherMetadataArray == null) 
                return null;

            // Reset arrays to avoid data leaking from old textures
            ClearPostProcessingArrays();
            
            // Setup compute shader
            _computeShader.SetBuffer(_postProcessIndex, "_OKHSLArray", _okhslArrayBuffer);
            _computeShader.SetBuffer(_postProcessIndex, "_GlobalOKHSLArray", _globalOkhslArrayBuffer);
            _computeShader.SetBuffer(_postProcessIndex, "_TopColorArray", _topColorArrayBuffer);
            _computeShader.SetBuffer(_postProcessIndex, "_OtherMetadataArray", _metadataBuffer);
            _computeShader.SetInts("_OKHSLArrayResolution", _okhslArraySize.x, _okhslArraySize.y);

            _computeShader.Dispatch(_postProcessIndex, _okhslArraySize.y / 8, 1, 1);
            
            // Get top colors
            _topColorArrayBuffer.GetData(_topColorArray);
            
            /*
            List<TopColor> topColors = new();
            for (int i = 0; i < _okhslArraySize.y; i++)
            {
                var rgb = _topColorArray[i * 2];
                var okhsl = _topColorArray[(i * 2) + 1];
                
                var topColor = new TopColor(new Color(rgb.r, rgb.g, rgb.b, 1), new Color(okhsl.r, okhsl.g, okhsl.b, 1), (int)okhsl.a, (int)rgb.a);
                topColors.Add(topColor);
            }
            */
            
            // Sort on CPU
            // var topColorsArray = _topColorArray.OrderByDescending(x => x.Count).Take((int)_topColorsCount).ToArray();
            // Array.Sort(_topColorArray, (color, topColor) => (int)(topColor.Count - color.Count)); // doesn't allocate; IComparer does
            var sortedTopColorArray = SortTopColorArray(_topColorArray);
            
            // Handle other non-topcolor metadata
            _metadataBuffer.GetData(_otherMetadataArray);
            uint totalColorCount = _otherMetadataArray[0];
            float totalColorCoverage = ((float)totalColorCount / (_okhslArraySize.x * _okhslArraySize.y)) * 100f;

            // vibe check
            // var vibe = _vibeUtilites.GetVibe(GetOKHSLTopColorsFromPostProcessData(_postProcessArray, _topColorsCount));

            return new OKHSLData(sortedTopColorArray, totalColorCount, totalColorCoverage);
        }

        // Made specifically to truncate and sort an array in one swoop
        // Probably possible to use a more efficient algorithm if we're really squeezing performance
        private TopColor[] SortTopColorArray(TopColor[] topColors)
        {
            if (_sortingArray == null)
                _sortingArray = new TopColor[_topColorsCount];
            
            Array.Clear(_sortingArray, 0, _sortingArray.Length);
            
            float lowestInArray = 0f;
            for (int i = 0; i < topColors.Length; i++)
            {
                var topColor = topColors[i];
                if (topColor.Count < lowestInArray) 
                    continue;

                for (int j = 0; j < _sortingArray.Length; j++)
                {
                    var arrayTopColor = _sortingArray[j];
                    if (topColor.Count > arrayTopColor.Count)
                    {
                        if (j + 1 < _sortingArray.Length)
                            Array.Copy(_sortingArray, j, _sortingArray, j + 1, _sortingArray.Length - (j + 1));
                        else
                            lowestInArray = arrayTopColor.Count;

                        _sortingArray[j] = topColor;
                        break;
                    }
                }
            }

            return _sortingArray.ToArray();
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
            // NOTE: Top color count interfacing with the GPU is clamped to OKHSLscale.y
            // return ((int)_topColorsCount * 2) + 1;
            return (_okhslArraySize.y * 2);
        }
    }
}