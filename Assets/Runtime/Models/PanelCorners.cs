using System;
using UnityEngine;

namespace Resat.Models
{
    [Serializable]
    public struct PanelCorners
    {
        public RectTransform TopLeft;
        public RectTransform TopRight;
        public RectTransform BottomLeft;
        public RectTransform BottomRight;
    }
}