using System;
using System.Collections.Generic;
using Resat.Colors;
using Resat.Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Resat.UI
{
    public class MoreInfoPanel : MonoBehaviour
    {
        [SerializeField]
        private RawImage? _mainImage;
        
        [SerializeField]
        private RawImage? _colorArrayImage;

        [SerializeField]
        private OKHSLController _okhslController = null!;
        
        [SerializeField]
        public TextMeshProUGUI? NewColorCountText;
        
        [SerializeField]
        public TextMeshProUGUI? NewColorCountPercentageText;

        [SerializeField]
        public List<Image>? TopColorImages;
        public void SetScreenshotData(ScreenshotData screenshotData)
        {
            if (_mainImage == null || _colorArrayImage == null || NewColorCountText== null || NewColorCountPercentageText == null || TopColorImages == null)
                return;
            
            // force clear global array and re-render to get the correct colors
            _okhslController.ClearGlobalArray();
            var fakeOkhslData = _okhslController.RunComputeShader(screenshotData.PreviewRenderTexture);

            _mainImage.texture = screenshotData.RenderTexture;
            _colorArrayImage.texture = _okhslController.OutputArrayTexture;
            
            for (int i = 0; i < TopColorImages?.Count && i < screenshotData.OkhslData.TopColors.Length; i++)
            {
                var topColor = screenshotData.OkhslData.TopColors[i];
                var topColorImage = TopColorImages[i];
                topColorImage.color = topColor.Color;
            }

            var percent = Single.IsNaN(screenshotData.OkhslData.TotalColorCoveragePercent) ? 0.0f : screenshotData.OkhslData.TotalColorCoveragePercent;
            if (NewColorCountText != null)
                NewColorCountText.text = $"Total Colors: {screenshotData.OkhslData.TotalColorCount}";
            if (NewColorCountPercentageText != null)
                NewColorCountPercentageText.text = $"{percent:00.0}%";

        }
    }
}