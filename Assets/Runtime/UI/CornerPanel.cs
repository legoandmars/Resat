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
        public PanelCorners? Corners;

        [Header("Settings")] 
        [SerializeField]
        private Vector2 _size;
        
        [SerializeField]
        private float _animationDuration = 1f;
        
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
            
            // height is the same, so we only need to await one
            _tweenController.RunTween(_animationDuration, SetHeight, Ease.Linear, MainPanel.sizeDelta.y, 0f).Forget();
            await _tweenController.RunTween(_animationDuration, SetWidth, Ease.Linear, MainPanel.sizeDelta.x, 0f);
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
            
            
            // height is the same, so we only need to await one
            _tweenController.RunTween(_animationDuration, SetHeight, Ease.Linear, MainPanel.sizeDelta.y, _size.y).Forget();
            await _tweenController.RunTween(_animationDuration, SetWidth, Ease.Linear, MainPanel.sizeDelta.x, _size.x);
        }
        
        private void SetHeight(float height)
        {
            if (MainPanel == null)
                return;

            MainPanel.sizeDelta = new Vector2(MainPanel.sizeDelta.x, height);
        }

        private void SetWidth(float width)
        {
            if (MainPanel == null)
                return;

            MainPanel.sizeDelta = new Vector2(width, MainPanel.sizeDelta.y);
        }

    }
}