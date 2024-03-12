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
        private RectTransform? _scaledWidthRectTransform;
        
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

        public async UniTask AnimateScaleIn(float duration)
        {
            await _tweenController.RunTween(duration, OnWidthTween, _ease, 3840, 2750);
        }

        private void OnWidthTween(float value)
        {
            if (_scaledWidthRectTransform == null)
                return;

            _scaledWidthRectTransform.sizeDelta = new Vector2(value, 2160);
        }

        private void OnSizeTween(float value)
        {
            if (_scaledRectTransform == null)
                return;
            
            _scaledRectTransform.localScale = new Vector3(value, value, 1);
        }
    }
}