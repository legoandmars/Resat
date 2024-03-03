using System.Collections.Generic;

namespace Resat.Models
{
    public class OKHSLData
    {
        public TopColor[] TopColors;
        public int TotalColorCount;
        public float TotalColorCoveragePercent;
        
        // for debuggin
        
        // public int UniqueColorCount;
        // public int RemainingColorCoveragePercentage;
        
        public OKHSLData(TopColor[] topColors, int totalColorCount, float totalColorCoveragePercent)
        {
            TopColors = topColors;
            TotalColorCount = totalColorCount;
            TotalColorCoveragePercent = totalColorCoveragePercent;
        }
    }
}