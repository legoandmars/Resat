using Resat.Dialogue;
using Resat.Models;
using UnityEngine;

namespace Resat.Npcs
{
    [CreateAssetMenu(fileName = "NPC", menuName = "ScriptableObjects/NPC", order = 1)]
    public class NpcSO : ScriptableObject
    {
        public string Name = "NPC";

        public DialogueSO? InitialDialogue;
    }
}