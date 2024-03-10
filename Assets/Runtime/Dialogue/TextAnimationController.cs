using System.Threading;
using Cysharp.Threading.Tasks;
using Resat.Models;
using TMPro;
using UnityEngine;

namespace Resat.Dialogue
{
    public class TextAnimationController : MonoBehaviour
    {
        /*[SerializeField]
        private float _typingSpeedMin;

        [SerializeField]
        private float _typingSpeedMax;
        */
        public async UniTask AnimateText(string content, TextMeshProUGUI text, TextAnimationSpeed textAnimationSpeed, CancellationToken cancellationToken = default)
        {
            string currentText = "";
            // should probably make this use  a stringbuilder or something
            for (int i = 0; i < content.Length; i++)
            {
                await UniTask.WaitForSeconds(textAnimationSpeed.Sample());
                currentText = currentText + content[i];
                text.SetText(currentText);

                if (cancellationToken.IsCancellationRequested)
                {
                    text.SetText(content);
                    break;
                }
            }
        }
        
        public async UniTask UnanimateText(string content, TextMeshProUGUI text, TextAnimationSpeed textAnimationSpeed, CancellationToken cancellationToken = default)
        {
            string currentText = content;
            // should probably make this use  a stringbuilder or something
            for (int i = 0; i < content.Length; i++)
            {
                await UniTask.WaitForSeconds(textAnimationSpeed.Sample());
                currentText = currentText.Substring(0, content.Length - 1 - i);
                text.SetText(currentText);

                if (cancellationToken.IsCancellationRequested)
                {
                    text.SetText("");
                    break;
                }
            }
        }
    }
}