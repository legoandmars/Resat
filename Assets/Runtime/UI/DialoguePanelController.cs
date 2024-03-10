using System.Threading;
using Cysharp.Threading.Tasks;
using Resat.Behaviours;
using Resat.Dialogue;
using Resat.Intermediates;
using Resat.Models;
using Resat.Models.Events;
using Resat.Npcs;
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

        [SerializeField]
        private DialogueIcon? _dialogueIcon;

        [Header("Delays, each one is additive; eg dialogue panel include name panel start delay")]
        [SerializeField]
        private float _dialoguePanelStartDelay = 0.5f;
        
        [SerializeField]
        private float _namePanelStartDelay = 1f;
        
        [SerializeField]
        private float _dialogueTextStartDelay = 0f;

        [Header("Text speeds")] 
        [SerializeField]
        private TextAnimationSpeed _dialogueTextSpeed;
        
        [SerializeField]
        private TextAnimationSpeed _nameTextSpeed;

        [SerializeField]
        private float _textCloseSpeedMultiplier = 2f;

        private DialogueSO? _currentDialogue;
        private NpcSO? _currentNpc; // used soley for the exit event
        private int _dialoguePage = 0;
        private CancellationTokenSource? _currentTokenSource;
        private bool _canContinueDialogue = false;
        
        // Disable dialogue panels on start
        private void Start()
        {
            if (_dialoguePanel != null)
                CloseWithText(_dialoguePanel).Forget();
            if (_namePanel != null)
                CloseWithText(_namePanel).Forget();
            if (_interactionPromptPanel != null)
                _interactionPromptPanel.Close(true).Forget();
        }
        
        private void OnEnable()
        {
            _npcIntermediate.NpcFocusChanged += OnNpcFocusChanged;
            _npcIntermediate.DialogueStarted += OnDialogueStarted;
            _npcIntermediate.DialogueContinueRequested += OnDialogueContinueRequested;
        }

        private void OnDialogueContinueRequested()
        {
            if (!_canContinueDialogue)
                return;
            
            if (_currentTokenSource == null)
            {
                // next page
                NextDialoguePage().Forget();
            }
            else
            {
                // skip text animation
                _currentTokenSource.Cancel();
            }
        }

        private void OnDisable()
        {
            _npcIntermediate.NpcFocusChanged -= OnNpcFocusChanged;
            _npcIntermediate.DialogueStarted -= OnDialogueStarted;
        }

        private async void OnDialogueStarted(DialogueStartedEvent dialogueStartedEvent)
        {
            // setup & clear stuff
            _currentDialogue = dialogueStartedEvent.Dialogue;
            _currentNpc = dialogueStartedEvent.Npc;
            _canContinueDialogue = false;
            _dialogueIcon?.SetIcon(DialogueIconType.InDialogue);
                
            UniTask<bool> promptPanelSuccess = _interactionPromptPanel?.Close() ?? UniTask.FromResult(true);
            await UniTask.WaitForSeconds(_dialoguePanelStartDelay);
            UniTask<bool> dialoguePanelSuccess = _dialoguePanel?.Open() ?? UniTask.FromResult(true);
            await UniTask.WaitForSeconds(_namePanelStartDelay);
            UniTask<bool> namePanelSuccess = _namePanel != null ? OpenWithText(_namePanel, dialogueStartedEvent.Npc.Name, _nameTextSpeed) : UniTask.FromResult(true);
            await UniTask.WaitForSeconds(_dialogueTextStartDelay);

            // add first page of dialogue
            _dialoguePage = 0;
            NextDialoguePage().Forget();
            _canContinueDialogue = true;
            
            bool success = await promptPanelSuccess && await namePanelSuccess && await dialoguePanelSuccess;
        }

        private async UniTask NextDialoguePage()
        {
            if (_currentDialogue == null || _dialoguePanel?.Text == null)
                return;

            var lastPage = _currentDialogue.Dialogue.Count - 1;
            if (_dialoguePage > lastPage)
            {
                // cancel out!
                await CloseDialogue();
                return;
            }
            
            // Setup variables for in-dialogue
            _currentTokenSource = new();
            _dialogueIcon?.SetIcon(DialogueIconType.InDialogue);
            var pageContent = _currentDialogue.Dialogue[_dialoguePage]!;
            
            await _textAnimationController.AnimateText(pageContent, _dialoguePanel.Text, _dialogueTextSpeed, _currentTokenSource.Token);

            // setup for next page
            _currentTokenSource = null;
            _dialoguePage++;
            _dialogueIcon?.SetIcon(_dialoguePage > lastPage ? DialogueIconType.DialogueFinished : DialogueIconType.DialoguePageFinished);
        }

        private async UniTask CloseDialogue()
        {
            if (_dialoguePanel == null || _namePanel == null || _dialoguePanel.Text == null || _namePanel.Text == null)
                return;
            
            bool showInteractPrompt = _currentDialogue?.ShowInteractPromptAfterDialogueComplete ?? false;
            DialogueStoppedEvent dialogueStoppedEvent = new(_currentDialogue!, _currentNpc!);
            
            // reset all variables
            _currentDialogue = null;
            _currentNpc = null;
            _dialoguePage = 0;
            _currentTokenSource = null;
            _canContinueDialogue = false;

            // kill text first so UI outro is consistent
            var dialogueTextUnanimate = _textAnimationController.UnanimateText(_dialoguePanel.Text.text, _dialoguePanel.Text, _dialogueTextSpeed * _textCloseSpeedMultiplier);
            var nameTextUnanimate = _textAnimationController.UnanimateText(_namePanel.Text.text, _namePanel.Text, _nameTextSpeed * _textCloseSpeedMultiplier);

            UniTask<bool> namePanelSuccess = _namePanel.Close();
            await UniTask.WaitForSeconds(_namePanelStartDelay);
            UniTask<bool> dialoguePanelSuccess = _dialoguePanel.Close();
            await UniTask.WaitForSeconds(_dialoguePanelStartDelay);

            UniTask<bool> promptPanelSuccess;
            if (!showInteractPrompt || _interactionPromptPanel == null)
                promptPanelSuccess = UniTask.FromResult(true);
            else
                promptPanelSuccess = _interactionPromptPanel.Open();

            bool success = await promptPanelSuccess && await namePanelSuccess && await dialoguePanelSuccess;
            
            // just in case text isn't somehow done
            await dialogueTextUnanimate;
            await nameTextUnanimate;

            _npcIntermediate.StopDialogue(dialogueStoppedEvent);
        }
        
        public async UniTask<bool> CloseWithText(DialoguePanel dialoguePanel, TextAnimationSpeed? textAnimationSpeed = null)
        {
            if (dialoguePanel.Text == null)
                return false;
            
            if (textAnimationSpeed == null)
            {
                // instant
                dialoguePanel.SetText("");
            }
            else
            {
                await _textAnimationController.UnanimateText(dialoguePanel.Text.text, dialoguePanel.Text, textAnimationSpeed.Value);
            }

            var success = await dialoguePanel.Close(textAnimationSpeed == null);

            return success;
        }

        public async UniTask<bool> OpenWithText(DialoguePanel dialoguePanel, string content, TextAnimationSpeed? textAnimationSpeed = null)
        {
            if (dialoguePanel.Text == null)
                return false;
            
            // disable all text
            dialoguePanel.SetText("");
            
            var success = await dialoguePanel.Open(textAnimationSpeed == null);

            if (success)
            {
                if (textAnimationSpeed == null)
                {
                    dialoguePanel.SetText(content);
                }
                else
                {
                    await _textAnimationController.AnimateText(content, dialoguePanel.Text, textAnimationSpeed.Value);
                }
            }
            
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