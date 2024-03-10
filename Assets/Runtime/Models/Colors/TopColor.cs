using System;
using UnityEngine;

namespace Resat.Models
{
    public struct TopColor : IComparable<TopColor>
    {
        // takes up 16 bytes
        // structured this way to directly deserialize from the buffer
        private float R;
        private float G;
        private float B;
        public float Count;
        private float H;
        private float S;
        private float L;
        public float ArrayIndex;

        public Color Color => new Color(R, G, B, 1f);
        public Color Okhsl => new Color(H, S, L, 1f);
        
        private string ColorToString(Color color, string? prefix = null)
        {
            if (prefix == null)
                return color.ToString();

            return $"{prefix}({color.r}, {color.g}, {color.b}, {color.a})";
        }
        
        public override string ToString()
        {
            return $"{ColorToString(Color)}, {ColorToString(Okhsl, "OKHSL")}, (Count: {Count}, Index: {ArrayIndex})";
        }

        public int CompareTo(TopColor other)
        {
            return (int)(other.Count - Count);
        }
    }
}