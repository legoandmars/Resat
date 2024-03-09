using System;
using System.Collections.Generic;
using Resat.Intermediates;
using Resat.Models;
using Resat.Models.Events;
using UnityEngine;

namespace Resat.Biomes
{
    public class BiomeController : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField]
        private BiomeIntermediate _biomeIntermediate = null!;
        
        [SerializeField]
        private BiomeType _initialBiomeType = BiomeType.Desert;

        [SerializeField]
        private List<BiomeSO> _biomes = new();

        private Dictionary<BiomeType, BiomeSO>? _biomesByType;
        // private bool _transitioning = false;
        private BiomeType _biomeType;
        
        // cache a dictionary in awake because unity doesn't let us use dictionaries in-inspector
        private void Awake()
        {
            _biomesByType = new Dictionary<BiomeType, BiomeSO>();

            foreach (var biome in _biomes)
            {
                _biomesByType[biome.BiomeType] = biome;
            }
            

        }

        private void Start()
        {
            if (_biomesByType == null || !_biomesByType.TryGetValue(_initialBiomeType, out var initialBiome))
                return;

            ChangeBiome(initialBiome, true);
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
            
            ChangeBiome(biome);
        }

        private void ChangeBiome(BiomeSO biome, bool first = false)
        {
            Debug.Log($"Switching biome to {biome.BiomeType}!");
            
            _biomeType = biome.BiomeType;
            _biomeIntermediate.ChangeBiome(new BiomeChangeEvent(biome, first));
        }
    }
}