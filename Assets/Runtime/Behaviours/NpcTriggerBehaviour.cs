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
        public ExclamationPointBehaviour? ExclamationPoint => _exclamationPoint;
        
        [SerializeField]
        private NpcSO? _npcSO;

        [SerializeField]
        private ExclamationPointBehaviour? _exclamationPoint;

        [SerializeField]
        private SpriteRenderer? _spriteRenderer;
        
        [NonSerialized]
        public DialogueSO? CurrentDialogue;

        private bool _interactable = true;
        
        private void Awake()
        {
            CurrentDialogue = _npcSO?.InitialDialogue;

            if (_exclamationPoint != null)
            {
                _exclamationPoint.SetActive(false);
            }
        }
        
        public void DisableInteractions()
        {
            _interactable = false;
        }

        private void Update()
        {
            if (_spriteRenderer == null || _exclamationPoint?.PlayerCamera == null)
                return;
            
            _spriteRenderer.transform.forward = new Vector3(-_exclamationPoint.PlayerCamera.transform.forward.x, 0, -_exclamationPoint.PlayerCamera.transform.forward.z);
        }
    }
}