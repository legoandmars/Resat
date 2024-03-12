using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Resat.Dialogue;
using Resat.Models;
using TMPro;
using UnityEngine;

namespace Resat.UI
{
    public class DialoguePanel : CornerPanel
    {
        public TextMeshProUGUI? Text => _text;
        
        [SerializeField]
        private TextMeshProUGUI? _text;

        [SerializeField]
        private TextAnimationController _textAnimationController = null!;

        // only used for OpenWithText
        [SerializeField]
        private TextAudioSO? _textAudio;
        
        public void SetText(string content)
        {
            if (_text == null)
                return;
            
            // TODO: Interpolation
            _text.text = content;
        }

        public async UniTask<bool> CloseWithText(string content, TextAnimationSpeed speed, bool instant = false, TextAudioSO? textAudio = null, Action<float>? setFloat = null, Action<Vector2>? setVector = null, CancellationToken cancellationToken = default)
        {
            if (Text != null)
            {
                if (!instant)
                {
                    await _textAnimationController.UnanimateTextSmoothCancel(content, Text, speed, cancellationToken);
                }
                else
                {
                    Text.SetText("");
                }
            }

            return await Close(instant, setFloat, setVector);
        }
        
        public async UniTask<bool> OpenWithText(string content, TextAnimationSpeed speed, bool instant = false, TextAudioSO? textAudio = null, Action<float>? setFloat = null, Action<Vector2>? setVector = null, CancellationToken cancellationToken = default)
        {
            var result = Open(instant, setFloat, setVector);
            await UniTask.WaitForSeconds(_tweenSettings.Duration - 0.4f);
            
            if (Text != null)
            {
                if (!instant)
                {
                    await _textAnimationController.AnimateTextSmoothCancel(content, Text, speed, textAudio ?? _textAudio, cancellationToken);
                }
                else
                {
                    Text.SetText(content);
                }
            }

            return await result;
        }
    }
}