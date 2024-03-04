using System.Collections.Generic;

namespace Resat.Models
{
    public class OKHSLData
    {
        public TopColor[] TopColors;
        public uint TotalColorCount;
        public float TotalColorCoveragePercent;
        
        // for debuggin
        
        // public int UniqueColorCount;
        // public int RemainingColorCoveragePercentage;
        
        public OKHSLData(TopColor[] topColors, uint totalColorCount, float totalColorCoveragePercent)
        {
            TopColors = topColors;
            TotalColorCount = totalColorCount;
            TotalColorCoveragePercent = totalColorCoveragePercent;
        }
    }
}