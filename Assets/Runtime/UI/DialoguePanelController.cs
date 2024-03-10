using Cysharp.Threading.Tasks;
using Resat.Behaviours;
using Resat.Dialogue;
using Resat.Intermediates;
using Resat.Models.Events;
using TMPro;
using UnityEngine;

namespace Resat.UI
{
    public class DialoguePanelController : MonoBehaviour
    {
        [SerializeField]
        private TextAnimationController _textAnimationController = null!;

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
        
        [SerializeField]
        private float _dialogueTextStartDelay = 0f;

        private DialogueSO? _currentDialogue;
        private int _dialoguePage = 0;
        
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

        private async void OnDialogueStarted(DialogueStartedEvent dialogueStartedEvent)
        {
            Debug.Log("Oh no!");
            _currentDialogue = dialogueStartedEvent.Dialogue;

            UniTask<bool> promptPanelSuccess = _interactionPromptPanel?.Close() ?? UniTask.FromResult(true);
            await UniTask.WaitForSeconds(_dialoguePanelStartDelay);
            UniTask<bool> dialoguePanelSuccess = _dialoguePanel?.Open() ?? UniTask.FromResult(true);
            await UniTask.WaitForSeconds(_namePanelStartDelay);
            UniTask<bool> namePanelSuccess = _namePanel != null ? OpenWithText(_namePanel, dialogueStartedEvent.Npc.Name) : UniTask.FromResult(true);
            await UniTask.WaitForSeconds(_dialogueTextStartDelay);

            // add first page of dialogue
            _dialoguePage = 0;
            NextDialoguePage().Forget();

            bool success = await promptPanelSuccess && await namePanelSuccess && await dialoguePanelSuccess;
        }

        private async UniTask NextDialoguePage()
        {
            if (_currentDialogue == null || _dialoguePanel?.Text == null)
                return;
            
            var lastPage = _currentDialogue.Dialogue.Count - 1;
            if (_dialoguePage == lastPage)
            {
                // cancel out!
                return;
            }

            var pageContent = _currentDialogue.Dialogue[_dialoguePage]!;
            
            await _textAnimationController.AnimateText(pageContent, _dialoguePanel.Text);
            
            // setup for next page
        }
        
        public async UniTask<bool> OpenWithText(DialoguePanel dialoguePanel, string content, bool instant = false)
        {
            if (dialoguePanel.Text == null)
                return false;
            
            // disable all text
            dialoguePanel.SetText("");
            
            var success = await dialoguePanel.Open(instant);

            // TODO: Animate this
            if (success)
            {
                if (instant)
                {
                    dialoguePanel.SetText(content);
                }
                else
                {
                    // TODO: Animate this
                    await _textAnimationController.AnimateText(content, dialoguePanel.Text);
                }
            }
            
            // do stuff
            return success;
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