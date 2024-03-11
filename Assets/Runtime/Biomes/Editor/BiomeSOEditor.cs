using Resat.Environment;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Resat.Biomes.Editor
{
    [CustomEditor(typeof(BiomeSO))]
    public class BiomeSOEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            BiomeSO targetBiome = (BiomeSO)target;
            EditorGUILayout.LabelField ("Some help", "Some other text");

            // Show default inspector property editor
            if (DrawDefaultInspector())
            {
                
            }

            GUILayout.Space(20);
            
            if (GUILayout.Button("Apply (THIS WILL APPLY IN EDITOR)"))
            {
                Debug.Log("WOW");
                Apply(targetBiome);
            }
        }

        private void Apply(BiomeSO biome)
        {
            if (biome == null)
                return;
            
            // find environment controller
            var environmentController = FindObjectOfType<EnvironmentController>();

            Debug.Log(environmentController);

            if (environmentController.SkyboxMaterial != null)
            {
                environmentController.SkyboxMaterial.SetColor(EnvironmentController.TopColor, biome.SkyboxTopColor);
                environmentController.SkyboxMaterial.SetColor(EnvironmentController.BottomColor, biome.SkyboxBottomColor);
            }

            foreach (var light in environmentController.Lights)
            {
                light.color = biome.LightingColor;
            }
        }
    }
}