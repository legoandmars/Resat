using System;
using System.Collections.Generic;
using Resat.Models;

namespace Resat.Quests
{
    // wrapper over SO to use scene object references, but still have the ability to reference quests in SOs
    [Serializable]
    public struct QuestReference
    {
        public QuestSO QuestSO;
        public List<SaturatedObjectData> SaturatedObjects; // may be unused depending on quest type, but unity inspector will make nullable here not do anything 
    }
}