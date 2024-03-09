using System.Collections;
using System.Collections.Generic;
using Input;
using Resat.Behaviours;
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
        private Transform _raycastEmittingObject = null!;
        
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

        [SerializeField]
        private LayerMask _npcLayerMask;

        private float _verticalVelocity;
        private float _groundedTimer;        // to allow jumping when going down ramps
        private RaycastHit[] _raycastResults = new RaycastHit[1]; // no reason we would ever need to cast for more than one NPC
        private NpcTriggerBehaviour? _focusedNpc = null;
        
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
            }
            
            _characterController.Move(moveDirection * new Vector3(moveValue.x, _verticalVelocity * Time.deltaTime, moveValue.y));

            NpcInteractionCheck();
        }

        private void NpcInteractionCheck()
        {
            // TODO: Add behaviour on NPC focus
            var hits = Physics.RaycastNonAlloc(_raycastEmittingObject.position, _raycastEmittingObject.forward, _raycastResults, Mathf.Infinity, _npcLayerMask);
            
            if (hits == 0)
            {
                _focusedNpc = null;
            }
            for (int i = 0; i < hits; i++)
            {
                _focusedNpc = _raycastResults[0].collider.gameObject.GetComponent<NpcTriggerBehaviour>();
            }
        }
    }
}
