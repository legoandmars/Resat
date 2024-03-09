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

        public async UniTask Close()
        {
            if (MainPanel == null)
                return;

            if (_tweenController == null)
            {
                // don't lerp
                MainPanel.gameObject.SetActive(false);
                return;
            }

            
            var mainTween = _tweenController.TweenVectors(MainPanel.sizeDelta, Vector2.zero, _tweenSettings.Duration, SetSize, _tweenSettings.XEase, _tweenSettings.YEase);
            await UniTask.WaitForSeconds(_tweenSettings.Duration - _tweenSettings.CornerDurationOffset);
            var cornersTween = _tweenController.TweenVectors(Vector2.one, Vector2.zero, _tweenSettings.CornerDuration, SetCornersSize, _tweenSettings.CornersEaseOut, _tweenSettings.CornersEaseOut);
            await cornersTween;
        }

        
        public async UniTask Open()
        {
            if (MainPanel == null)
                return;

            if (_tweenController == null)
            {
                // don't lerp
                MainPanel.gameObject.SetActive(false);
                return;
            }
            
            
            var mainTween = _tweenController.TweenVectors(MainPanel.sizeDelta, _tweenSettings.Size, _tweenSettings.Duration, SetSize, _tweenSettings.XEase, _tweenSettings.YEase);
            var cornersTween = _tweenController.TweenVectors(Vector2.zero, Vector2.one, _tweenSettings.CornerDuration, SetCornersSize, _tweenSettings.CornersEaseIn, _tweenSettings.CornersEaseIn);

            await mainTween;
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