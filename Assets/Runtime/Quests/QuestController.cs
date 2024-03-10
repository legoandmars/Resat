using System.Collections.Generic;
using System.Linq;
using Resat.Models;
using Resat.Rendering;
using UnityEngine;

namespace Resat.Quests
{
    public class QuestController : MonoBehaviour
    {
        [SerializeField]
        private ForceSaturationBuffer _forceSaturationBuffer = null!;
        
        [SerializeField]
        public List<QuestReference> QuestReferences = new();

        private void Start()
        {
            // disable initial objects
            DisableQuestObjects();
            
            // start initial quest
            // TODO: if needed don't hardcode this to first
            StartQuest(QuestReferences.First());
        }

        private void DisableQuestObjects()
        {
            foreach (var questReference in QuestReferences)
            {
                foreach (var saturatedObject in questReference.SaturatedObjects)
                {
                    saturatedObject.SaturatedObject?.SetActive(false);
                    saturatedObject.PermanentObjectWhenComplete?.SetActive(false);
                }
            }
        }

        private void StartQuest(QuestReference questReference)
        {
            if (questReference.QuestSO.QuestType == QuestType.FindSaturatedObject)
            {
                // spawn all initial objects
                foreach (var saturatedObject in questReference.SaturatedObjects)
                {
                    if (saturatedObject.SaturatedObject == null)
                        continue;
                    
                    saturatedObject.SaturatedObject.SetActive(true);
                    // should really be like, one method TODO
                    _forceSaturationBuffer.AddRenderers(saturatedObject.SaturatedObject.Renderers, true);
                }
            }
        }
    }
}