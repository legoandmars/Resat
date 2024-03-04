using System;
using UnityEngine;

namespace Resat.Models
{
    [Serializable]
    public class CameraResolutionData
    {
        // Defaults are for a ~4:3 image at 1080p
        public Vector2Int Resolution = new (1088, 800);
        public Vector2Int NativeResolution = new (1920, 1080);
        public Vector2 Center = new Vector2(0.5f, 0.5f);
        public FilterMode FilterMode = FilterMode.Bilinear;
        public RenderTextureReadWrite RenderTextureReadWrite = RenderTextureReadWrite.Default;
        
        public Vector2 Scale
        {
            get
            {
                float scaleX = (float)Resolution.x / NativeResolution.x;
                float scaleY = (float)Resolution.y / NativeResolution.y;

                return new Vector2(scaleX, scaleY);
            }
        }

        public Vector2 Offset => new Vector2((1 - Scale.x) * Center.x, (1 - Scale.y) * Center.y);
        
        // used to calculate camera "fake" aspect ratio
        public float NativeAspectRatio => (float) NativeResolution.x / (float) NativeResolution.y;
        public float AspectRatio => (float) Resolution.x / (float) Resolution.y;
        
        public CameraResolutionData()
        {
        }
        
        public CameraResolutionData(Vector2Int resolution, Vector2Int nativeResolution, Vector2 center, FilterMode? filterMode = null, RenderTextureReadWrite? renderTextureReadWrite = null)
        {
            Resolution = resolution;
            NativeResolution = nativeResolution;
            Center = center;
            
            if (filterMode != null)
                FilterMode = filterMode.Value;
            
            if (renderTextureReadWrite != null)
                RenderTextureReadWrite = renderTextureReadWrite.Value;
        }

        public Vector2 GetNativeResolutionScale(Vector2 newNativeResolution) => NativeResolution / newNativeResolution;

        public Vector2 GetRescaledResolution(Vector2 newNativeResolution)
        {
            var scale = GetNativeResolutionScale(newNativeResolution);
            return new Vector2(Resolution.x / scale.x, Resolution.y / scale.y);
        }
    }
}