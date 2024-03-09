using Resat.Models;
using UnityEngine;

namespace Resat.Biomes
{
    [CreateAssetMenu(fileName = "Biome", menuName = "ScriptableObjects/Biome", order = 1)]
    public class BiomeSO : ScriptableObject
    {
        public BiomeType BiomeType;
        public Color DialogueBoxColor = new(0,0,0,1);
        public Color SkyboxTopColor = new Color(1, 1, 1, 1);
        public Color SkyboxBottomColor = new Color(1, 1, 1, 1);
    }
}