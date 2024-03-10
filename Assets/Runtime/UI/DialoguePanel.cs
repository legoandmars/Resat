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
        
        public async UniTask<bool> OpenWithText(string content)
        {
            // disable all text
            SetText("");
            
            var success = await Open();

            // TODO: Animate this
            if (success)
            {
                SetText(content);
            }
            
            // do stuff
            return success;
        }
        
        public async UniTask<bool> CloseWithText(string content = "")
        {
            // TODO: Animate this
            SetText(content);

            var success = await Close();
            
            // do stuff
            return success;
        }

    }
}