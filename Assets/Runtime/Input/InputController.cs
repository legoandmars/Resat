using Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Resat.Input
{
    public class InputController : MonoBehaviour
    {
        [HideInInspector]
        public bool PlayerInputEnabled = false;
        public ResatInput Input = null!;

        public void Start()
        {
            Input = new ResatInput();
        }

        public void EnablePlayerInput()
        {
            Input.Player.Enable();
            PlayerInputEnabled = true;
            Cursor.lockState = CursorLockMode.Locked;
        }

        public void DisablePlayerInput()
        {
            Input.Player.Disable();
            PlayerInputEnabled = false;
            Cursor.lockState = CursorLockMode.None;
        }
    }
}