using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using Input;
using Resat.Audio;
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
        private CameraAudioController _cameraAudioController = null!;
        
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
        
        [Header("Settings")]
        [SerializeField]
        private bool _savePhotosToDisk = true;

        [SerializeField]
        private bool _startWithCameraEnabled = false;

        [Header("Animation")]
        [SerializeField]
        private float _animationSpeed = 1f;

        private CameraState _cameraState = CameraState.Minimized;
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

        private void SetCameraState(CameraState cameraState, string? message = null)
        {
            if (message != null)
                Debug.Log(message);

            _cameraState = cameraState;
        }
        
        private async UniTask AnimateAfterPicture()
        {
            SetAnimationPercent(0f);
            SetCameraState(CameraState.TakingPhoto, "Taking photo!");
            
            while (_animationPercent < 1f)
            {
                await UniTask.NextFrame();
                SetAnimationPercent(_animationPercent + (Time.deltaTime * _animationSpeed));
            }

            SetAnimationPercent(1f);
            SetCameraState(CameraState.InView, "Finished taking photo!");
        }
        
        private void EnableCamera(bool soundEffects = true)
        {
            SetResolution(_photoResolutionData);
            
            if (soundEffects)
                _cameraAudioController.PlaySoundEffect(SoundEffect.MenuOpen);
            
            // TODO: Animation
            SetCameraState(CameraState.InView, "Enabling camera!");
            
            // Set outside color
            _desaturationCamera.SetOutsideCutoutColor();
        }
        
        private void DisableCamera(bool soundEffects = true)
        {
            SetResolution(_minimizedResolutionData);
            
            if (soundEffects)
                _cameraAudioController.PlaySoundEffect(SoundEffect.MenuClose);

            // TODO: Animation
            SetCameraState(CameraState.Minimized, "Disabling camera!");
            
            // Set outside color
            _desaturationCamera.SetOutsideCutoutColor(Color.white);
        }
        
        public void OnTakePicture(InputAction.CallbackContext context)
        {
            // TODO: Improve "Can't take picture" effect
            if (!context.performed)
                return;

            if (!CanTakePhoto())
            {
                if (_cameraState == CameraState.TakingPhoto)
                {
                    _cameraAudioController.PlaySoundEffect(SoundEffect.InvalidOperation);
                }
                return;
            }
            
            // take a picture!
            _cameraAudioController.PlaySoundEffect(SoundEffect.CameraShutter);
            
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

            if (_cameraState == CameraState.Minimized)
            {
                EnableCamera();
            }
            else if (_cameraState == CameraState.InView)
            {
                DisableCamera();
            }
        }

        private void RenderPreview()
        {
            if (_cameraState != CameraState.InView) 
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

        private bool CanTakePhoto()
        {
            // TODO: Hook this up to ammo or something
            return _cameraState == CameraState.InView;
        }
        
        private void Update()
        {
            if (_cameraState != CameraState.InView) 
                return;

            RenderPreview();
            SetFieldOfView(_fieldOfView); // temporary
        }
        
        private void Start()
        {
            _inputController.EnableCameraInput();
            
            if (_startWithCameraEnabled)
                EnableCamera(false);
            else
                DisableCamera(false);
            
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