using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using Resat.Models;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

namespace Resat.Cameras
{
    // Used for actually interacting with a camera and directly taking renders/photos
    public class ResatCamera : MonoBehaviour
    {
        public Camera? Camera;
        
        private Dictionary<Vector2Int, RenderTexture> _renderTexturesByResolution = new();

        // TODO: Prealloc RT method
        
        // Usecases:
        // 1920x1080 -> 1080x800 (centered, 1080x1080 fine)
        // 1920x1080 -> 1080x800 (offcenter, 1080x1080 fine)
        // 3840x2160 -> 2160x1600 (centered, highres screenshot, need to render directly to 2160x1600 texture)
        public RenderTexture? Render(CameraResolutionData resolutionData)
        {
            if (Camera == null)
                return null;
         
            var nativeRenderTexture = GetCachedRenderTexture(resolutionData.NativeResolution, resolutionData.FilterMode, resolutionData.RenderTextureReadWrite);
            var renderTexture = GetCachedRenderTexture(resolutionData.Resolution, resolutionData.FilterMode, resolutionData.RenderTextureReadWrite);

            // Do the initial render at native res
            Camera.targetTexture = nativeRenderTexture;
            Camera.Render();

            // Scale down to intended resolution and return
            Graphics.Blit(nativeRenderTexture, renderTexture, resolutionData.Scale, resolutionData.Offset);
            return renderTexture;
        }

        public void RenderScreenshot(CameraResolutionData resolutionData)
        {
            Debug.Log("Taking a screenshot!");
            string directory = Path.Join(Application.persistentDataPath, "Photos");
            Directory.CreateDirectory(directory);

            string path = Path.Join(directory, $"{GetUnixTimestamp()}.png");
            var renderTexture = Render(resolutionData);
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
        
        private RenderTexture GetCachedRenderTexture(Vector2Int resolution, FilterMode filterMode, RenderTextureReadWrite renderTextureReadWrite)
        {
            if (_renderTexturesByResolution.TryGetValue(resolution, out RenderTexture renderTexture))
                return renderTexture;

            renderTexture = CreateTextureForCamera(resolution, filterMode, renderTextureReadWrite);
            _renderTexturesByResolution.Add(resolution, renderTexture);

            return renderTexture;
        }
        
        private RenderTexture CreateTextureForCamera(Vector2Int resolution, FilterMode filterMode, RenderTextureReadWrite renderTextureReadWrite)
        {
            Debug.Log($"Allocating new RenderTexture for camera with resolution {resolution}.");
            var renderTexture = new RenderTexture(resolution.x, resolution.y, 0, RenderTextureFormat.Default, renderTextureReadWrite);
            renderTexture.enableRandomWrite = true;
            renderTexture.filterMode = filterMode;
            
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
