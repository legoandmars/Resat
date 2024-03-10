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

        [Header("Delays, each one is additive; eg dialogue panel include name panel start delay")]
        [SerializeField]
        private float _dialoguePanelStartDelay = 0.5f;
        
        [SerializeField]
        private float _namePanelStartDelay = 1f;

        private DialogueSO? _currentDialogue;

        // Disable dialogue panels on start
        private void Start()
        {
            _dialoguePanel?.CloseWithText("", true);
            _namePanel?.CloseWithText("", true);
            _interactionPromptPanel?.Close(true);
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

            UniTask<bool> promptPanelSuccess = _interactionPromptPanel?.Close() ?? UniTask.FromResult(true);
            await UniTask.WaitForSeconds(_dialoguePanelStartDelay);
            UniTask<bool> dialoguePanelSuccess = _dialoguePanel?.Open() ?? UniTask.FromResult(true);
            await UniTask.WaitForSeconds(_namePanelStartDelay);
            UniTask<bool> namePanelSuccess = _namePanel?.OpenWithText(dialogueSO.name) ?? UniTask.FromResult(true);

            bool success = await promptPanelSuccess && await namePanelSuccess && await dialoguePanelSuccess;

            // add first page of dialogue
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