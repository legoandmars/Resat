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
        private float _animationDuration = 1f;
        
        public void Close()
        {
            // TODO: Tween
            if (MainPanel == null)
                return;
            
            MainPanel.gameObject.SetActive(false);
        }

        public void Open()
        {
            if (MainPanel == null)
                return;

            MainPanel.gameObject.SetActive(true);
        }
    }
}