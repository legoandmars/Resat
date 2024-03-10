using System.Collections.Generic;
using System.Linq;
using Resat.Intermediates;
using Resat.Models;
using Resat.Models.Events;
using Resat.Rendering;
using UnityEngine;

namespace Resat.Quests
{
    public class QuestController : MonoBehaviour
    {
        [SerializeField]
        private ForceSaturationBuffer _forceSaturationBuffer = null!;
        
        [SerializeField]
        private NpcIntermediate _npcIntermediate = null!;

        [SerializeField]
        public List<QuestReference> QuestReferences = new();

        private List<QuestReference> _activeQuests = new();
        private List<QuestReference> _completedQuests = new(); // TODO: how do you mark a quest as complete?
        
        private void Start()
        {
            // disable initial objects
            DisableQuestObjects();
            
            // start initial quest
            // TODO: if needed don't hardcode this to first
            StartQuest(QuestReferences.First());
        }

        private void OnEnable()
        {
            _npcIntermediate.DialogueStopped += OnDialogueStopped;
        }

        private void OnDisable()
        {
            _npcIntermediate.DialogueStopped -= OnDialogueStopped;
        }

        public bool QuestIsComplete(QuestSO quest)
        {
            return _completedQuests.Any(x => x.QuestSO == quest);
        }
        
        // used for removing objects when picked up
        private void OnDialogueStopped(DialogueStoppedEvent dialogueStoppedEvent)
        {
            foreach (var activeQuest in _activeQuests)
            {
                foreach (var saturatedObject in activeQuest.SaturatedObjects)
                {
                    if (dialogueStoppedEvent.Npc != saturatedObject.AssignedNPC)
                        continue;
                    
                    CompleteSaturatedObject(saturatedObject);
                    // fix up saturated object
                }
            }
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

        // TODO: Think more about how multi-support would work
        // TODO: see if there's any way we can interpolate this? slowly scale down?
        private void CompleteSaturatedObject(SaturatedObjectData saturatedObject)
        {
            if (saturatedObject.SaturatedObject == null)
                return;
            
            saturatedObject.SaturatedObject.SetActive(false);
            _forceSaturationBuffer.RemoveRenderers(saturatedObject.SaturatedObject.Renderers, true);

            if (saturatedObject.PermanentObjectWhenComplete == null)
                return;
            
            saturatedObject.PermanentObjectWhenComplete.SetActive(true);
            _forceSaturationBuffer.AddRenderers(saturatedObject.PermanentObjectWhenComplete.Renderers, true);
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
            
            _activeQuests.Add(questReference);
        }
    }
}