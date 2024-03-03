using System;
using Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Resat.Cameras
{
    public class PhotoController : MonoBehaviour, ResatInput.ICameraActions
    {
        [SerializeField]
        private DesaturationCamera _desaturationCamera = null!;

        [NonSerialized]
        public bool Enabled;

        private void SetResolution(Vector2Int resolution)
        {
            
        }
        private void EnableCamera()
        {
            // guh
        }
        
        public void OnTakePicture(InputAction.CallbackContext context)
        {
            if (!Enabled) 
                return;
        }

        public void OnToggleCamera(InputAction.CallbackContext context)
        {
            if (!Enabled)
            {
                EnableCamera();
            } 
        }
    }
}