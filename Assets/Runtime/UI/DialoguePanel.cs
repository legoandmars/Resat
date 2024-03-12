using System;
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

        public async UniTask<bool> CloseWithTextScaling(bool instant = false, Action<float>? setFloat = null, Action<Vector2>? setVector = null)
        {
            return await Close(instant, setFloat, (value) =>
            {
                var scale = new Vector3(value.y / _tweenSettings.Size.y, value.x / _tweenSettings.Size.x, 1);
                if (_text != null)
                {
                    _text.transform.localScale = scale;
                }
                setVector?.Invoke(value);
            });
        }
        
        public async UniTask<bool> OpenWithTextScaling(bool instant = false, Action<float>? setFloat = null, Action<Vector2>? setVector = null)
        {
            return await Open(instant, setFloat, (value) =>
            {
                var scale = new Vector3(value.x / _tweenSettings.Size.x, value.y / _tweenSettings.Size.y, 1);
                if (_text != null)
                {
                    _text.transform.localScale = scale;
                }
                setVector?.Invoke(value);
            });
        }
    }
}