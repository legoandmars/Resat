using System.Collections.Generic;
using UnityEngine;

namespace Resat.Quests
{
    public class QuestController : MonoBehaviour
    {
        [SerializeField]
        public List<QuestReference> QuestReferences = new();

        private void Awake()
        {
            // start quest
        }
    }
}