using System;
using Resat.Biomes;
using Resat.Models;
using UnityEngine;

namespace Resat.Intermediates
{
    public class BiomeIntermediate : MonoBehaviour
    {
        public event Action<BiomeType>? BiomeChangeRequested;
        public event Action<BiomeSO>? BiomeChanged;
        
        public void RequestBiomeChange(BiomeType biomeType)
        {
            BiomeChangeRequested?.Invoke(biomeType);
        }
        
        public void ChangeBiome(BiomeSO biomeSO)
        {
            BiomeChanged?.Invoke(biomeSO);
        }
    }
}