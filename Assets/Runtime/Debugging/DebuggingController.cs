﻿using Input;
using Resat.Cameras;
using Resat.Colors;
using Resat.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Resat.Debugging
{
    public class DebuggingController : MonoBehaviour, ResatInput.IDebuggingActions
    {
        [SerializeField]
        private InputController _inputController = null!;

        [SerializeField]
        private DesaturationCamera _desaturationCamera = null!;
        
        [SerializeField]
        private PhotoController _photoController = null!;
        
        [SerializeField]
        private OKHSLController _okhslController = null!;

        public void OnDefaultCamView(InputAction.CallbackContext context)
        {
            if (!context.performed || _desaturationCamera.Material == null)
                return;
            
            _desaturationCamera.Material.EnableKeyword("DISABLE_SHOW_OKHSL_VIEW");
            // throw new System.NotImplementedException();
        }

        public void OnOKHSLView(InputAction.CallbackContext context)
        {
            if (!context.performed || _desaturationCamera.Material == null)
                return;
            
            _desaturationCamera.Material.DisableKeyword("DISABLE_SHOW_OKHSL_VIEW");
            // throw new System.NotImplementedException();
        }

        public void OnDesaturatedView(InputAction.CallbackContext context)
        {
            if (!context.performed) 
                return;

            _okhslController.SetArraySize(new Vector2Int(2048, 2048));
            _okhslController.enabled = false;
            _okhslController.enabled = true;

            int mult = 8;
            _photoController.SetSize(new Vector2Int(272*mult, 200*mult), new Vector2Int(480*mult, 270*mult));
            _photoController.enabled = false;
            _photoController.enabled = true;
            Debug.Log("stress testing");
            // throw new System.NotImplementedException();
        }

        public void OnResaturatedView(InputAction.CallbackContext context)
        {
            if (!context.performed) 
                return;

            // throw new System.NotImplementedException();
        }

        public void OnResetGlobalArray(InputAction.CallbackContext context)
        {
            if (!context.performed) 
                return;
            
            // throw new System.NotImplementedException();
            _okhslController.ClearGlobalArray();
        }

        private void Start()
        {
#if UNITY_EDITOR
            _inputController.EnableDebugInput();
#endif
        }
        
        private void OnEnable()
        {
            _inputController.Input.Debugging.AddCallbacks(this);
        }

        private void OnDisable()
        {
            _inputController.Input.Debugging.RemoveCallbacks(this);
        }
    }
}