using Resat.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Resat.UI
{
    public class CameraPanelController : MonoBehaviour
    {
        [SerializeField]
        private DesaturationCamera _desaturationCamera = null!;

        [SerializeField]
        private CanvasScaler _canvasScaler = null!;
        
        [SerializeField]
        private RectTransform _mainPanel = null!;
        
        [SerializeField]
        private CameraDebugPanel _debugPanel = null!;
        
        public void SetResolution(CameraResolutionData resolutionData)
        {
            _desaturationCamera.SetResolution(resolutionData);
            
            // get "difference" between 1080p canvas scaler
            _mainPanel.sizeDelta = resolutionData.GetRescaledResolution(_canvasScaler.referenceResolution);
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