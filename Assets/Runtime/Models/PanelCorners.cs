using System;
using UnityEngine;

namespace Resat.Models
{
    [Serializable]
    public struct PanelCorners
    {
        public RectTransform? CornersContainer;
        public RectTransform? TopLeft;
        public RectTransform? TopRight;
        public RectTransform? BottomLeft;
        public RectTransform? BottomRight;

        public bool AnyCornersNull => CornersContainer == null && TopLeft == null && TopRight == null && BottomLeft == null && BottomRight == null;
    }
}