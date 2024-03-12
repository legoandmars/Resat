using System;
using Resat.Biomes;
using Resat.Models;
using Resat.Quests;

namespace Resat.Models
{
    [Serializable]
    public class BiomeReference
    {
        public BiomeSO BiomeSO;
        public QuestSO? RequirementQuest;
        public BiomeType BiomeType;
        public bool Unlocked = false;

        public BiomeReference(BiomeSO biomeSo, QuestSO requirementQuest, BiomeType biomeType, bool unlocked)
        {
            BiomeSO = biomeSo;
            BiomeType = biomeType;
            Unlocked = unlocked;
            RequirementQuest = requirementQuest;
        }
    }
}