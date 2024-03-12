using UnityEngine;

namespace Resat.Models
{
    public class ScreenshotData
    {
        public RenderTexture RenderTexture;
        public OKHSLData OkhslData;

        public ScreenshotData(RenderTexture renderTexture, OKHSLData okhslData)
        {
            RenderTexture = renderTexture;
            OkhslData = okhslData;
        }
    }
}