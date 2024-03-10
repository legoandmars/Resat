using System;
using Random = UnityEngine.Random;

namespace Resat.Models
{
    // glorified vector2, but it makes the type slightly more clear
    [Serializable]
    public struct TextAnimationSpeed
    {
        public float Min;
        public float Max;

        public TextAnimationSpeed(float min, float max)
        {
            Min = min;
            Max = max;
        }

        public float Sample()
        {
            return Random.Range(Min, Max);
        }
        
        // add float mult support
        public static TextAnimationSpeed operator *(TextAnimationSpeed textAnimationSpeed, float multiplier)
        {
            // multiplying the *speed* by a high number should make it *faster*, so we'll need to divide
            return new TextAnimationSpeed(textAnimationSpeed.Min / multiplier, textAnimationSpeed.Max / multiplier);
        }
    }
}