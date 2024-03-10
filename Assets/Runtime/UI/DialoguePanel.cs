using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Resat.UI
{
    public class DialoguePanel : CornerPanel
    {
        [SerializeField]
        private TextMeshProUGUI? _text;

        public void SetText(string content)
        {
            if (_text == null)
                return;
            
            Debug.Log(content);
            // TODO: Interpolation
            _text.text = content;
        }

        public void SetActive(bool active)
        {
            /*if (_rectTransform == null)
                return;
            
            // TODO: Interpolation
            _rectTransform.gameObject.SetActive(active);*/
        }
        
        public async UniTask<bool> OpenWithText(string content, bool instant = false)
        {
            // disable all text
            SetText("");
            
            var success = await Open(instant);

            // TODO: Animate this
            if (success)
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
            }
            
            // do stuff
            return success;
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