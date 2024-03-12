using System;
using UnityEngine;

namespace Resat.Behaviours
{
    public class ExclamationPointBehaviour : ForceSaturationBehaviour
    {
        public Camera? PlayerCamera => _playerCamera; 
        
        [SerializeField]
        private Transform? _exclamationPointVisuals;

        [SerializeField]
        private Camera? _playerCamera;
        
        private void Update()
        {
            if (_exclamationPointVisuals == null || _playerCamera == null)
                return;

            _exclamationPointVisuals.forward = _playerCamera.transform.forward;
        }
    }
}