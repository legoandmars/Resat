using Resat.Biomes;

namespace Resat.Models.Events
{
    public struct BiomeChangeEvent
    {
        public BiomeSO Biome;
        public bool FirstChange;

        public BiomeChangeEvent(BiomeSO biome, bool firstChange)
        {
            Biome = biome;
            FirstChange = firstChange;
        }
    }
}