using System.Collections;
using System.Collections.Generic;
using Input;
using Resat.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Resat.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField]
        private InputController _inputController = null!;
        
        [SerializeField]
        private CharacterController _characterController = null!;
        
        [SerializeField]
        private Camera _camera = null!;
        
        [SerializeField]
        private float _lookSensitivity = 1f;
        
        [SerializeField]
        private float _speed = 1f;
        
        [SerializeField]
        private float _jumpHeight = 1f;
        
        [SerializeField]
        private float _fallSpeed = 9.8f;

        private float _verticalVelocity;
        private float _groundedTimer;        // to allow jumping when going down ramps
        
        void Start()
        {
            _inputController.EnablePlayerInput();
        }

        void Update()
        {
            if (!_inputController.PlayerInputEnabled) 
                return;
            
            // Camera movement
            Vector2 lookValue = _inputController.Input.Player.Look.ReadValue<Vector2>() * _lookSensitivity * 0.1f;
            Vector3 angles = _camera.transform.localEulerAngles + new Vector3(-lookValue.y, lookValue.x, 0);
            
            if (angles.x > 180)
                angles.x -= 360;
            
            angles.x = Mathf.Clamp(angles.x, -90f, 90f);

            _camera.transform.localEulerAngles = angles;
            
            // Player movement
            Vector2 moveValue = _inputController.Input.Player.Move.ReadValue<Vector2>() * _speed * Time.deltaTime;
            Quaternion moveDirection = Quaternion.Euler(0, angles.y, 0); // apply camera rotation to move in the correct direction

            // Jump
            bool groundedPlayer = _characterController.isGrounded;
            
            if (groundedPlayer)
                _groundedTimer = 0.2f;
            if (_groundedTimer > 0)
                _groundedTimer -= Time.deltaTime;
 
            // slam into the ground
            if (groundedPlayer && _verticalVelocity < 0)
                _verticalVelocity = 0f;

            // apply gravity always, to let us track down ramps properly
            _verticalVelocity -= _fallSpeed * Time.deltaTime;

            // must have been grounded recently to allow jump
            if (_inputController.Input.Player.Jump.IsPressed() && _groundedTimer > 0)
            {
                // no more until we recontact ground
                _groundedTimer = 0;
 
                // Physics dynamics formula for calculating jump up velocity based on height and gravity
                _verticalVelocity += Mathf.Sqrt(_jumpHeight * 2 * _fallSpeed);
                Debug.Log("HJUMP!");
            }
            
            _characterController.Move(moveDirection * new Vector3(moveValue.x, _verticalVelocity * Time.deltaTime, moveValue.y));
        }
    }
}
