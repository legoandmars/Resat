using System.Collections.Generic;
using Resat.Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Resat.UI
{
    public class CameraDebugPanel : MonoBehaviour
    {
        // Debug options
        [SerializeField]
        public RawImage? CameraImage;
        
        [SerializeField]
        public RawImage? ResizedCameraImage;

        [SerializeField]
        public RawImage? ArrayTextureImage;
        
        [SerializeField]
        public TextMeshProUGUI? TotalColorCountText;
        
        [SerializeField]
        public TextMeshProUGUI? TotalColorCoveragePercentText;

        [SerializeField]
        public TextMeshProUGUI? PhotoVibeText;

        [SerializeField]
        public List<RawImage>? TopColorImages;

        [SerializeField]
        public bool ResizeCameraImage = false;
        
        public void SetData(OKHSLData okhslData)
        {
            for (int i = 0; i < TopColorImages?.Count && i < okhslData.TopColors.Length; i++)
            {
                var topColor = okhslData.TopColors[i];
                var topColorImage = TopColorImages[i];
                topColorImage.color = topColor.Color;
            }
            
            if (TotalColorCountText != null)
                TotalColorCountText.text = $"Total colors: {okhslData.TotalColorCount}";
            if (TotalColorCoveragePercentText != null)
                TotalColorCoveragePercentText.text = $"Total color coverage: {okhslData.TotalColorCoveragePercent:0.##}%";
            if (PhotoVibeText != null)
                PhotoVibeText.text = $"Photo vibes: N/A"; // TODO
        }
        
        public void SetPreviewTexture(RenderTexture renderTexture)
        {
            if (CameraImage == null)
                return;
            
            CameraImage.texture = renderTexture;

            if (ResizeCameraImage && ResizedCameraImage != null)
                ResizedCameraImage.texture = renderTexture;
        }
        
        public void SetArrayTexture(RenderTexture renderTexture)
        {
            if (ArrayTextureImage == null)
                return;
            
            ArrayTextureImage.texture = renderTexture;
        }

        public void SetPreviewTextureResolution(CameraResolutionData resolutionData, CanvasScaler canvasScaler)
        {
            if (!ResizeCameraImage && ResizedCameraImage != null)
                ResizedCameraImage.gameObject.SetActive(false);
            
            if (ResizedCameraImage == null || !ResizeCameraImage)
                return;

            ResizedCameraImage.gameObject.SetActive(true);
            
            // get "difference" between 1080p canvas scaler
            var resolution = resolutionData.GetRescaledResolution(canvasScaler.referenceResolution);
            var nativeResolution = resolutionData.NativeResolution / resolutionData.GetNativeResolutionScale(canvasScaler.referenceResolution);
            
            ResizedCameraImage.rectTransform.sizeDelta = resolution;

            float xOffset = (resolutionData.Center.x - 0.5f) * nativeResolution.x;
            float yOffset = (resolutionData.Center.y - 0.5f) * nativeResolution.y;

            ResizedCameraImage.rectTransform.anchoredPosition = new Vector2(xOffset, yOffset);

        }
    }
}