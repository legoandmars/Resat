using System;
using Resat.Behaviours;
using Resat.Dialogue;
using Resat.Models;
using Resat.Models.Events;
using UnityEngine;

namespace Resat.Intermediates
{
    public class NpcIntermediate : MonoBehaviour
    {
        public event Action<NpcTriggerBehaviour?>? NpcFocusChanged;
        public event Action<DialogueStartedEvent>? DialogueStarted;
        public event Action<DialogueStoppedEvent>? DialogueStopped;
        public event Action<bool>? DialogueAbilityToggled;
        public event Action? DialogueContinueRequested;
        
        public void ChangeNpcFocus(NpcTriggerBehaviour? npcBehaviour)
        {
            NpcFocusChanged?.Invoke(npcBehaviour);
        }

        public void StartDialogue(DialogueStartedEvent dialogueStartedEvent)
        {
            DialogueStarted?.Invoke(dialogueStartedEvent);
        }
        
        public void StopDialogue(DialogueStoppedEvent dialogueStoppedEvent)
        {
            DialogueStopped?.Invoke(dialogueStoppedEvent);
        }

        public void RequestDialogueContinue()
        {
            DialogueContinueRequested?.Invoke();
        }

        public void ToggleDialogueAbility(bool state)
        {
            DialogueAbilityToggled?.Invoke(state);
        }
    }
}