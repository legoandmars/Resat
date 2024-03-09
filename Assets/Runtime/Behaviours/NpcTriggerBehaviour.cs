using Resat.Npcs;
using UnityEngine;

namespace Resat.Behaviours
{
    // essentially just a dummy
    public class NpcTriggerBehaviour : MonoBehaviour
    {
        public NpcSO? NpcSO => _npcSO;
        
        [SerializeField]
        private NpcSO? _npcSO;
    }
}