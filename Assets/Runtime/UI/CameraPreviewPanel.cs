using System;
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

        [SerializeField]
        public TextureDebugPanel? TextureDebugPanel;
        
        public void SetData(OKHSLData okhslData)
        {
            for (int i = 0; i < TopColorImages?.Count && i < okhslData.TopColors.Length; i++)
            {
                var topColor = okhslData.TopColors[i];
                var topColorImage = TopColorImages[i];
                topColorImage.color = topColor.Color;
            }

            var percent = Single.IsNaN(okhslData.NewColorCoveragePercent) ? 0.0f : okhslData.NewColorCoveragePercent;
            if (NewColorCountText != null)
                NewColorCountText.text = $"New Colors: {okhslData.NewColorCount}";
            if (NewColorCountPercentageText != null)
                NewColorCountPercentageText.text = $"{percent:00.0}%";
        }

        public void SetPreviewTexture(RenderTexture renderTexture)
        {
            if (TextureDebugPanel != null)
            {
                TextureDebugPanel.SetPreviewTexture(renderTexture);
            }
        }
        
        public void SetAnimationPercent(float animationPercent)
        {
            if (TextureDebugPanel != null)
            {
                TextureDebugPanel.SetAnimationPercent(animationPercent);
            }
        }
        
        public void ToggleAnimation(bool state)
        {
            if (TextureDebugPanel != null)
            {
                TextureDebugPanel.SetAnimationState(state);
            }
        }
    }
}