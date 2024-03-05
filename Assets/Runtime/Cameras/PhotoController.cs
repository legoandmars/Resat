using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using Input;
using Resat.Colors;
using Resat.Input;
using Resat.Models;
using Resat.UI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Resat.Cameras
{
    public class PhotoController : MonoBehaviour, ResatInput.ICameraActions
    {
        [Header("Dependencies")]
        [SerializeField]
        private OKHSLController _okhslController = null!;
        
        [SerializeField]
        private InputController _inputController = null!;

        [SerializeField]
        private ResatCamera _resatCamera = null!;
        
        [SerializeField]
        private DesaturationCamera _desaturationCamera = null!;

        [SerializeField]
        private AudioSource? _cameraAudioSource;
        
        [SerializeField]
        private CameraPanelController _cameraPanelController = null!;

        [Header("Resolution")]
        [SerializeField]
        private CameraResolutionData _photoResolutionData = new();
        
        [SerializeField]
        private CameraResolutionData _minimizedResolutionData = new();

        [SerializeField]
        private Vector2Int _screenshotNativeResolution = new(1920, 1080);

        [SerializeField]
        private float _fieldOfView = 60f;
        
        [SerializeField]
        private bool _savePhotosToDisk = true;

        [Header("Animation")]
        [SerializeField]
        private float _animationSpeed = 1f;
        
        private bool _enabled;
        private bool _canTakePhotos = true;
        private float _animationPercent = 0f;
        
        private CameraResolutionData _currentResolutionData = new();
        
        private void SetResolution(CameraResolutionData resolutionData)
        {
            _currentResolutionData = resolutionData;
            _desaturationCamera.SetResolution(resolutionData);
            _cameraPanelController.SetResolution(resolutionData);
        }

        private void SetFieldOfView(float fieldOfView)
        {
            _fieldOfView = fieldOfView;
            _desaturationCamera.SetFieldOfView(fieldOfView);
            _resatCamera.SetFieldOfView(fieldOfView);
        }

        private void SetAnimationPercent(float animationPercent)
        {
            _animationPercent = Mathf.Clamp01(animationPercent);
            _desaturationCamera.SetAnimationPercent(_animationPercent);
        }

        private async UniTask AnimateAfterPicture()
        {
            SetAnimationPercent(0f);
            _canTakePhotos = false;

            while (_animationPercent < 1f)
            {
                await UniTask.NextFrame();
                SetAnimationPercent(_animationPercent + (Time.deltaTime * _animationSpeed));
                Debug.Log(_animationPercent);
            }

            SetAnimationPercent(1f);
            _canTakePhotos = true;
        }
        
        private void EnableCamera()
        {
            _enabled = true;
            SetResolution(_photoResolutionData);
        }
        
        private void DisableCamera()
        {
            _enabled = false;
            SetResolution(_minimizedResolutionData);
        }
        
        public void OnTakePicture(InputAction.CallbackContext context)
        {
            // TODO: Animation when a picture was attempted too soon
            if (!_enabled || !context.performed || !_canTakePhotos) 
                return;
            
            // take a picture!
            if (_cameraAudioSource != null)
                _cameraAudioSource.Play();
            
            // serialize our new color data
            _okhslController.SerializeLastRender();

            AnimateAfterPicture().Forget();
            
            if (!_savePhotosToDisk) 
                return;
            
            // TODO: This assumes native res will always cleanly divide into the screenshot's native res
            var scaleMultiplier = new Vector2((float)_screenshotNativeResolution.x / (float)_currentResolutionData.NativeResolution.x, (float)_screenshotNativeResolution.y / (float)_currentResolutionData.NativeResolution.y);
            var screenshotResolution = new Vector2Int((int)(_currentResolutionData.Resolution.x * scaleMultiplier.x), (int)(_currentResolutionData.Resolution.y * scaleMultiplier.y));
            
            var screenshotResolutionData = new CameraResolutionData(screenshotResolution, _screenshotNativeResolution, _currentResolutionData.Center, FilterMode.Bilinear, RenderTextureReadWrite.sRGB);
            _resatCamera.RenderScreenshot(screenshotResolutionData);
        }

        public void OnToggleCamera(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;
            
            Debug.Log("TOGGLING!");
            if (!_enabled)
                EnableCamera();
            else
                DisableCamera();
        }

        private void Update()
        {
            if (!_enabled) 
                return;

            RenderPreview();
            EnableCamera(); // temporary; meant so i can see resolution changes when debuggin
            SetFieldOfView(_fieldOfView);
        }

        private void RenderPreview()
        {
            if (!_enabled) 
                return;

            var renderTexture = _resatCamera.Render(_currentResolutionData);
            if (renderTexture == null)
                return;

            // set debug camera preview 
            _cameraPanelController.SetPreviewTexture(renderTexture);
            
            var okhslData = _okhslController.RunComputeShader(renderTexture);

            if (okhslData == null)
                return;
            
            _cameraPanelController.SetData(okhslData);
        }

        private void Start()
        {
            _inputController.EnableCameraInput();
            EnableCamera();
            
            if (_okhslController.OutputArrayTexture != null)
                _cameraPanelController.SetArrayTexture(_okhslController.OutputArrayTexture);
        }

        private void OnEnable()
        {
            _inputController.Input.Camera.AddCallbacks(this);
        }

        private void OnDisable()
        {
            _inputController.Input.Camera.RemoveCallbacks(this);
        }
    }
}