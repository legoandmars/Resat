using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Resat.Colors
{
    public class VibeUtilites
    {
        public string GetVibe(List<Color> okhslColors)
        {
            // TODO: Take all colors into account instead of just the first
            foreach (var color in okhslColors)
            {
                var colorDescription = ColorDescription.Get(color);
                return string.Join(' ', colorDescription.Select(x => x.ToString()));
            }

            return "Default";
        }
    }
}