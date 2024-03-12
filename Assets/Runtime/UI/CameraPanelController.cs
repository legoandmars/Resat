using Resat.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Resat.UI
{
    public class CameraPanelController : MonoBehaviour
    {
        [SerializeField]
        private CanvasScaler _canvasScaler = null!;
        
        [SerializeField]
        private RectTransform _mainPanel = null!;

        [SerializeField]
        private RectTransform _leftInfoPanel = null!;
        
        [SerializeField]
        private CameraDebugPanel _debugPanel = null!;
        
        public void SetResolution(CameraResolutionData resolutionData)
        {
            _debugPanel.SetPreviewTextureResolution(resolutionData, _canvasScaler);
        }

        public void SetData(OKHSLData okhslData)
        {
            _debugPanel.SetData(okhslData);
        }

        public void SetPreviewTexture(RenderTexture renderTexture)
        {
            _debugPanel.SetPreviewTexture(renderTexture);
        }
        
        public void SetArrayTexture(RenderTexture renderTexture)
        {
            _debugPanel.SetArrayTexture(renderTexture);
        }
    }
}