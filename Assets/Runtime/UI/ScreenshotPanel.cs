using UnityEngine;
using UnityEngine.UI;

namespace Resat.UI
{
    public class ScreenshotPanel : MonoBehaviour
    {
        [SerializeField]
        private RawImage? _rawImage;

        public void SetTexture(RenderTexture renderTexture)
        {
            _rawImage.texture = renderTexture;
        }
    }
}