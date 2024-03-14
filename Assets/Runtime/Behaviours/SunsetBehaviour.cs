using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Resat.Biomes;
using Resat.Cameras;
using Resat.Models;
using Resat.Quests;
using UnityEngine;

namespace Resat.Behaviours
{
    public class SunsetBehaviour : MonoBehaviour
    {
        [SerializeField]
        private PhotoController? _photoController = null;
        
        [SerializeField]
        private BiomeController? _biomeController;

        [SerializeField]
        private QuestController? _questController;

        [SerializeField]
        private BiomeSO? _sunsetBiome;
        
        [SerializeField]
        private GameObject? _sunsetObject;
        
        [SerializeField]
        private GameObject? _nonSunsetNpcs;

        [SerializeField]
        private List<NpcTriggerBehaviour> _endingNpcs = new();
        
        [SerializeField]
        private Camera _actualPhotoCameraWithSmallViewport = null!;

        private bool _sunsetted = false;
        private OKHSLData? _okhslData;
        
        private void DoSunset()
        {
            if (_photoController == null || _sunsetBiome == null || _sunsetObject == null || _biomeController == null || _nonSunsetNpcs == null || _sunsetted)
                return;
            
            _photoController.ShowTopNotification("Everybody is gathering around the campfire.").Forget();
            
            // set to final sunset biome, and disable all biome changing
            _biomeController.DisableBiomeChanging();
            _biomeController.ChangeBiome(new BiomeReference(_sunsetBiome, null, BiomeType.Sunset, true), false, true);
            
            // enable visuals (new NPCs, physical sun, etc)
            _sunsetObject.SetActive(true);
            _nonSunsetNpcs.SetActive(false);
            _sunsetted = true;
        }

        private void Awake()
        {
            if (_sunsetObject == null)
                return;
            
            _sunsetObject.SetActive(false);
        }

        private void OnEnable()
        {
            if (_questController == null)
                return;
            
            _questController.OnQuestCompleted += OnQuestCompleted;
        }

        private void OnDisable()
        {
            if (_questController == null)
                return;
            
            _questController.OnQuestCompleted -= OnQuestCompleted;
        }

        private void OnQuestCompleted(QuestSO obj)
        {
            CheckForSunsetCondition();
        }

        private async void Start()
        {
            await UniTask.WaitForSeconds(10f);
            
            // DoSunset();
        }

        public void CheckForSunsetCondition()
        {
            int minColors = 64;
            if (_sunsetted || _questController == null || _okhslData == null)
                return;

            if (_okhslData.ColorsLeft > minColors)
                return;
            
            bool questsCompleted = _questController.AllQuestsComplete();
            if (questsCompleted && _okhslData.ColorsLeft <= minColors)
            {
                DoSunset();
            }
        }

        public void UpdateOkhslData(OKHSLData okhslData)
        {
            _okhslData = okhslData;
            CheckForSunsetCondition();
        }

        public bool DoEndingCutscene()
        {
            if (!_sunsetted)
                return false;
            
            var planes = GeometryUtility.CalculateFrustumPlanes(_actualPhotoCameraWithSmallViewport);

            // check npcs
            foreach (var npc in _endingNpcs)
            {
                if (!(npc.SpriteRenderer != null && npc.SpriteRenderer.isVisible && npc.GetComponent<Collider>() != null &&
                    GeometryUtility.TestPlanesAABB(planes, npc.GetComponent<Collider>().bounds)))
                {
                    return false;
                }
            }

            return true;
        }
    }
}