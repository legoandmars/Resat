using Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Resat.Input
{
    public class InputController : MonoBehaviour
    {
        [HideInInspector]
        public bool PlayerInputEnabled = false;
        
        [HideInInspector]
        public bool CameraInputEnabled = false;
        
        private ResatInput? _input = null;
        public ResatInput Input
        {
            get
            {
                if (_input == null)
                    _input = new ResatInput();

                return _input;
            }
        }

        public void EnablePlayerInput()
        {
            Input.Player.Enable();
            PlayerInputEnabled = true;
            Cursor.lockState = CursorLockMode.Locked;
        }
        
        public void EnableCameraInput()
        {
            Input.Camera.Enable();
            CameraInputEnabled = true;
        }

        public void EnableDebugInput()
        {
            Input.Debugging.Enable();
        }

        public void DisablePlayerInput()
        {
            Input.Player.Disable();
            PlayerInputEnabled = false;
            Cursor.lockState = CursorLockMode.None;
        }
    }
}