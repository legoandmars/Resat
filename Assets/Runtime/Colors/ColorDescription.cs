using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Resat.Colors
{

    // https://github.com/waacton/Unicolour/blob/bb075f370965a9eec97725005391fde7a876ec6e/Unicolour/ColourDescription.cs
    internal class ColorDescription
    {
        private readonly string description;

        public ColorDescription(string description)
        {
            this.description = description;
        }

        internal static readonly ColorDescription NotApplicable = new("-"); 
        internal static readonly ColorDescription Black = new(nameof(Black)); 
        internal static readonly ColorDescription Shadow = new(nameof(Shadow)); 
        internal static readonly ColorDescription Dark = new(nameof(Dark)); 
        internal static readonly ColorDescription Pure = new(nameof(Pure)); 
        internal static readonly ColorDescription Light = new(nameof(Light)); 
        internal static readonly ColorDescription Pale = new(nameof(Pale)); 
        internal static readonly ColorDescription White = new(nameof(White));
        internal static readonly ColorDescription Grey = new(nameof(Grey)); 
        internal static readonly ColorDescription Faint = new(nameof(Faint)); 
        internal static readonly ColorDescription Weak = new(nameof(Weak)); 
        internal static readonly ColorDescription Mild = new(nameof(Mild)); 
        internal static readonly ColorDescription Strong = new(nameof(Strong)); 
        internal static readonly ColorDescription Vibrant = new(nameof(Vibrant));
        internal static readonly ColorDescription Red = new(nameof(Red)); 
        internal static readonly ColorDescription Orange = new(nameof(Orange)); 
        internal static readonly ColorDescription Yellow = new(nameof(Yellow)); 
        internal static readonly ColorDescription Chartreuse = new(nameof(Chartreuse)); 
        internal static readonly ColorDescription Green = new(nameof(Green)); 
        internal static readonly ColorDescription Mint = new(nameof(Mint)); 
        internal static readonly ColorDescription Cyan = new(nameof(Cyan)); 
        internal static readonly ColorDescription Azure = new(nameof(Azure)); 
        internal static readonly ColorDescription Blue = new(nameof(Blue)); 
        internal static readonly ColorDescription Violet = new(nameof(Violet)); 
        internal static readonly ColorDescription Magenta = new(nameof(Magenta)); 
        internal static readonly ColorDescription Rose = new(nameof(Rose));

        internal static readonly List<ColorDescription> Lightnesses = new() { Faint, Weak, Pure, Light, Pale };
        internal static readonly List<ColorDescription> Hues = new() { Red, Orange, Yellow, Chartreuse, Green, Mint, Cyan, Azure, Blue, Violet, Magenta, Rose };
        // internal static readonly List<ColorDescription> Greyscales = new() { Black, Grey, White };
        
        internal static IEnumerable<ColorDescription> Get(UnityEngine.Color hsl)
        {
            // *might* need a - 15f or something to hue, some colors seem a bit off
            var (h, s, l) = ((hsl.r * 360), hsl.g, hsl.b);
            switch (l)
            {
                case <= 0: return new List<ColorDescription> { Black };
                case >= 1: return new List<ColorDescription> { White };
            }

            Debug.Log(hsl.r * 360);
            var lightness = l switch
            {
                < 0.20f => Faint,
                < 0.40f => Weak,
                < 0.60f => Pure,
                < 0.80f => Light,
                _ => Pale
            };

            var hue = h switch
            {
                < 15 => Red,
                < 45 => Orange,
                < 75 => Yellow,
                < 105 => Chartreuse,
                < 135 => Green,
                < 165 => Mint,
                < 195 => Cyan,
                < 225 => Azure,
                < 255 => Blue,
                < 285 => Violet,
                < 315 => Magenta,
                < 345 => Rose,
                _ => Red
            };

            return new List<ColorDescription> { lightness, hue };
        }
        
        public override string ToString() => description.ToLower();
    }
}
