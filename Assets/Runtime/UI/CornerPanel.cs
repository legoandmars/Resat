using AuraTween;
using Cysharp.Threading.Tasks;
using Resat.Models;
using Resat.Tweening;
using UnityEngine;

namespace Resat.UI
{
    public class CornerPanel : MonoBehaviour
    {
        [Header("Dependencies")]
        public TweenController? _tweenController;
        
        [Header("UI")]
        public RectTransform? MainPanel;
        
        [SerializeField]
        public PanelCorners Corners;

        [Header("Settings")] 
        [SerializeField]
        private CornerTweenSettings _tweenSettings;

        public async UniTask<bool> Close()
        {
            if (MainPanel == null || Corners.AnyCornersNull)
                return false;

            if (_tweenController == null)
            {
                // don't lerp
                MainPanel.gameObject.SetActive(false);
                return true;
            }

            var success = await _tweenController.TweenCornerPanelOut(
                _tweenSettings,
                MainPanel.sizeDelta,
                Corners.CornersContainer!.localScale,
                SetSize,
                SetCornersSize,
                Models.TweenType.InteractionPrompt,
                Models.TweenType.InteractionPromptCorners);

            return success;
        }
        
        public async UniTask<bool> Open()
        {
            if (MainPanel == null)
                return false;

            if (_tweenController == null)
            {
                // don't lerp
                MainPanel.gameObject.SetActive(false);
                return true;
            }
            
            var success = await _tweenController.TweenCornerPanelIn(
                _tweenSettings,
                MainPanel.sizeDelta,
                Corners.CornersContainer!.localScale,
                SetSize,
                SetCornersSize,
                Models.TweenType.InteractionPrompt,
                Models.TweenType.InteractionPromptCorners);

            return success;
        }
        
        private void SetSize(Vector2 size)
        {
            if (MainPanel == null)
                return;

            MainPanel.sizeDelta = size;
        }

        private void SetCornersSize(Vector2 size)
        {
            if (Corners.AnyCornersNull)
                return;

            Corners.CornersContainer!.localScale = size;
        }
    }
}