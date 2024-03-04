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
            var resolution = resolutionData.GetRescaledResolution(_canvasScaler.referenceResolution);
            var nativeResolution = resolutionData.NativeResolution / resolutionData.GetNativeResolutionScale(_canvasScaler.referenceResolution);
            
            _mainPanel.sizeDelta = resolution;

            Debug.Log(resolution);
            Debug.Log(nativeResolution);
            float xOffset = (resolutionData.Center.x - 0.5f) * nativeResolution.x;
            float yOffset = (resolutionData.Center.y - 0.5f) * nativeResolution.y;

            _mainPanel.anchoredPosition = new Vector2(xOffset, yOffset);

            _debugPanel.SetPreviewTextureResolution(resolutionData, _canvasScaler);
            // TEMPORARY!!!
            // _debugPanel.CameraImage.rectTransform.sizeDelta = resolutionData.GetRescaledResolution(_canvasScaler.referenceResolution);
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