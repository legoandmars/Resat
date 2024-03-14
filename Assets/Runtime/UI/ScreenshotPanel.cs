using Resat.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Resat.UI
{
    public class ScreenshotPanel : MonoBehaviour
    {
        public ScreenshotData? ScreenshotData => _screenshotData;
        
        [SerializeField]
        private RawImage? _rawImage;

        private ScreenshotData? _screenshotData;
        
        public void SetTexture(RenderTexture renderTexture)
        {
            if (_rawImage == null)
                return;
            
            _rawImage.texture = renderTexture;
            _rawImage.color = Color.white;
        }

        public void SetScreenshotData(ScreenshotData screenshotData)
        {
            _screenshotData = screenshotData;
            SetTexture(_screenshotData.RenderTexture);
        }

        public void ResetValues()
        {
            if (_rawImage == null)
                return;
            
            _screenshotData = null;
            _rawImage.texture = null;
            _rawImage.color = Color.gray; // TODO : BETTER DISABLE
        }
    }
}