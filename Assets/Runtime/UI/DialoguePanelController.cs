using Cysharp.Threading.Tasks;
using Resat.Behaviours;
using Resat.Dialogue;
using Resat.Intermediates;
using TMPro;
using UnityEngine;

namespace Resat.UI
{
    public class DialoguePanelController : MonoBehaviour
    {
        [SerializeField]
        private NpcIntermediate _npcIntermediate = null!;
        
        [SerializeField]
        private DialoguePanel? _dialoguePanel;
        
        [SerializeField]
        private DialoguePanel? _namePanel;

        [SerializeField]
        private CornerPanel? _interactionPromptPanel;

        private DialogueSO? _currentDialogue;

        // Disable dialogue panels on start
        private void Start()
        {
            _dialoguePanel?.Close();
            _namePanel?.CloseWithText("");
            _interactionPromptPanel?.Close();
        }
        
        private void OnEnable()
        {
            _npcIntermediate.NpcFocusChanged += OnNpcFocusChanged;
            _npcIntermediate.DialogueStarted += OnDialogueStarted;
        }

        private void OnDisable()
        {
            _npcIntermediate.NpcFocusChanged -= OnNpcFocusChanged;
            _npcIntermediate.DialogueStarted -= OnDialogueStarted;
        }

        private async void OnDialogueStarted(DialogueSO dialogueSO)
        {
            Debug.Log("Oh no!");
            _currentDialogue = dialogueSO;
            
            // Setup UI
            if (_interactionPromptPanel != null)
                await _interactionPromptPanel.Close();

            if (_namePanel != null)
            {
                await _namePanel.OpenWithText("Cowboy carl");
            }
        }

        private async void OnNpcFocusChanged(NpcTriggerBehaviour? npcBehaviour)
        {
            // do other stuff here if necessary, before any panel stuff
            
            if (_interactionPromptPanel == null)
                return;

            if (npcBehaviour == null)
                await _interactionPromptPanel.Close();
            else
                await _interactionPromptPanel.Open();
        }
    }
}