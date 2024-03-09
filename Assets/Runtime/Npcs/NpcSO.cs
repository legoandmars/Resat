using Resat.Models;
using UnityEngine;

namespace Resat.Npcs
{
    [CreateAssetMenu(fileName = "NPC", menuName = "ScriptableObjects/NPC", order = 1)]
    public class NpcSO : ScriptableObject
    {
        public string Name = "NPC";
        /*
        public BiomeType BiomeType;
        public Color DialogueBoxColor = new(0,0,0,1);
        public Color SkyboxTopColor = new Color(1, 1, 1, 1);
        public Color SkyboxBottomColor = new Color(1, 1, 1, 1);
        public Color LightingColor = new Color(1, 1, 1, 1);*/
    }
}