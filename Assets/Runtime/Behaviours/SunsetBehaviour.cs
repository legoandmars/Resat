using Cysharp.Threading.Tasks;
using Resat.Biomes;
using Resat.Cameras;
using Resat.Models;
using UnityEngine;

namespace Resat.Behaviours
{
    public class SunsetBehaviour : MonoBehaviour
    {
        [SerializeField]
        private PhotoController? _photoController = null;

        [SerializeField]
        private BiomeSO? _sunsetBiome;

        [SerializeField]
        private BiomeController? _biomeController;
        
        [SerializeField]
        private GameObject? _sunsetObject;
        
        [SerializeField]
        private GameObject? _nonSunsetNpcs;
        private void DoSunset()
        {
            if (_photoController == null || _sunsetBiome == null || _sunsetObject == null || _biomeController == null || _nonSunsetNpcs == null)
                return;
            
            _photoController.ShowTopNotification("Everybody is gathering around the campfire.").Forget();
            
            // set to final sunset biome, and disable all biome changing
            _biomeController.DisableBiomeChanging();
            _biomeController.ChangeBiome(new BiomeReference(_sunsetBiome, null, BiomeType.Sunset, true), false, true);
            
            // enable visuals (new NPCs, physical sun, etc)
            _sunsetObject.SetActive(true);
            _nonSunsetNpcs.SetActive(false);
        }

        private void Awake()
        {
            if (_sunsetObject == null)
                return;
            
            _sunsetObject.SetActive(false);
        }

        private async void Start()
        {
            await UniTask.WaitForSeconds(10f);
            
            DoSunset();
        }
    }
}