using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Resat.UI
{
    public class DialoguePanel : CornerPanel
    {
        public TextMeshProUGUI? Text => _text;
        
        [SerializeField]
        private TextMeshProUGUI? _text;

        public void SetText(string content)
        {
            if (_text == null)
                return;
            
            // TODO: Interpolation
            _text.text = content;
        }

        public async UniTask<bool> CloseWithText(string content = "", bool instant = false)
        {
            if (instant)
            {
                SetText(content);
            }
            else
            {
                // TODO: Animate this
                SetText(content);
            }

            var success = await Close(instant);
            
            // do stuff
            return success;
        }

    }
}