﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AuraTween;
using Cysharp.Threading.Tasks;
using Input;
using Resat.Audio;
using Resat.Biomes;
using Resat.Colors;
using Resat.Input;
using Resat.Intermediates;
using Resat.Models;
using Resat.Models.Events;
using Resat.Tweening;
using Resat.UI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Resat.Cameras
{
    // this class is a bit of a monolith. it was meant to be rewritten, but i don't have enough time if I want to finish the jam at this point
    // it handles input, camera state, and UI state
    public class PhotoController : MonoBehaviour, ResatInput.ICameraActions
    {
        [Header("Dependencies")]
        [SerializeField]
        private OKHSLController _okhslController = null!;
        
        [SerializeField]
        private InputController _inputController = null!;
        
        [SerializeField]
        private TweenController _tweenController = null!;

        [SerializeField]
        private BiomeController _biomeController = null!;

        [SerializeField]
        private BiomeIntermediate _biomeIntermediate = null!;

        [SerializeField]
        private ResatCamera _resatCamera = null!;
        
        [SerializeField]
        private DesaturationCamera _desaturationCamera = null!;

        [SerializeField]
        private CameraAudioController _cameraAudioController = null!;
        
        [SerializeField]
        private CameraPanelController _cameraPanelController = null!;

        [SerializeField]
        private RectTransform? _topSidePanel;

        // atp i'm just directly interacting with UI elements, i need to get something out
        [SerializeField]
        private OutroPanel _outroPanel = null!;
        
        [SerializeField]
        private CornerPanel _cameraViewportPanel = null!;
        
        [SerializeField]
        private DialoguePanel _topNotificationPanel = null!;

        [SerializeField]
        private CameraIntermediate _cameraIntermediate = null!;

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
        private float _animationDuration = 1f;
        
        [SerializeField]
        private float _sidePanelAnimationDuration = 1f;
        
        [SerializeField]
        private float _outroFadeAnimationDuration = 1f;
        
        [SerializeField]
        private float _outroScaleAnimationDuration = 1f;
        
        [SerializeField]
        private float _outroScaleInAnimationDuration = 1f;
        
        [SerializeField]
        private float _notificationShowTime = 1f;

        [SerializeField]
        private TextAnimationSpeed _notificationTextSpeed = new(1f, 1f);

        [SerializeField]
        private Color _outroCutoutColor = Color.black;
        
        [SerializeField]
        private Ease _outroFadeEase = Ease.Linear;

        [SerializeField]
        private bool _tempOutro = true;

        private CameraState _cameraState = CameraState.Minimized;
        private float _animationPercent = 0f;
        
        private CameraResolutionData _currentResolutionData = new();
        private bool _forceOverrideActive = false;
        private OKHSLData? _previousOkhslData;
        private List<ScreenshotData> _screenshots = new();
        private CancellationTokenSource? _topNotificationCts;
        
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

        private void SetOutroAnimationPercent(float animationPercent)
        {
            // rgb lerp is fine since we're working in grayscale
            var lerpedColor = Color.Lerp(_desaturationCamera.OutsideCutoutColor, _outroCutoutColor, animationPercent);
            _desaturationCamera.SetOutsideCutoutColor(lerpedColor);
        }

        // DEBUGGING ONLY!!
        public void SetSize(Vector2Int main, Vector2Int native)
        {
            _photoResolutionData.Resolution = main;
            _photoResolutionData.NativeResolution = native;
        }
        
        private void SetCameraState(CameraState cameraState, string? message = null)
        {
            if (message != null)
                Debug.Log(message);

            _cameraState = cameraState;
        }
        
        private async UniTask AnimateAfterPicture()
        {
            SetAnimationPercent(0f); // just in case, likely unnecessary
            SetCameraState(CameraState.TakingPhoto, "Taking photo!");

            await _tweenController.RunTween(_animationDuration, SetAnimationPercent);

            SetAnimationPercent(1f); // just in case, likely unnecessary
            SetCameraState(CameraState.InView, "Finished taking photo!");
        }
        
        private async UniTask AnimateAfterPictureOutro()
        {
            SetAnimationPercent(0f); // just in case, likely unnecessary
            SetCameraState(CameraState.TakingPhoto, "Taking photo!");

            await _tweenController.RunTween(_animationDuration, SetAnimationPercent);
        }

        private async UniTask AnimateOpenCamera(bool force)
        {
            var openCam = _cameraViewportPanel.Open(force, null, SetViewportOpenVector);

            // we need to pull the side panel earlier than the tween's normal completion
            if (!force)
            {
                await UniTask.WaitForSeconds(_cameraViewportPanel.TweenSettings.Duration - _sidePanelAnimationDuration - 0.21f);
            }
            
            await AnimateSidePanel(force);

            await openCam;
        }
        
        private async UniTask AnimateCloseCamera(bool force)
        {
            var sidePanel = AnimateSidePanel(force, true);
            await _cameraViewportPanel.Close(force, null, SetViewportOpenVector);

            await sidePanel;
        }

        private async UniTask AnimateSidePanel(bool force, bool reverse = false)
        {
            if (_topSidePanel == null)
                return;

            if (force)
            {
                _topSidePanel.localScale = new Vector3(1, reverse ? 0 : 1, 1);
                return;
            }
            
            // TODO NON LINEAR EASE
            await _tweenController.RunTween(_sidePanelAnimationDuration, (value) =>
            {
                _topSidePanel.localScale = new Vector3(1, reverse ? 1 - value : value, 1);
            });
        }

        // this should force the camera to the new state regardless of what's happening
        public async UniTask ToggleCameraExternal(bool active, bool tempDisableUserControl = false)
        {
            if (tempDisableUserControl)
            {
                _forceOverrideActive = active;
            }
            
            if (_forceOverrideActive && !tempDisableUserControl)
                return;
            
            _cameraAudioController.PlaySoundEffect(active ? SoundEffect.MenuOpen : SoundEffect.MenuClose);

            if (active && _cameraState != CameraState.InView)
            {
                SetCameraState(CameraState.InView, "Enabling camera!");
                await AnimateOpenCamera(false);
            }
            else if (_cameraState != CameraState.Minimized)
            {
                SetCameraState(CameraState.Minimized, "Disabling camera!");
                await AnimateCloseCamera(false);
            }
        }
        
        private void SetViewportOpenVector(Vector2 value)
        {
            var fakerd = new CameraResolutionData();
            fakerd.Center = _photoResolutionData.Center;
            fakerd.Resolution = new Vector2Int((int)value.x, (int)value.y);
            _desaturationCamera.SetResolution(fakerd);
            
            // estimate how done we are based on viewport X
            // gross hack
            var approximateValue = (value.x / _photoResolutionData.GetRescaledResolution(new Vector2(1920, 1080)).x);
            // rgb lerp is fine since we're working in grayscale
            var lerpedColor = Color.Lerp(Color.white, _desaturationCamera.OutsideCutoutColor, approximateValue);
            _desaturationCamera.SetOutsideCutoutColor(lerpedColor);
        }

        public void EnableCamera(bool soundEffects = true, bool force = false)
        {
            if (force)
                _forceOverrideActive = true;

            if (_forceOverrideActive && !force)
                return;

            var biomeIsUnlocked = _biomeController.BiomeIsUnlocked();
            if (!biomeIsUnlocked)
            {
                Debug.Log("nah. we not unlocked");
                ShowTopNotification("An evil presence in this area overwhelms you.").Forget();
                _cameraAudioController.PlaySoundEffect(SoundEffect.InvalidOperation);
                return;
            }

            if (soundEffects)
                _cameraAudioController.PlaySoundEffect(SoundEffect.MenuOpen);
            
            SetCameraState(CameraState.InView, "Enabling camera!");
            AnimateOpenCamera(force).Forget();
        }

        private async UniTask ShowTopNotification(string content)
        {
            var notificationCts = new CancellationTokenSource();
            if (_topNotificationCts != null)
            {
                _topNotificationCts.Cancel();
            }
            _topNotificationCts = notificationCts;
            
            await _topNotificationPanel.OpenWithText(content, _notificationTextSpeed, false, null, null, null, _topNotificationCts.Token);
            
            if (notificationCts.IsCancellationRequested)
                return;
            
            await UniTask.WaitForSeconds(_notificationShowTime).AttachExternalCancellation(_topNotificationCts.Token);
            
            if (notificationCts.IsCancellationRequested)
                return;
            
            await _topNotificationPanel.CloseWithText(content, _notificationTextSpeed, false, null, null, null, _topNotificationCts.Token);
            
            notificationCts.Cancel();
            if (_topNotificationCts == notificationCts)
            {
                _topNotificationCts = null;
            }
        }
        
        public void DisableCamera(bool soundEffects = true, bool force = false)
        {
            if (force)
                _forceOverrideActive = false;

            if (_forceOverrideActive && !force)
                return;

            if (soundEffects)
                _cameraAudioController.PlaySoundEffect(SoundEffect.MenuClose);

            SetCameraState(CameraState.Minimized, "Disabling camera!");
            AnimateCloseCamera(force).Forget();
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
            _cameraIntermediate.TakePhoto();
            _cameraAudioController.PlaySoundEffect(SoundEffect.CameraShutter);
            
            // serialize our new color data
            _okhslController.SerializeLastRender();

            // else we do it in HandlePictureData
            if (!_tempOutro)
            {
                AnimateAfterPicture().Forget();
            }
            else
            {
                // we want to disable input if we're doing an outro sequence
                _inputController.DisablePlayerInput();
            }
            
            // TODO: This assumes native res will always cleanly divide into the screenshot's native res
            var scaleMultiplier = new Vector2((float)_screenshotNativeResolution.x / (float)_currentResolutionData.NativeResolution.x, (float)_screenshotNativeResolution.y / (float)_currentResolutionData.NativeResolution.y);
            var screenshotResolution = new Vector2Int((int)(_currentResolutionData.Resolution.x * scaleMultiplier.x), (int)(_currentResolutionData.Resolution.y * scaleMultiplier.y));
            
            var screenshotResolutionData = new CameraResolutionData(screenshotResolution, _screenshotNativeResolution, _currentResolutionData.Center, FilterMode.Bilinear, RenderTextureReadWrite.sRGB);
            
            var renderTexture = _resatCamera.RenderScreenshot(screenshotResolutionData, _savePhotosToDisk);

            // serialize taken photo data
            if (renderTexture != null)
            {
                HandlePictureData(renderTexture).Forget();
            }
        }

        private async UniTask HandlePictureData(RenderTexture renderTexture)
        {
            if (_previousOkhslData == null)
                return;
            
            var screenshotData = new ScreenshotData(renderTexture, _previousOkhslData);
            _screenshots.Add(screenshotData);

            if (_tempOutro)
            {
                // finally play animation
                await PlayOutro();
            }

            if (_screenshots.Count == 3)
            {
                // _tempOutro = true;
            }
        }

        private async UniTask PlayOutro()
        {
            // finally play animation
            await AnimateAfterPictureOutro();

            Debug.Log("Animation done, playing outro...");
            
            // run fade to black
            await _tweenController.RunTween(_outroFadeAnimationDuration, SetOutroAnimationPercent, _outroFadeEase);
            
            // run transition to UI
            // TODO: Panel should not be interactive at this point
            _outroPanel.SetScreenshotData(_screenshots);
            _outroPanel.SetActive(true);

            var scaleOut = _outroPanel.AnimateScaleOut(_outroScaleAnimationDuration);
            
            // wait partial so the scale in starts a bit sooner
            await UniTask.WaitForSeconds(_outroScaleAnimationDuration * 0.6f);
            
            await _outroPanel.AnimateScaleIn(_outroScaleInAnimationDuration);
            await scaleOut; // await just in case we haven't met the condition already
            
            // we done done
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
            _previousOkhslData = okhslData;
            
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
                EnableCamera(false, true);
            else
                DisableCamera(false, true);
            
            if (_okhslController.OutputArrayTexture != null)
                _cameraPanelController.SetArrayTexture(_okhslController.OutputArrayTexture);

            _topNotificationPanel.CloseWithText("", _notificationTextSpeed, true).Forget();
        }

        private void OnEnable()
        {
            _inputController.Input.Camera.AddCallbacks(this);
            _biomeIntermediate.BiomeChanged += OnBiomeChanged;
        }

        private void OnBiomeChanged(BiomeChangeEvent biomeChangeEvent)
        {
            var unlocked = _biomeController.BiomeIsUnlocked(biomeChangeEvent.Biome.BiomeType);
            if (!unlocked)
            {
                Debug.Log("New biome is NOT unlocked!");
            }
        }

        private void OnDisable()
        {
            _inputController.Input.Camera.RemoveCallbacks(this);
            _biomeIntermediate.BiomeChanged -= OnBiomeChanged;
        }
    }
}