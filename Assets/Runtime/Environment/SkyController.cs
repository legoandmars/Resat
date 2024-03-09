using Cysharp.Threading.Tasks;
using Resat.Intermediates;
using Resat.Models;
using Resat.Models.Events;
using Resat.Tweening;
using UnityEngine;

namespace Resat.Environment
{
    public class SkyController : MonoBehaviour
    {
        private static readonly int TopColor = Shader.PropertyToID("_Tint");
        private static readonly int BottomColor = Shader.PropertyToID("_Tint2");

        [Header("Dependencies")]
        [SerializeField]
        private BiomeIntermediate _biomeIntermediate = null!;
        
        [SerializeField]
        private TweenController _tweenController = null!;

        [SerializeField]
        private Material? _skyboxMaterial;

        private void Awake()
        {
            if (_skyboxMaterial == null)
                return;
            
            // instantiate material to avoid leaking changes into editor
            _skyboxMaterial = Instantiate(_skyboxMaterial);
            RenderSettings.skybox = _skyboxMaterial;
        }
        
        private void OnEnable()
        {
            _biomeIntermediate.BiomeChanged += OnBiomeChanged;
        }
        
        private void OnDisable()
        {
            _biomeIntermediate.BiomeChanged -= OnBiomeChanged;
        }

        private void OnBiomeChanged(BiomeChangeEvent biomeChangeEvent)
        {
            if (_skyboxMaterial == null)
                return;

            var biome = biomeChangeEvent.Biome;

            if (biomeChangeEvent.FirstChange)
            {
                SetTopColor(biome.SkyboxTopColor);
                SetBottomColor(biome.SkyboxBottomColor);
            }
            else
            {
                AnimateColors(_skyboxMaterial.GetColor(TopColor), biome.SkyboxTopColor, ColorTweenType.SkyboxTop).Forget();
                AnimateColors(_skyboxMaterial.GetColor(BottomColor), biome.SkyboxBottomColor, ColorTweenType.SkyboxBottom).Forget();
            }
        }

        private async UniTask AnimateColors(Color startColor, Color endColor, ColorTweenType colorTweenType)
        {
            await _tweenController.TweenColors(startColor, endColor, colorTweenType == ColorTweenType.SkyboxTop ? SetTopColor : SetBottomColor, colorTweenType);
        }

        private void SetTopColor(Color color)
        {
            if (_skyboxMaterial == null)
                return;

            _skyboxMaterial.SetColor(TopColor, color);
        }

        private void SetBottomColor(Color color)
        {
            if (_skyboxMaterial == null)
                return;

            _skyboxMaterial.SetColor(BottomColor, color);
        }
    }
}