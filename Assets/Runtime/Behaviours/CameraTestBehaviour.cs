using System.Collections;
using System.Collections.Generic;
using Resat.Cameras;
using Resat.Colors;
using Resat.Input;
using Resat.Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Resat
{
    public class CameraTestBehaviour : MonoBehaviour
    {
        [SerializeField]
        private ComputeShader _computeShader = null!;

        [SerializeField]
        private ResatCamera _resatCamera = null!;
        
        [SerializeField]
        private DesaturationCamera _desaturationCamera = null!;
        
        [SerializeField]
        private InputController _inputController = null!;

        [SerializeField]
        private CameraResolutionData _inputTextureResolutionData = new();
        
        [SerializeField]
        private CameraResolutionData _screenshotResolutionData = new();

        [SerializeField]
        private Vector2Int _okhslArraySize = new(32, 32);

        [SerializeField]
        private uint _topColorsCount = 8;
        
        // Debug options
        [SerializeField]
        public RawImage? DebugCameraImage;
        
        [SerializeField]
        public RawImage? DebugArrayImage;
        
        [SerializeField]
        public List<RawImage>? DebugTopColorImages;
        
        [SerializeField]
        public TextMeshProUGUI? DebugUniqueColorCountText;
        
        [SerializeField]
        public TextMeshProUGUI? DebugColorCoveragePercentText;

        [SerializeField]
        public TextMeshProUGUI? DebugPhotoVibeText;

        private RenderTexture? _outputArrayTexture;
        private ComputeBuffer? _okhslArrayBuffer;
        private ComputeBuffer? _okhslPostProcessBuffer;
        private int[]? _outputArray; // reused for performance
        private Color[]? _postProcessArray; // reused for performance

        private VibeUtilites _vibeUtilites = new();
        
        // setup all necessary re-usable datatypes for compute shader
        private void OnEnable()
        {
            _okhslArrayBuffer = new ComputeBuffer(_okhslArraySize.x * _okhslArraySize.y, 4);
            _okhslPostProcessBuffer = new ComputeBuffer(GetPostProcessDataLength(), 16);
            _outputArray = new int[_okhslArraySize.x * _okhslArraySize.y];
            _postProcessArray = new Color[GetPostProcessDataLength()];

            _outputArrayTexture = new RenderTexture(_okhslArraySize.x, _okhslArraySize.y, 0, RenderTextureFormat.R8, RenderTextureReadWrite.Linear);
            _outputArrayTexture.enableRandomWrite = true;
            _outputArrayTexture.filterMode = FilterMode.Point;
            _outputArrayTexture.Create();
            
            if (DebugArrayImage != null)
                DebugArrayImage.texture = _outputArrayTexture;
        }
        
        // clear all textures/buffers/etc
        private void OnDisable()
        {
            _okhslArrayBuffer?.Release();
            _okhslArrayBuffer = null;
            _okhslPostProcessBuffer?.Release();
            _okhslPostProcessBuffer = null;
            
            if (_outputArrayTexture != null)
            {
                _outputArrayTexture.Release();
                _outputArrayTexture = null;
            }

            _outputArray = null;
        }
        
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            if (_okhslArrayBuffer == null || _outputArray == null || _outputArrayTexture == null) 
                return;
            
            // Get camera preview
            var inputTexture = _resatCamera.Render(_inputTextureResolutionData, true);
            if (inputTexture == null)
                return;

            // Set desaturation array
            // TODO: Only do this when the value changes, instead of in update
            _desaturationCamera.SetResolution(_inputTextureResolutionData);
            
            if (DebugCameraImage != null)
            {
                DebugCameraImage.texture = inputTexture;
                DebugCameraImage.color = Color.white;
            }

            // Reset
            // TODO: Cache empty arrays?
            _okhslArrayBuffer.SetData(new int[_okhslArraySize.x * _okhslArraySize.y]);
            RenderTexture.active = _outputArrayTexture;
            GL.Clear(false, true, Color.black);
            
            _computeShader.SetTexture(0, "_InputTexture", inputTexture);
            _computeShader.SetTexture(0, "_OutputArrayTexture", _outputArrayTexture);
            _computeShader.SetBuffer(0, "_OKHSLArray", _okhslArrayBuffer);
            _computeShader.SetInts("_InputTextureResolution", _inputTextureResolutionData.Resolution.x, _inputTextureResolutionData.Resolution.y);
            _computeShader.SetInts("_OKHSLArrayResolution", _okhslArraySize.x, _okhslArraySize.y);
            
            _computeShader.Dispatch(0, _inputTextureResolutionData.Resolution.x / 8, _inputTextureResolutionData.Resolution.y / 8, 1);
            _okhslArrayBuffer.GetData(_outputArray);

            Postprocess();
            
            // screenshot if necessary
            if (_inputController.Input.Player.Photograph.WasPressedThisFrame())
            {
                _resatCamera.RenderScreenshot(_screenshotResolutionData);
            }
        }

        void Postprocess()
        {
            if (_okhslPostProcessBuffer == null || _postProcessArray == null) 
                return;

            // Reset
            _okhslPostProcessBuffer.SetData(new Color[GetPostProcessDataLength()]);
            
            _computeShader.SetBuffer(1, "_OKHSLArray", _okhslArrayBuffer);
            _computeShader.SetBuffer(1, "_PostProcessArray", _okhslPostProcessBuffer);
            _computeShader.SetInts("_OKHSLArrayResolution", _okhslArraySize.x, _okhslArraySize.y);
            _computeShader.SetInt("_TopColorsCount", (int)_topColorsCount);
            
            _computeShader.Dispatch(1, 1, 1, 1);
            _okhslPostProcessBuffer.GetData(_postProcessArray);

            for (int i = 0; i < DebugTopColorImages?.Count && i < (int)_topColorsCount; i++)
            {
                var debugTopColorImage = DebugTopColorImages?[i];
                if (debugTopColorImage == null)
                    continue;

                Color topColor = _postProcessArray[i * 2];
                debugTopColorImage.color = new Color(topColor.r, topColor.g, topColor.b, 1);
            }
            
            // handle other metadata
            var otherMetadata = _postProcessArray[GetPostProcessDataLength() - 1];
            int uniqueColorCount = (int)otherMetadata.r;
            float totalColorCoverage = otherMetadata.g;
            
            // vibe check
            var vibe = _vibeUtilites.GetVibe(GetOKHSLTopColorsFromPostProcessData(_postProcessArray, _topColorsCount));

            if (DebugColorCoveragePercentText != null)
                DebugColorCoveragePercentText.text = $"Total color coverage: {totalColorCoverage:0.##}%";
            if (DebugUniqueColorCountText != null)
                DebugUniqueColorCountText.text = $"Total new colors: {uniqueColorCount}";
            if (DebugPhotoVibeText != null)
                DebugPhotoVibeText.text = $"Photo vibes: {vibe}";
        }

        private List<Color> GetOKHSLTopColorsFromPostProcessData(Color[] postProcessArray, uint topColorsCount)
        {
            List<Color> okhslColors = new();
            for (int i = 0; i < topColorsCount; i++)
            {
                okhslColors.Add(postProcessArray[i + 1]);
            }

            return okhslColors;
        }
        
        private int GetPostProcessDataLength()
        {
            return ((int)_topColorsCount * 2) + 1;
        }
    }
}
