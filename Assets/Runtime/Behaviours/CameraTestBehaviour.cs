using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Resat
{
    public class CameraTestBehaviour : MonoBehaviour
    {
        [SerializeField]
        private ComputeShader _computeShader = null!;

        [SerializeField]
        private Camera _inputCamera = null!;

        [SerializeField]
        private RawImage? DebugCameraImage;
        
        [SerializeField]
        private RawImage? DebugArrayImage;
        
        [SerializeField]
        private int _inputTextureResolution = 32;
        
        [SerializeField]
        private float _minimumSaturation = 0.5f;

        [SerializeField]
        private Vector2Int _okhslArraySize = new(32, 32);

        private RenderTexture? _inputTexture;
        private RenderTexture? _outputArrayTexture;
        private ComputeBuffer? _okhslArrayBuffer;
        private int[]? _outputArray; // reused for performance

        // setup all necessary re-usable datatypes for compute shader
        private void OnEnable()
        {
            _okhslArrayBuffer = new ComputeBuffer(_okhslArraySize.x * _okhslArraySize.y, 4);
            _outputArray = new int[_okhslArraySize.x * _okhslArraySize.y];
            
            _inputTexture = new RenderTexture(_inputTextureResolution, _inputTextureResolution, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);
            _inputTexture.enableRandomWrite = true;
            _inputTexture.filterMode = FilterMode.Point;
            _inputTexture.Create();
            _inputCamera.targetTexture = _inputTexture;

            _outputArrayTexture = new RenderTexture(_okhslArraySize.x, _okhslArraySize.y, 0, RenderTextureFormat.R8, RenderTextureReadWrite.Linear);
            _outputArrayTexture.enableRandomWrite = true;
            _inputTexture.filterMode = FilterMode.Point;
            _outputArrayTexture.Create();

            if (DebugCameraImage != null)
                DebugCameraImage.texture = _inputTexture;
            
            if (DebugArrayImage != null)
                DebugArrayImage.texture = _outputArrayTexture;
        }
        
        // clear all textures/buffers/etc
        private void OnDisable()
        {
            _okhslArrayBuffer?.Release();
            _okhslArrayBuffer = null;

            if (_inputTexture != null)
            {
                _inputTexture.Release();
                _inputTexture = null;
            }
            
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
            /*
                Texture2D<fixed3> _InputTexture;
                RWStructuredBuffer<float3> _OKHSLArray;

                uint _InputTextureResolution;
                uint _OutputTextureResolution;
                uint _PaletteSize;
                float _MinimumSaturation;

             */
            _computeShader.SetTexture(0, "_InputTexture", _inputTexture);
            _computeShader.SetTexture(0, "_OutputArrayTexture", _outputArrayTexture);
            _computeShader.SetBuffer(0, "_OKHSLArray", _okhslArrayBuffer);
            _computeShader.SetInt("_InputTextureResolution", _inputTextureResolution);
            _computeShader.SetFloat("_MinimumSaturation", _minimumSaturation);
            _computeShader.SetInts("_OKHSLArrayResolution", _okhslArraySize.x, _okhslArraySize.y);
            
            int groups = Mathf.CeilToInt(_inputTextureResolution / 8f);
            _computeShader.Dispatch(0, groups, groups, 1);
            _okhslArrayBuffer.GetData(_outputArray);

            Debug.Log("AAAAAAAH!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
        }
    }
}
