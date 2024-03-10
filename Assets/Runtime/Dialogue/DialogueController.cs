using Input;
using Resat.Behaviours;
using Resat.Input;
using Resat.Intermediates;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Resat.Dialogue
{
    // responsible for actually handling/starting dialogue, and doing dialogue input
    public class DialogueController : MonoBehaviour, ResatInput.IDialogueActions
    {
        [SerializeField]
        private InputController _inputController = null!;

        [SerializeField]
        private NpcIntermediate _npcIntermediate = null!;

        private NpcTriggerBehaviour? _npcBehaviour;
        private bool _inDialogue = false;
        
        private void Start()
        {
            _inputController.EnableDialogueInput();
        }
        
        private void OnEnable()
        {
            _npcIntermediate.NpcFocusChanged += OnNpcFocusChanged;
            _inputController.Input.Dialogue.AddCallbacks(this);
        }

        private void OnDisable()
        {
            _npcIntermediate.NpcFocusChanged -= OnNpcFocusChanged;
            _inputController.Input.Dialogue.RemoveCallbacks(this);
        }

        private void OnNpcFocusChanged(NpcTriggerBehaviour? npcBehaviour)
        {
            _npcBehaviour = npcBehaviour;
            
            Debug.Log("Focus changed!");
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            // on NPC interact
            if (!context.performed)
                return;

            if (!_inputController.DialogueInputEnabled || _inDialogue || _npcBehaviour == null)
                return;
            
            // we are in dialogue, disable/enable everything as needed
            BeginDialogue();
        }

        private void BeginDialogue()
        {
            var npc = _npcBehaviour?.NpcSO;
            var dialogue = npc?.Dialogue;

            if (npc == null || dialogue == null)
                return;
            
            Debug.Log("Starting dialogue...");
            _inDialogue = true;

            // Disable input
            _inputController.DisablePlayerInput();
            _inputController.DisableCameraInput();
            
            _npcIntermediate.StartDialogue(new (dialogue, npc));
        }
    }
}