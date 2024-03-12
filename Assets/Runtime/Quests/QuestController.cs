using System.Collections.Generic;
using System.Linq;
using Resat.Behaviours;
using Resat.Intermediates;
using Resat.Models;
using Resat.Models.Events;
using Resat.Npcs;
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

        private void Start()
        {
            // disable initial objects
            DisableQuestObjects();
            
            // start initial quest
            // TODO: if needed don't hardcode this to first
            StartQuest(QuestReferences.First());
            StartQuest(QuestReferences[1]);
        }

        private void OnEnable()
        {
            _npcIntermediate.DialogueStopped += OnDialogueStopped;
            _npcIntermediate.DialogueStarted += OnDialogueStarted;
        }

        private void OnDisable()
        {
            _npcIntermediate.DialogueStopped -= OnDialogueStopped;
            _npcIntermediate.DialogueStarted -= OnDialogueStarted;
        }

        private IEnumerable<QuestReference> GetQuestsOfType(QuestState questState)
        {
            return QuestReferences.Where(x => x.State == questState);
        }
        
        private void SetObjectState(SaturatedObjectData objectData, SaturatedObjectState objectState)
        {
            bool normalActive = objectState == SaturatedObjectState.Active;
            bool permanentActive = objectState == SaturatedObjectState.Permanent;
            
            SetSaturationBehaviourState(objectData.SaturatedObject, normalActive);
            SetSaturationBehaviourState(objectData.PermanentObjectWhenComplete, permanentActive);
        }

        // TODO: Lerp
        private void SetSaturationBehaviourState(ForceSaturationBehaviour? saturationBehaviour, bool active)
        {
            if (saturationBehaviour == null)
                return;
            
            saturationBehaviour.SetActive(active);
            if (active)
                _forceSaturationBuffer.AddRenderers(saturationBehaviour.Renderers, saturationBehaviour.Resaturate);
            else
                _forceSaturationBuffer.RemoveRenderers(saturationBehaviour.Renderers, saturationBehaviour.Resaturate);
        }
        
        public bool AllQuestItemsCollected(QuestSO quest)
        {
            return GetQuestsOfType(QuestState.AllItemsCollected).Any(x => x.QuestSO == quest);
        }
        
        // used for removing objects when picked up
        private void OnDialogueStopped(DialogueStoppedEvent dialogueStoppedEvent)
        {
            CompleteQuestItemsIfNecessary(dialogueStoppedEvent.Npc);
        }

        private void OnDialogueStarted(DialogueStartedEvent dialogueStartedEvent)
        {
            CompleteQuestIfNecessary(dialogueStartedEvent.Npc);
        }

        private void DisableQuestObjects()
        {
            foreach (var questReference in QuestReferences)
            {
                foreach (var saturatedObject in questReference.SaturatedObjects)
                {
                    SetObjectState(saturatedObject, SaturatedObjectState.Inactive);
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
                    
                    SetObjectState(saturatedObject, SaturatedObjectState.Active);
                }
            }

            questReference.State = QuestState.Active;
        }

        private void CompleteQuestItemsIfNecessary(NpcSO npc)
        {
            foreach (var activeQuest in GetQuestsOfType(QuestState.Active))
            {
                if (activeQuest.QuestSO.QuestType == QuestType.FindSaturatedObject)
                {
                    foreach (var saturatedObject in activeQuest.SaturatedObjects)
                    {
                        if (npc != saturatedObject.AssignedNPC)
                            continue;
                    
                        // collect saturated object
                        SetObjectState(saturatedObject, SaturatedObjectState.Collected);
                        saturatedObject.Collected = true;
                    }
                }
            }

            // change state of any now-fully collected quests
            foreach (var activeQuest in GetQuestsOfType(QuestState.Active))
            {
                if (activeQuest.QuestSO.QuestType == QuestType.FindSaturatedObject)
                {
                    if (activeQuest.SaturatedObjects.Any(x => !x.Collected))
                        continue;
                    
                    AllQuestItemsCollected(activeQuest);
                    
                    // set NPC exclamation mark if needed
                    SetSaturationBehaviourState(activeQuest.QuestNpc?.ExclamationPoint, true);
                }
            }
        }

        private void CompleteQuestIfNecessary(NpcSO npc)
        {
            foreach (var activeQuest in GetQuestsOfType(QuestState.AllItemsCollected))
            {
                if (activeQuest.QuestSO.QuestType == QuestType.FindSaturatedObject && activeQuest.QuestNpc?.NpcSO == npc)
                {
                    foreach (var saturatedObject in activeQuest.SaturatedObjects)
                    {
                        SetObjectState(saturatedObject, SaturatedObjectState.Permanent);
                    }

                    CompleteQuest(activeQuest);
                    
                    // set NPC exclamation mark if needed
                    SetSaturationBehaviourState(activeQuest.QuestNpc?.ExclamationPoint, false);
                }
            }
        }

        private void AllQuestItemsCollected(QuestReference questReference)
        {
            questReference.State = QuestState.AllItemsCollected;
        }

        private void CompleteQuest(QuestReference questReference)
        {
            questReference.State = QuestState.Completed;
        }
    }
}