using System.Collections.Generic;
using Resat.Intermediates;
using Resat.Models;
using Resat.Models.Events;
using UnityEngine;
using UnityEngine.UI;

namespace Resat.Behaviours
{
    public class ImageColorBehaviour : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField]
        private BiomeIntermediate? _biomeIntermediate;
        
        [Header("Settings")]
        [SerializeField]
        private List<Image> _dialogueImages = new();

        private void Awake()
        {
            if (_biomeIntermediate == null)
            {
                Debug.LogWarning("Biome intermediate null on image color behaviour! Not receiving any events!");
                enabled = false;
                return;
            }
        }

        private void OnEnable()
        {
            if (_biomeIntermediate == null) 
                return;

            _biomeIntermediate.BiomeChanged += OnBiomeChanged;
        }
        
        private void OnDisable()
        {
            if (_biomeIntermediate == null) 
                return;
            
            _biomeIntermediate.BiomeChanged -= OnBiomeChanged;
        }

        private void OnBiomeChanged(BiomeChangeEvent biomeChangeEvent)
        {
            var biome = biomeChangeEvent.Biome;
            
            // TODO: Tween color if initial false
            foreach (var image in _dialogueImages)
            {
                image.color = biome.DialogueBoxColor;
            }
        }
    }
}