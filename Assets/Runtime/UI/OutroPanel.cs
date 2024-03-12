using System.Collections.Generic;
using System.Linq;
using AuraTween;
using Cysharp.Threading.Tasks;
using Resat.Models;
using Resat.Tweening;
using UnityEngine;

namespace Resat.UI
{
    public class OutroPanel : MonoBehaviour
    {
        [SerializeField]
        private TweenController _tweenController = null!;
        
        [SerializeField]
        private CanvasGroup? _canvasGroup;

        [SerializeField]
        private RectTransform? _scaledRectTransform;
        
        [SerializeField]
        private List<ScreenshotPanel> _screenshotPanels = new();

        [SerializeField]
        private Ease _ease = Ease.Linear;
        
        private List<ScreenshotData> _screenshots = new();
        private int pageWidth = 4;

        public void SetActive(bool active)
        {
            if (_canvasGroup == null)
                return;
            
            // do stuff
            _canvasGroup.alpha = active ? 1f : 0f;
            _canvasGroup.interactable = active;
            _canvasGroup.blocksRaycasts = active;
        }
        
        public void SetScreenshotData(List<ScreenshotData> screenshots)
        {
            // reverse sort so we always have the newest screenshot first for the transition
            _screenshots = screenshots;
            _screenshots.Reverse();

            ApplyPage(0);
        }

        private void ApplyPage(int page)
        {
            // TODO proper sanity checks 
            int startIndex = pageWidth * page;
            
            for (int i = 0; i + startIndex < _screenshots.Count && i < 4; i++)
            {
                var screenshot = _screenshots[i + startIndex];
                _screenshotPanels[i].SetTexture(screenshot.RenderTexture);
            }
        }
        
        public async UniTask AnimateScaleOut(float duration)
        {
            await _tweenController.RunTween(duration, OnSizeTween, _ease, 1f, 0.5f);
        }

        private void OnSizeTween(float sizeScale)
        {
            _scaledRectTransform.localScale = new Vector3(sizeScale, sizeScale, 1);
        }
    }
}