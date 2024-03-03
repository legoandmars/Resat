using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Resat
{
    public class DesaturationCamera : MonoBehaviour
    {
        private static readonly int ScreenResolution = Shader.PropertyToID("_ScreenResolution");
        private static readonly int CutoutOffsetAndScale = Shader.PropertyToID("_CutoutOffsetAndScale");

        [SerializeField]
        private Material? _material;

        private Vector2? _resolution;

        public void SetResolution(Vector2Int resolution, Vector2Int nativeResolution, Vector2 center)
        {
            if (_material == null)
                return;

            // calculate scale and offset
            float scaleX = (float) resolution.x / nativeResolution.x;
            float scaleY = (float) resolution.y / nativeResolution.y;
            
            _material.SetVector(CutoutOffsetAndScale, new Vector4(scaleX, scaleY, center.x, center.y));
        }
        
        private void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            Graphics.Blit(src, dest, _material);
        }

        private void SetScreenResolution()
        {
            if (_material == null)
                return;
            
            var screenResolution = new Vector2(Screen.width, Screen.height);
            if (_resolution == screenResolution)
                return;

            _resolution = screenResolution;
            _material.SetVector(ScreenResolution, screenResolution);
        }
        
        // check every second or so to see if screen needs to be adjusted in-shader
        private async UniTask ScreenResolutionLoop()
        {
            while (enabled)
            {
                await UniTask.Delay(1000);
                
                SetScreenResolution();
            }
        }
        
        void Start()
        {
            SetScreenResolution();
        }

        void OnEnable()
        {
            ScreenResolutionLoop().AttachExternalCancellation(this.GetCancellationTokenOnDestroy()).Forget();
        }
    }
}
