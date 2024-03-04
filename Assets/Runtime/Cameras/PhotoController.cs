using System;
using System.Linq;
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
        private AudioSource? _cameraAudioSource;
        
        [SerializeField]
        private CameraPanelController _cameraPanelController = null!;

        [Header("Resolution")]
        [SerializeField]
        private CameraResolutionData _photoResolutionData = new();
        
        [SerializeField]
        private CameraResolutionData _minimizedResolutionData = new();

        [SerializeField]
        private CameraResolutionData _screenshotResolutionData = new();

        [NonSerialized]
        private bool _enabled;

        private CameraResolutionData _currentResolutionData = new();
        
        private void SetResolution(CameraResolutionData resolutionData)
        {
            _currentResolutionData = resolutionData;
            _cameraPanelController.SetResolution(resolutionData);
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
            if (!_enabled || !context.performed) 
                return;
            
            // take a picture!
            if (_cameraAudioSource != null)
                _cameraAudioSource.Play();

            _resatCamera.RenderScreenshot(_screenshotResolutionData);

            // serialize our new color data
            _okhslController.SerializeLastRender();
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
            EnableCamera(); // temporary; meant so i can redisable the camera when debugging
        }

        private void OnDisable()
        {
            _inputController.Input.Camera.RemoveCallbacks(this);
        }
    }
}