using System.Collections.Generic;
using UnityEngine;

namespace Resat.Behaviours
{
    public class CityMountainBehaviour : MonoBehaviour
    {
        [SerializeField]
        private Transform _playerTransform = null!;
        
        [SerializeField]
        private List<GameObject> _activeWhenPlayerLow;

        [SerializeField]
        private List<GameObject> _activeWhenPlayerHigh;
        
        private void Update()
        {
            foreach (var gameObject in _activeWhenPlayerHigh)
            {
                gameObject.SetActive(_playerTransform.position.y > 16f);
            }
            foreach (var gameObject in _activeWhenPlayerLow)
            {
                gameObject.SetActive(_playerTransform.position.y <= 16f);
            }
        }
    }
}