using UnityEngine;
using UnityEngine.UI;

namespace Resat.UI
{
    public class TextureDebugPanel : MonoBehaviour
    {
        private static readonly int AnimationPercent = Shader.PropertyToID("_AnimationPercent");
        private static readonly int AnimationState = Shader.PropertyToID("_AnimationState");

        [SerializeField]
        public RawImage? CameraImage;

        [SerializeField]
        private Material _material;
        
        public void SetPreviewTexture(RenderTexture renderTexture)
        {
            if (CameraImage == null)
                return;
            
            CameraImage.texture = renderTexture;
        }
        
        public void SetAnimationPercent(float animationPercent)
        {
            if (_material == null)
                return;

            _material.SetFloat(AnimationPercent, animationPercent);
        }
        
        public void SetAnimationState(bool state)
        {
            if (_material == null)
                return;

            _material.SetFloat(AnimationState, state ? 1f : 0f);
        }

        void Start()
        {
            if (_material == null)
                return;

            _material.SetFloat(AnimationState, 0f);
        }
    }
}