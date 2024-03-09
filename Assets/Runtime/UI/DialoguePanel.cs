using TMPro;
using UnityEngine;

namespace Resat.UI
{
    public class DialoguePanel : CornerPanel
    {
        [SerializeField]
        private RectTransform? _rectTransform;

        [SerializeField]
        private TextMeshProUGUI? _text;

        public void SetText(string content)
        {
            if (_text == null)
                return;
            
            // TODO: Interpolation
            _text.text = content;
        }

        public void SetActive(bool active)
        {
            if (_rectTransform == null)
                return;
            
            // TODO: Interpolation
            _rectTransform.gameObject.SetActive(active);
        }
    }
}