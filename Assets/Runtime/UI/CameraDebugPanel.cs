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
        public RawImage? ArrayTextureImage;
        
        [SerializeField]
        public TextMeshProUGUI? TotalColorCountText;
        
        [SerializeField]
        public TextMeshProUGUI? TotalColorCoveragePercentText;

        [SerializeField]
        public TextMeshProUGUI? PhotoVibeText;

        [SerializeField]
        public List<RawImage>? TopColorImages;

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
    }
}