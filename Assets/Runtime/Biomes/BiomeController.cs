﻿using System;
using System.Collections.Generic;
using System.Linq;
using Resat.Intermediates;
using Resat.Models;
using Resat.Models.Events;
using Resat.Quests;
using UnityEngine;

namespace Resat.Biomes
{
    public class BiomeController : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField]
        private BiomeIntermediate _biomeIntermediate = null!;
        
        [SerializeField]
        private QuestController _questController = null!;
        
        [SerializeField]
        private BiomeType _initialBiomeType = BiomeType.Desert;

        [SerializeField]
        private List<BiomeReference> _biomes = new();
        
        // private bool _transitioning = false;
        private BiomeType _biomeType;
        private bool _canChangeBiomes = true;
        
        private void Start()
        {
            var biome = _biomes.FirstOrDefault(x => x.BiomeType == _initialBiomeType);
            
            if (biome != null)
                ChangeBiome(biome, true);
        }
        
        private void OnEnable()
        {
            _biomeIntermediate.BiomeChangeRequested += OnBiomeChangeRequested;
            _questController.OnQuestCompleted += OnQuestCompleted;
        }

        private void OnQuestCompleted(QuestSO quest)
        {
            foreach (var biome in _biomes)
            {
                if (biome.RequirementQuest != quest)
                    continue;

                biome.Unlocked = true;
            }
        }

        private void OnDisable()
        {
            _biomeIntermediate.BiomeChangeRequested -= OnBiomeChangeRequested;
        }

        private void OnBiomeChangeRequested(BiomeType biomeType)
        {
            if (!_canChangeBiomes)
                return;
            
            var biome = _biomes.FirstOrDefault(x => x.BiomeType == biomeType);
            if (biome == null)
                return;
            
            ChangeBiome(biome);
        }

        public void ChangeBiome(BiomeReference biome, bool first = false, bool overrideDisable = false)
        {
            if (!_canChangeBiomes && !overrideDisable)
                return;

            Debug.Log($"Switching biome to {biome.BiomeType}!");
            
            _biomeType = biome.BiomeType;
            _biomeIntermediate.ChangeBiome(new BiomeChangeEvent(biome.BiomeSO, first));
        }

        public void DisableBiomeChanging()
        {
            _canChangeBiomes = false;
        }
        
        public bool BiomeIsUnlocked(BiomeType? biomeType = null)
        {
            if (!_canChangeBiomes)
                return true;
            
            return _biomes.Any(x => x.BiomeType == (biomeType ?? _biomeType) && x.Unlocked);
        }
    }
}