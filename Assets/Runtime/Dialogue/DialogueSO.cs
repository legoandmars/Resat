using System.Collections.Generic;
using Resat.Models;
using UnityEngine;

namespace Resat.Dialogue
{
    [CreateAssetMenu(fileName = "Dialogue", menuName = "ScriptableObjects/Dialogue", order = 1)]
    public class DialogueSO : ScriptableObject
    {
        public List<string> Dialogue = new();
        public bool ShowInteractPromptAfterDialogueComplete = true;
    }
}