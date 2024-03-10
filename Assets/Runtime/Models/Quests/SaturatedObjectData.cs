using System;
using Resat.Behaviours;

namespace Resat.Models
{
    [Serializable]
    public struct SaturatedObjectData
    {
        public ForceSaturationBehaviour? SaturatedObject;
        public ForceSaturationBehaviour? PermanentObjectWhenComplete;
    }
}