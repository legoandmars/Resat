using System;
using Resat.Biomes;
using Resat.Models;
using Resat.Models.Events;
using UnityEngine;

namespace Resat.Intermediates
{
    public class CameraIntermediate : MonoBehaviour
    {
        public event Action? PhotoTaken;
        
        public void TakePhoto()
        {
            PhotoTaken?.Invoke();
        }
    }
}