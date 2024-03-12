using System.Collections.Generic;
using Resat.Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Resat.UI
{
    public class CameraPreviewPanel : MonoBehaviour
    {
        [SerializeField]
        public TextMeshProUGUI? NewColorCountText;
        
        [SerializeField]
        public TextMeshProUGUI? NewColorCountPercentageText;

        [SerializeField]
        public List<Image>? TopColorImages;

        public void SetData(OKHSLData okhslData)
        {
            for (int i = 0; i < TopColorImages?.Count && i < okhslData.TopColors.Length; i++)
            {
                var topColor = okhslData.TopColors[i];
                var topColorImage = TopColorImages[i];
                topColorImage.color = topColor.Color;
            }
            
            if (NewColorCountText != null)
                NewColorCountText.text = $"New Colors: {okhslData.NewColorCount}";
            if (NewColorCountPercentageText != null)
                NewColorCountPercentageText.text = $"- {okhslData.NewColorCoveragePercent:00.0}%";
        }

    }
}