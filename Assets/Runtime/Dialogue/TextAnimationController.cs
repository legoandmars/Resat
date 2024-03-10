using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Resat.Dialogue
{
    public class TextAnimationController : MonoBehaviour
    {
        [SerializeField]
        private float _typingSpeedMin;

        [SerializeField]
        private float _typingSpeedMax;

        public async UniTask AnimateText(string content, TextMeshProUGUI text, CancellationToken cancellationToken = default)
        {
            string currentText = "";
            // should probably make this use  a stringbuilder or something
            for (int i = 0; i < content.Length; i++)
            {
                await UniTask.WaitForSeconds(Random.Range(_typingSpeedMin, _typingSpeedMax));
                currentText = currentText + content[i];
                text.SetText(currentText);

                if (cancellationToken.IsCancellationRequested)
                {
                    text.SetText(content);
                    break;
                }
            }
        }
    }
}