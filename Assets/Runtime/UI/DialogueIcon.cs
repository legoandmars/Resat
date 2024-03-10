using Resat.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Resat.UI
{
    // used for "continue" type thing
    // TODO: Tween
    public class DialogueIcon : MonoBehaviour
    {
        [SerializeField]
        private Image? _image;

        [SerializeField]
        private Color _iconColor = new(1,1,1,1);

        [SerializeField]
        private Sprite? _inDialogueIcon;

        [SerializeField]
        private Sprite? _dialoguePageFinishedIcon;

        [SerializeField]
        private Sprite? _dialogueFinishedIcon;

        public void SetIcon(DialogueIconType dialogueIconType)
        {
            if (_image == null)
                return;

            var sprite = dialogueIconType switch
            {
                DialogueIconType.InDialogue => _inDialogueIcon,
                DialogueIconType.DialoguePageFinished => _dialoguePageFinishedIcon,
                DialogueIconType.DialogueFinished => _dialogueFinishedIcon,
                _ => null
            };

            _image.sprite = sprite;
            _image.color = sprite != null ? _iconColor : Color.clear;
            
            if (dialogueIconType == DialogueIconType.InDialogue)
            {
                _image.sprite = _inDialogueIcon;
            }
            else if (dialogueIconType == DialogueIconType.DialoguePageFinished)
            {
                _image.sprite = _dialoguePageFinishedIcon;
            }
            else if (dialogueIconType == DialogueIconType.DialogueFinished)
            {
                _image.sprite = _dialogueFinishedIcon;
            }
        }
    }
}