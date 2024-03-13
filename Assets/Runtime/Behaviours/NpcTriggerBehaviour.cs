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
        public SpriteRenderer? SpriteRenderer => _spriteRenderer;
        
        [SerializeField]
        private NpcSO? _npcSO;

        [SerializeField]
        private ExclamationPointBehaviour? _exclamationPoint;

        [SerializeField]
        private SpriteRenderer? _spriteRenderer;

        [SerializeField]
        private ScribbleBehaviour? _scribbleBehaviour;

        [SerializeField]
        public DialogueSO? _describbleDialogue;
        
        [NonSerialized]
        public DialogueSO? CurrentDialogue;

        private bool _interactable = true;
        private bool _describbled = false;
        
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
            
            var vector = new Vector3(-_exclamationPoint.PlayerCamera.transform.forward.x, 0, -_exclamationPoint.PlayerCamera.transform.forward.z);
            if (vector == Vector3.zero)
                return;
            
            _spriteRenderer.transform.forward = vector;
        }

        public virtual void OnDialogueAnimationStarted()
        {
            if (_describbleDialogue == null || _describbleDialogue != CurrentDialogue || _describbled)
                return;
            
            Debug.Log("It's describblin' time.");
            _describbled = true;

            if (_scribbleBehaviour != null)
            {
                _scribbleBehaviour.Describble();
            }
        } 

        public virtual void OnDialogueAnimationEnded()
        {
            
        } 
    }
}