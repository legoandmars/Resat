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
        public event Action<DialogueSO>? DialogueStarted;
        
        public void ChangeNpcFocus(NpcTriggerBehaviour? npcBehaviour)
        {
            NpcFocusChanged?.Invoke(npcBehaviour);
        }

        public void StartDialogue(DialogueSO dialogueSO)
        {
            DialogueStarted?.Invoke(dialogueSO);
        }
    }
}