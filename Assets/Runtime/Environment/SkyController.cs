using Resat.Intermediates;
using Resat.Models.Events;
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
            
            // TODO: Tween
            _skyboxMaterial.SetColor(TopColor, biome.SkyboxTopColor);
            _skyboxMaterial.SetColor(BottomColor, biome.SkyboxBottomColor);
        }
    }
}