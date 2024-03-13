using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Input;
using Resat.Behaviours;
using Resat.Input;
using Resat.Intermediates;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Resat.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField]
        private InputController _inputController = null!;

        [SerializeField]
        private NpcIntermediate _npcIntermediate = null!;

        [SerializeField]
        private CharacterController _characterController = null!;

        [SerializeField]
        private Transform _raycastEmittingObject = null!;

        [SerializeField]
        private Collider _boxcastCollider = null!;

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
        private float _slideFriction = 0f;

        [SerializeField]
        private float _npcInteractionDistance = 1f;

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

        private void OnEnable()
        {
            _npcIntermediate.DialogueAbilityToggled += DialogueAbilityToggled;
        }

        private void OnDisable()
        {
            _npcIntermediate.DialogueAbilityToggled -= DialogueAbilityToggled;
        }
        
        // hack to just re-send the npc when our ability to talk gets enabled
        private async void DialogueAbilityToggled(bool state)
        {
            Debug.Log("wao");
            Debug.Log(state);
            Debug.Log(_focusedNpc);
            
            if (state)
            {
                // yeeaah it's that point in the jam
                await UniTask.WaitForSeconds(.1f);
                _npcIntermediate.ChangeNpcFocus(_focusedNpc);
            }
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
            Vector3? groundNormal = GetGroundNormal();

            bool groundMeetsAngle = GetAngleFromNormal(groundNormal) < _characterController.slopeLimit;

            if (groundedPlayer && groundMeetsAngle)
                _groundedTimer = 0.2f;
            if (_groundedTimer > 0)
                _groundedTimer -= Time.deltaTime;
 
            // prepare slide movement
            Vector3 slideMovement = Vector3.zero;
            _ = _slideFriction; // ignore warning
            
            // We're grounded, but should be sliding down a slope
            /*if (groundedPlayer && !groundMeetsAngle && groundNormal != null)
            {
                slideMovement.x = ((1f - groundNormal.Value.y) * groundNormal.Value.x) * _slideFriction;
                slideMovement.z = ((1f - groundNormal.Value.y) * groundNormal.Value.z) * _slideFriction;
                slideMovement *= Time.deltaTime;
            }*/
            
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
            
            _characterController.Move((moveDirection * new Vector3(moveValue.x, _verticalVelocity * Time.deltaTime, moveValue.y)) + slideMovement);

            NpcInteractionCheck();
        }

        private Vector3? GetGroundNormal()
        {
            var hits = Physics.BoxCastNonAlloc(_boxcastCollider.bounds.center, _boxcastCollider.bounds.size, Vector2.down, _raycastResults);
            if (hits == 0)
                return null;

            return _raycastResults[0].normal;
            // return Vector3.Angle(_raycastResults[0].normal, Vector3.up);
        }

        private float GetAngleFromNormal(Vector3? normal)
        {
            if (normal == null) 
                return 180;
            
            return Vector3.Angle(normal.Value, Vector3.up);
        }
        
        private void NpcInteractionCheck()
        {
            // TODO: Add behaviour on NPC focus
            var hits = Physics.RaycastNonAlloc(_raycastEmittingObject.position, _raycastEmittingObject.forward, _raycastResults, Mathf.Infinity, _npcLayerMask);
            
            if (hits == 0 || _raycastResults[0].distance > _npcInteractionDistance)
            {
                TrySetNpc(null);
            }
            else if (_focusedNpc == null)
            {
                var triggerBehaviour = _raycastResults[0].collider.gameObject.GetComponent<NpcTriggerBehaviour>();
                if (triggerBehaviour == null || !triggerBehaviour.Interactable)
                    TrySetNpc(null);
                else
                    TrySetNpc(triggerBehaviour);
            }
            // do nothing if we have an already focused NPC, and one's in view. this will almost certainly be the same NPC, and we don't need to run the GetComponent call again.
        }

        private void TrySetNpc(NpcTriggerBehaviour? focusedNpc)
        {
            if (_focusedNpc == focusedNpc)
                return;

            _focusedNpc = focusedNpc;
            _npcIntermediate.ChangeNpcFocus(_focusedNpc);
        }
    }
}
