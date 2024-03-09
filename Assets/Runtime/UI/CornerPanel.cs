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
        private Vector2 _size;
        
        [SerializeField]
        private float _animationDuration = 1f;
        
        [SerializeField]
        private float _cornerAnimationDuration = 1f;

        [SerializeField]
        private float _cornerOutAnimationOffset = 0f;
        
        [SerializeField]
        private Ease _xEase;
        
        [SerializeField]
        private Ease _yEase;
        
        [SerializeField]
        private Ease _cornersEaseIn;
        
        [SerializeField]
        private Ease _cornersEaseOut;

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

            
            var mainTween = _tweenController.TweenVectors(MainPanel.sizeDelta, Vector2.zero, _animationDuration, SetSize, _xEase, _yEase);
            await UniTask.WaitForSeconds(_animationDuration - _cornerOutAnimationOffset);
            var cornersTween = _tweenController.TweenVectors(Vector2.one, Vector2.zero, _cornerAnimationDuration, SetCornersSize, _cornersEaseOut, _cornersEaseOut);
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
            
            
            var mainTween = _tweenController.TweenVectors(MainPanel.sizeDelta, _size, _animationDuration, SetSize, _xEase, _yEase);
            var cornersTween = _tweenController.TweenVectors(Vector2.zero, Vector2.one, _cornerAnimationDuration, SetCornersSize, _cornersEaseIn, _cornersEaseIn);

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