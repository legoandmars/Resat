using UnityEngine;

namespace Resat.Models
{
    public struct TopColor
    {
        public Color Color;
        public Color Okhsl;
        public int ArrayIndex;
        public int Count;
        
        public float R => Color.r;
        public float G => Color.g;
        public float B => Color.b;
        public float H => Okhsl.r;
        public float S => Okhsl.g;
        public float L => Okhsl.b;
        
        public TopColor(Color color, Color okhsl, int arrayIndex, int count)
        {
            Color = color;
            Okhsl = okhsl;
            ArrayIndex = arrayIndex;
            Count = count;
        }

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
    }
}