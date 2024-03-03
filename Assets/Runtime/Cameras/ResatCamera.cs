using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

namespace Resat.Cameras
{
    public class ResatCamera : MonoBehaviour
    {
        public Camera? Camera;
        
        private Dictionary<Vector2Int, RenderTexture> _renderTexturesByResolution = new();

        // Usecases:
        // 1920x1080 -> 1080x800 (centered, 1080x1080 fine)
        // 1920x1080 -> 1080x800 (offcenter, 1080x1080 fine)
        // 3840x2160 -> 2160x1600 (centered, highres screenshot, need to render directly to 2160x1600 texture)
        public RenderTexture? Render(Vector2Int resolution, Vector2Int nativeResolution, Vector2? center = null, bool usePointFiltering = false, bool srgb = false)
        {
            if (Camera == null)
                return null;
         
            if (center == null)
                center = new Vector2(0.5f, 0.5f);

            // calculate scale and offset
            float scaleX = (float) resolution.x / nativeResolution.x;
            float scaleY = (float) resolution.y / nativeResolution.y;
            Vector2 offset = new Vector2((1 - scaleX) * center.Value.x, (1 - scaleY) * center.Value.y);
            
            var nativeRenderTexture = GetCachedRenderTexture(nativeResolution, usePointFiltering, srgb);
            var renderTexture = GetCachedRenderTexture(resolution, usePointFiltering, srgb);

            // Do the initial render at native res
            Camera.targetTexture = nativeRenderTexture;
            Camera.Render();

            // Scale down to intended resolution and return
            Graphics.Blit(nativeRenderTexture, renderTexture, new Vector2((float)resolution.x / nativeResolution.x, (float)resolution.y / nativeResolution.y), offset);
            return renderTexture;
        }

        public void RenderScreenshot(Vector2Int resolution, Vector2Int nativeResolution, Vector2? center = null, bool usePointFiltering = false)
        {
            Debug.Log("Taking a screenshot!");
            string directory = Path.Join(Application.persistentDataPath, "Photos");
            Directory.CreateDirectory(directory);

            string path = Path.Join(directory, $"{GetUnixTimestamp()}.png");
            var renderTexture = Render(resolution, nativeResolution, center, usePointFiltering, true);
            if (renderTexture == null)
                return;
            
            SaveRenderTexture(renderTexture, path);
        }
        
        private void SaveRenderTexture(RenderTexture renderTexture, string path)
        {
            AsyncGPUReadback.Request(renderTexture, 0, TextureFormat.RGBA32, (request) =>
            {
                if (request.hasError)
                {
                    Debug.Log("Saving render texture failed.");
                    return;
                }
                
                var imageBytes = request.GetData<byte>().ToArray();
                SaveImageByteArrayOnNewThread(imageBytes, path, new Vector2Int(renderTexture.width, renderTexture.height), renderTexture.graphicsFormat).Forget();
            });
        }

        private async UniTask SaveImageByteArrayOnNewThread(byte[] imageBytes, string path, Vector2Int resolution, GraphicsFormat graphicsFormat)
        {
            await UniTask.SwitchToThreadPool();
            
            await File.WriteAllBytesAsync(path,ImageConversion.EncodeArrayToPNG(imageBytes, graphicsFormat, (uint)resolution.x, (uint)resolution.y));
        }
        
        public static long GetUnixTimestamp()
        {
            return (long)(DateTime.UtcNow.ToUniversalTime().Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;
        }
        
        private RenderTexture GetCachedRenderTexture(Vector2Int resolution, bool usePointFiltering = false, bool srgb = false)
        {
            if (_renderTexturesByResolution.TryGetValue(resolution, out RenderTexture renderTexture))
                return renderTexture;

            renderTexture = CreateTextureForCamera(resolution, usePointFiltering, srgb);
            _renderTexturesByResolution.Add(resolution, renderTexture);

            return renderTexture;
        }
        
        public RenderTexture CreateTextureForCamera(Vector2Int resolution, bool usePointFiltering = false, bool srgb = false)
        {
            var renderTexture = new RenderTexture(resolution.x, resolution.y, 0, RenderTextureFormat.Default, srgb ? RenderTextureReadWrite.sRGB : RenderTextureReadWrite.Linear);
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
