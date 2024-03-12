using System.Collections.Generic;
using Resat.Models;
using Resat.Quests;
using UnityEngine;

namespace Resat.Dialogue
{
    [CreateAssetMenu(fileName = "Dialogue", menuName = "ScriptableObjects/Dialogue", order = 1)]
    public class DialogueSO : ScriptableObject
    {
        [TextArea(5, 3)]
        public List<string> Dialogue = new();
        public bool ShowInteractPromptAfterDialogueComplete = true;
        public bool StopShowingDialogueAfterwards = false;
        public DialogueSO? NextDialogue;
        public QuestSO? QuestRequirementToPass; // will be force passed after this quest is completed
    }
}