using System;
using System.Collections.Generic;
using Resat.Intermediates;
using Resat.Models;
using UnityEngine;

namespace Resat.Biomes
{
    public class BiomeController : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField]
        private BiomeIntermediate _biomeIntermediate = null!;
        
        [SerializeField]
        private BiomeType _biomeType = BiomeType.Desert;

        [SerializeField]
        private List<BiomeSO> _biomes = new();

        private Dictionary<BiomeType, BiomeSO>? _biomesByType;
        private bool _transitioning = false;

        // cache a dictionary in awake because unity doesn't let us use dictionaries in-inspector
        private void Awake()
        {
            _biomesByType = new Dictionary<BiomeType, BiomeSO>();

            foreach (var biome in _biomes)
            {
                _biomesByType[biome.BiomeType] = biome;
            }
        }
        
        private void OnEnable()
        {
            _biomeIntermediate.BiomeChangeRequested += OnBiomeChangeRequested;
        }
        
        private void OnDisable()
        {
            _biomeIntermediate.BiomeChangeRequested -= OnBiomeChangeRequested;
        }

        private void OnBiomeChangeRequested(BiomeType biomeType)
        {
            if (_biomesByType == null || biomeType == _biomeType)
                return;

            if (!_biomesByType.TryGetValue(biomeType, out var biome))
                return;
            
            Debug.Log($"Switching biome to {biomeType}!");
            
            _biomeType = biomeType;
            _biomeIntermediate.ChangeBiome(biome);
        }
    }
}