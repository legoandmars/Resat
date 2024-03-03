using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Resat.Cameras
{
    public class ResatCamera : MonoBehaviour
    {
        
        public Camera? Camera;

        [SerializeField]
        private Vector2Int _baseResolution = new Vector2Int(1920, 1080);
        
        private Dictionary<Vector2Int, RenderTexture> _renderTexturesByResolution = new();

        // Usecases:
        // 1920x1080 -> 1080x800 (centered, 1080x1080 fine)
        // 1920x1080 -> 1080x800 (offcenter, 1080x1080 fine)
        // 3840x2160 -> 2160x1600 (centered, highres screenshot, need to render directly to 2160x1600 texture)
        public RenderTexture? Render(Vector2Int resolution, Vector2Int nativeResolution, Vector2? center = null, bool usePointFiltering = false)
        {
            if (Camera == null)
                return null;
         
            if (center == null)
                center = new Vector2(0.5f, 0.5f);

            // calculate scale and offset
            float scaleX = (float) resolution.x / nativeResolution.x;
            float scaleY = (float) resolution.y / nativeResolution.y;
            Vector2 offset = new Vector2((1 - scaleX) * center.Value.x, (1 - scaleY) * center.Value.y);
            
            var nativeRenderTexture = GetCachedRenderTexture(nativeResolution, usePointFiltering);
            var renderTexture = GetCachedRenderTexture(resolution, usePointFiltering);

            // Do the initial render at native res
            Camera.targetTexture = nativeRenderTexture;
            Camera.Render();

            // Scale down to intended resolution and return
            Graphics.Blit(nativeRenderTexture, renderTexture, new Vector2((float)resolution.x / nativeResolution.x, (float)resolution.y / nativeResolution.y), offset);
            return renderTexture;
        }

        private RenderTexture GetCachedRenderTexture(Vector2Int resolution, bool usePointFiltering = false)
        {
            if (_renderTexturesByResolution.TryGetValue(resolution, out RenderTexture renderTexture))
                return renderTexture;

            renderTexture = CreateCameraForTexture(resolution, usePointFiltering);
            _renderTexturesByResolution.Add(resolution, renderTexture);

            return renderTexture;
        }
        
        public RenderTexture CreateCameraForTexture(Vector2Int resolution, bool usePointFiltering = false)
        {
            var renderTexture = new RenderTexture(resolution.x, resolution.y, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);
            renderTexture.enableRandomWrite = true;
            
            if (usePointFiltering)
                renderTexture.filterMode = FilterMode.Point;
            
            renderTexture.Create();

            return renderTexture;
        }

        void OnEnable()
        {
            if (Camera != null)
                Camera.enabled = false;
        }
        void OnDisable()
        {
            foreach (var renderTexture in _renderTexturesByResolution.Values)
            {
                renderTexture.Release();
            }
            
            _renderTexturesByResolution.Clear();
        }
    }
}
