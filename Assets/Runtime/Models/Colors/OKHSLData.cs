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
        
        public OKHSLData(TopColor[] topColors, uint totalColorCount, float totalColorCoveragePercent, uint newColorCount, float newColorCoveragePercent)
        {
            TopColors = topColors;
            TotalColorCount = totalColorCount;
            TotalColorCoveragePercent = totalColorCoveragePercent;
            NewColorCount = newColorCount;
            NewColorCoveragePercent = newColorCoveragePercent;
        }
    }
}