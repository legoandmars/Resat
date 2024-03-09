using System;
using Resat.Biomes;
using Resat.Models;
using Resat.Models.Events;
using UnityEngine;

namespace Resat.Intermediates
{
    public class BiomeIntermediate : MonoBehaviour
    {
        public event Action<BiomeType>? BiomeChangeRequested;
        public event Action<BiomeChangeEvent>? BiomeChanged;
        
        public void RequestBiomeChange(BiomeType biomeType)
        {
            BiomeChangeRequested?.Invoke(biomeType);
        }
        
        public void ChangeBiome(BiomeChangeEvent biomeChangeEvent)
        {
            BiomeChanged?.Invoke(biomeChangeEvent);
        }
    }
}