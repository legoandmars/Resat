using Resat.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Resat.Runtime.UI
{
    public class CameraPanelController : MonoBehaviour
    {
        [SerializeField]
        private DesaturationCamera _desaturationCamera = null!;

        [SerializeField]
        private CanvasScaler _canvasScaler = null!;
        
        [SerializeField]
        private RectTransform _mainPanel = null!;
        
        public void SetResolution(CameraResolutionData resolutionData)
        {
            _desaturationCamera.SetResolution(resolutionData);
            // get "difference" between 1080p canvas scaler
            var scale = resolutionData.NativeResolution / _canvasScaler.referenceResolution;
            Debug.Log(scale);
            Debug.Log(resolutionData.Resolution.x);
            _mainPanel.sizeDelta = new Vector2(resolutionData.Resolution.x / scale.x, resolutionData.Resolution.y / scale.y);
            Debug.Log(_mainPanel.anchorMax.x);
            Debug.Log(_mainPanel.sizeDelta.x);
        }
    }
}