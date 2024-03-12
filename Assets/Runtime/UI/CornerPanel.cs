using System;
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
        private CornerTweenSettings _tweenSettings = new();

        public async UniTask<bool> Close(bool instant = false, Action<float>? setFloat = null, Action<Vector2>? setVector = null)
        {
            if (MainPanel == null || Corners.AnyCornersNull)
                return false;

            if (_tweenController == null || instant)
            {
                // don't lerp
                SetSize(Vector2.zero);
                SetCornersSize(Vector2.zero);
                return true;
            }

            var success = await _tweenController.TweenCornerPanelOut(
                _tweenSettings,
                MainPanel.sizeDelta,
                Corners.CornersContainer!.localScale,
                SetSize,
                SetCornersSize,
                null,
                setFloat,
                setVector);

            return success;
        }
        
        public async UniTask<bool> Open(bool instant = false, Action<float>? setFloat = null, Action<Vector2>? setVector = null)
        {
            if (MainPanel == null)
                return false;

            if (_tweenController == null || instant)
            {
                // don't lerp
                SetSize(_tweenSettings.Size);
                SetCornersSize(Vector2.one);
                return true;
            }
            
            var success = await _tweenController.TweenCornerPanelIn(
                _tweenSettings,
                MainPanel.sizeDelta,
                Corners.CornersContainer!.localScale,
                SetSize,
                SetCornersSize,
                null,
                setFloat,
                setVector);

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