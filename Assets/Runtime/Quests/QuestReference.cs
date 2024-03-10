using System;
using System.Collections.Generic;
using Resat.Behaviours;
using Resat.Models;

namespace Resat.Quests
{
    // wrapper over SO to use scene object references, but still have the ability to reference quests in SOs
    [Serializable]
    public class QuestReference
    {
        public QuestSO QuestSO;
        public NpcTriggerBehaviour? QuestNpc;
        public List<SaturatedObjectData> SaturatedObjects = new(); // may be unused depending on quest type, but unity inspector will make nullable here not do anything 
        public QuestState State = QuestState.NotStarted;

        public QuestReference(QuestSO questSO, List<SaturatedObjectData> saturatedObjects, QuestState state)
        {
            QuestSO = questSO;
            SaturatedObjects = saturatedObjects;
            State = state;
        }
    }
}