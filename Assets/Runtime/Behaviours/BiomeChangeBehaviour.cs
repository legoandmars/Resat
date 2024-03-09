using System;
using Resat.Biomes;
using Resat.Intermediates;
using Resat.Models;
using UnityEngine;

namespace Resat.Behaviours
{
    public class BiomeChangeBehaviour : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField]
        private BiomeIntermediate? _biomeIntermediate;
        
        [Header("Settings")]
        [SerializeField]
        private BiomeType _biomeType;

        private int? _playerLayer;
        
        private void Awake()
        {
            if (_biomeIntermediate == null)
            {
                Debug.LogWarning("Biome intermediate null on change behaviour! Not firing any events!");
                enabled = false;
                return;
            }
            _playerLayer = LayerMask.NameToLayer("Player");
        }
        
        private void OnTriggerEnter(Collider collider)
        {
            Debug.Log("GUH");
            if (_playerLayer == null || _biomeIntermediate == null || collider.gameObject.layer != _playerLayer)
                return;
            
            _biomeIntermediate.RequestBiomeChange(_biomeType);
        }
    }
}