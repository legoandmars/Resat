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
    }
}