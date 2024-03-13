using System.Collections.Generic;
using System.Linq;
using Input;
using Resat.Behaviours;
using Resat.Input;
using Resat.Intermediates;
using Resat.Models.Events;
using Resat.Npcs;
using Resat.Quests;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Resat.Dialogue
{
    // responsible for actually handling/starting dialogue, and doing dialogue input
    // also tracks what NPCs are giving which dialogue
    public class DialogueController : MonoBehaviour, ResatInput.IDialogueActions
    {
        [SerializeField]
        private InputController _inputController = null!;

        [SerializeField]
        private QuestController _questController = null!;

        [SerializeField]
        private NpcIntermediate _npcIntermediate = null!;

        // hardcoded
        [SerializeField]
        private List<NpcTriggerBehaviour> _skateboardNpcs = new();
        [SerializeField]
        private List<NpcTriggerBehaviour> _logNpcs = new();
        [SerializeField]
        private List<NpcTriggerBehaviour> _carNpcs = new();

        private NpcTriggerBehaviour? _npcBehaviour;
        private bool _inDialogue = false;
        private bool _forceDisableDialogue = false;

        private void Start()
        {
            _inputController.EnableDialogueInput();
        }
        
        private void OnEnable()
        {
            _npcIntermediate.NpcFocusChanged += OnNpcFocusChanged;
            _npcIntermediate.DialogueStopped += OnDialogueStopped;
            _inputController.Input.Dialogue.AddCallbacks(this);
        }

        private void OnDisable()
        {
            _npcIntermediate.NpcFocusChanged -= OnNpcFocusChanged;
            _npcIntermediate.DialogueStopped -= OnDialogueStopped;
            _inputController.Input.Dialogue.RemoveCallbacks(this);
        }

        public void ToggleDialogue(bool state)
        {
            _forceDisableDialogue = !state;
            _npcIntermediate.ToggleDialogueAbility(state);
        }
        
        private void OnNpcFocusChanged(NpcTriggerBehaviour? npcBehaviour)
        {
            if (_forceDisableDialogue)
                return;

            _npcBehaviour = npcBehaviour;
            
            Debug.Log("Focus changed!");
        }
        
        private void OnDialogueStopped(DialogueStoppedEvent dialogueStoppedEvent)
        {
            Debug.Log("Stopping dialogue...");
            _inDialogue = false;
         
            // Attempt to advance dialogue to next stage
            AdvanceDialogueAfterFinished();
            
            // Enable input
            _inputController.EnablePlayerInput();
            _inputController.EnableCameraInput();
            
            // Tell the NPC we've just ended the dialogue
            if (_npcBehaviour != null)
            {
                _npcBehaviour.OnDialogueAnimationEnded();
            }
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            if (_forceDisableDialogue)
                return;
            
            // on NPC interact
            if (!context.performed)
                return;

            if (!_inputController.DialogueInputEnabled || _inDialogue || _npcBehaviour == null)
                return;
            
            // we are in dialogue, disable/enable everything as needed
            BeginDialogue();
        }

        public void OnContinue(InputAction.CallbackContext context)
        {
            if (!context.performed || !_inDialogue)
                return;
            
            _npcIntermediate.RequestDialogueContinue();
        }

        private void BeginDialogue()
        {
            var npc = _npcBehaviour?.NpcSO;
            var dialogue = GetDialogueFromNpc(_npcBehaviour);

            if (npc == null || dialogue == null || _inDialogue)
                return;
            
            Debug.Log("Starting dialogue...");
            _inDialogue = true;

            // Disable input
            _inputController.DisablePlayerInput();
            _inputController.DisableCameraInput();
            
            _npcIntermediate.StartDialogue(new (dialogue, npc));
            
            // Tell the NPC we've just started the dialogue
            if (_npcBehaviour != null)
            {
                _npcBehaviour.OnDialogueAnimationStarted();
            }
        }

        private void AdvanceDialogueAfterFinished()
        {
            var dialogue = _npcBehaviour?.CurrentDialogue;

            if (_npcBehaviour == null || dialogue == null)
                return;

            if (dialogue.StopShowingDialogueAfterwards)
            {
                _npcBehaviour.DisableInteractions();
                
                // very hardcoded skateboard...
                if (_skateboardNpcs.Any(x => x == _npcBehaviour))
                {
                    foreach (var skateboardNpc in _skateboardNpcs)
                    {
                        if (skateboardNpc.Interactable && skateboardNpc.CurrentDialogue?.NextDialogue != null)
                        {
                            skateboardNpc.CurrentDialogue = skateboardNpc.CurrentDialogue.NextDialogue;
                        }
                    }
                }
                // very hardcoded log...
                if (_logNpcs.Any(x => x == _npcBehaviour))
                {
                    foreach (var logNpc in _logNpcs)
                    {
                        if (logNpc.Interactable && logNpc.CurrentDialogue?.NextDialogue != null)
                        {
                            logNpc.CurrentDialogue = logNpc.CurrentDialogue.NextDialogue;
                        }
                    }
                }
                // very hardcoded car...
                if (_carNpcs.Any(x => x == _npcBehaviour))
                {
                    foreach (var carNpc in _carNpcs)
                    {
                        if (carNpc.Interactable && carNpc.CurrentDialogue?.NextDialogue != null)
                        {
                            carNpc.CurrentDialogue = carNpc.CurrentDialogue.NextDialogue;
                        }
                    }
                }
                
                return;
            }
            
            if (dialogue.QuestRequirementToPass == null && dialogue.NextDialogue)
            {
                // easy street
                _npcBehaviour.CurrentDialogue = dialogue.NextDialogue;
            }
        }
        
        private DialogueSO? GetDialogueFromNpc(NpcTriggerBehaviour? npcBehaviour)
        {
            if (npcBehaviour == null)
                return null;

            // advance through quests dialogues as necessary
            // TODO: This will break with consecutive quests and should be recurisve. I do not forsee this being a problem as-is so i will spend my time elsewhere
            if (npcBehaviour.CurrentDialogue?.QuestRequirementToPass != null && npcBehaviour.CurrentDialogue.NextDialogue != null)
            {
                if (_questController.AllQuestItemsCollected(npcBehaviour.CurrentDialogue.QuestRequirementToPass))
                {
                    npcBehaviour.CurrentDialogue = npcBehaviour.CurrentDialogue.NextDialogue;
                }
            }
            
            return npcBehaviour.CurrentDialogue;
        }
    }
}