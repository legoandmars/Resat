using System;
using Resat.Behaviours;
using Resat.Npcs;

namespace Resat.Models
{
    [Serializable]
    public struct SaturatedObjectData
    {
        public NpcSO? AssignedNPC; // yes, we use NPCs to pick up objects
        public ForceSaturationBehaviour? SaturatedObject;
        public ForceSaturationBehaviour? PermanentObjectWhenComplete;
    }
}