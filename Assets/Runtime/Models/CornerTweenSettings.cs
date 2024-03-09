using System;
using AuraTween;
using UnityEngine;

namespace Resat.Models
{
    [Serializable]
    public class CornerTweenSettings
    {
        public Vector2 Size;
        public float Duration = 1f;
        public float CornerDuration = 1f;
        public float CornerDurationOffset = 0f;
        public Ease XEase;
        public Ease YEase;
        public Ease CornersEaseIn;
        public Ease CornersEaseOut;
    }
}