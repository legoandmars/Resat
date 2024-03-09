using Resat.Dialogue;
using Resat.Models;
using UnityEngine;

namespace Resat.Npcs
{
    [CreateAssetMenu(fileName = "NPC", menuName = "ScriptableObjects/NPC", order = 1)]
    public class NpcSO : ScriptableObject
    {
        public string Name = "NPC";

        // TODO: Multiple dialogue support
        public DialogueSO? Dialogue;
    }
}