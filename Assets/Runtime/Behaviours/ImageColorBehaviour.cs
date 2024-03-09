using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Resat.Intermediates;
using Resat.Models;
using Resat.Models.Events;
using Resat.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Resat.Behaviours
{
    public class ImageColorBehaviour : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField]
        private BiomeIntermediate? _biomeIntermediate;
        
        [SerializeField]
        private TweenController _tweenController = null!;
        
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
            if (_dialogueImages.Count == 0)
                return;
            
            var biome = biomeChangeEvent.Biome;

            if (biomeChangeEvent.FirstChange)
            {
                SetImageColor(biomeChangeEvent.Biome.DialogueBoxColor);
            }
            else
            {
                // assumes all images start out as the same color, as we're setting them to the same color anyways
                AnimateColors(_dialogueImages[0].color, biome.DialogueBoxColor).Forget();
            }
        }

        private async UniTask AnimateColors(Color startColor, Color endColor)
        {
            await _tweenController.TweenColors(startColor, endColor, SetImageColor, ColorTweenType.ImageBehaviourColor);
        }

        private void SetImageColor(Color color)
        {
            foreach (var image in _dialogueImages)
            {
                image.color = color;
            }
        }
    }
}