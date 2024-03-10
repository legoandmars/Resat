using System;
using Resat.Behaviours;
using Resat.Npcs;

namespace Resat.Models
{
    [Serializable]
    public class SaturatedObjectData
    {
        public NpcSO? AssignedNPC; // yes, we use NPCs to pick up objects
        public bool Collected;
        public ForceSaturationBehaviour? SaturatedObject;
        public ForceSaturationBehaviour? PermanentObjectWhenComplete;
    }
}