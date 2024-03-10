using System.Collections.Generic;
using Resat.Behaviours;
using Resat.Dialogue;
using Resat.Models;
using UnityEngine;

namespace Resat.Quests
{
    [CreateAssetMenu(fileName = "Quest", menuName = "ScriptableObjects/Quest", order = 1)]
    public class QuestSO : ScriptableObject
    {
        public string Name = "Quest";
        public QuestType QuestType;
        // public List<SaturatedObjectData> SaturatedObjects = new(); // may be unused depending on quest type, but unity inspector will make nullable here not do anything
    }
}