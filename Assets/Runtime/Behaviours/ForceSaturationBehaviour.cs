using System.Collections.Generic;
using UnityEngine;

namespace Resat.Behaviours
{
    public class ForceSaturationBehaviour : MonoBehaviour
    {
        public List<Renderer> Renderers => _renderers;
        public bool Resaturate => _resaturate;
        
        [SerializeField]
        private List<Renderer> _renderers = new();
        
        // true if saturated, false if desat
        // should be an enum but no time
        [Header("True if force saturated, false if force desaturated")]
        [SerializeField]
        private bool _resaturate = true;

        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }
    }
}