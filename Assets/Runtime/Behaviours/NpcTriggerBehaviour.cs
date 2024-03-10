using System;
using Resat.Dialogue;
using Resat.Npcs;
using UnityEngine;

namespace Resat.Behaviours
{
    // essentially just a dummy
    public class NpcTriggerBehaviour : MonoBehaviour
    {
        public bool Interactable => _interactable;
        public NpcSO? NpcSO => _npcSO;
        
        [SerializeField]
        private NpcSO? _npcSO;

        [NonSerialized]
        public DialogueSO? CurrentDialogue;

        private bool _interactable = true;
        
        private void Awake()
        {
            CurrentDialogue = _npcSO?.InitialDialogue;
        }
        
        public void DisableInteractions()
        {
            _interactable = false;
        }
    }
}