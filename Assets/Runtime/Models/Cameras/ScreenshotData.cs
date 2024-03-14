using UnityEngine;

namespace Resat.Models
{
    public class ScreenshotData
    {
        public RenderTexture RenderTexture;
        public RenderTexture PreviewRenderTexture;
        public OKHSLData OkhslData;

        public ScreenshotData(RenderTexture renderTexture, RenderTexture previewRenderTexture, OKHSLData okhslData)
        {
            RenderTexture = renderTexture;
            PreviewRenderTexture = previewRenderTexture;
            OkhslData = okhslData;
        }
    }
}