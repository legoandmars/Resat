using System.Collections.Generic;

namespace Resat.Models
{
    public class OKHSLData
    {
        public TopColor[] TopColors;
        public uint TotalColorCount;
        public float TotalColorCoveragePercent;
        public uint NewColorCount;
        public float NewColorCoveragePercent;
        public uint ExistingColorCount;
        public uint ColorsLeft;

        public OKHSLData(TopColor[] topColors, uint totalColorCount, float totalColorCoveragePercent, uint newColorCount, float newColorCoveragePercent, uint existingColorCount, uint colorsLeft)
        {
            TopColors = topColors;
            TotalColorCount = totalColorCount;
            TotalColorCoveragePercent = totalColorCoveragePercent;
            NewColorCount = newColorCount;
            NewColorCoveragePercent = newColorCoveragePercent;
            ExistingColorCount = existingColorCount;
            ColorsLeft = colorsLeft;
        }
    }
}