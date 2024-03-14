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
        
        // *shrug*
        public float XOffsetMultiplier = 4.8f;
        public float YOffsetMultiplier = 4.8f;

        private Vector4? _lastResolutionDataScaleAndOffset;
        private float? _lastFieldOfView;
        private bool _needsProjectionMatrixRecalculation = true;
        
        // TODO: Prealloc RT method

        public void SetFieldOfView(float fieldOfView)
        {
            if (Camera == null || _lastFieldOfView == fieldOfView)
                return;

            _lastFieldOfView = fieldOfView;
            Camera.fieldOfView = fieldOfView;
            _needsProjectionMatrixRecalculation = true;
        }
        
        // Usecases:
        // 1920x1080 -> 1080x800 (centered, 1080x1080 fine)
        // 1920x1080 -> 1080x800 (offcenter, 1080x1080 fine)
        // 3840x2160 -> 2160x1600 (centered, highres screenshot, need to render directly to 2160x1600 texture)
        public RenderTexture? Render(CameraResolutionData resolutionData, bool forceCreateNewTexture = false)
        {
            if (Camera == null)
                return null;

            Vector4 resolutionDataScaleAndOffset = resolutionData.RatioAndOffsetVector;
            if (_lastResolutionDataScaleAndOffset == null || _lastResolutionDataScaleAndOffset != resolutionDataScaleAndOffset || _needsProjectionMatrixRecalculation)
            {
                _lastResolutionDataScaleAndOffset = resolutionDataScaleAndOffset;
                _needsProjectionMatrixRecalculation = false;
                RecalculateProjectionMatrix(Camera, resolutionData);
            }

            RenderTexture renderTexture;

            if (forceCreateNewTexture)
                renderTexture = CreateTextureForCamera(resolutionData.Resolution, resolutionData.FilterMode, resolutionData.RenderTextureReadWrite);
            else
                renderTexture = GetCachedRenderTexture(resolutionData.Resolution, resolutionData.FilterMode, resolutionData.RenderTextureReadWrite);

            // Do the initial render at native res
            Camera.targetTexture = renderTexture;
            Camera.Render();

            // Scale down to intended resolution and return
            return renderTexture;
        }

        private void RecalculateProjectionMatrix(Camera camera, CameraResolutionData resolutionData)
        {
            Debug.Log("Recalculating projection matrix...");
            
            camera.ResetProjectionMatrix();
            camera.projectionMatrix = CalculateZoomedProjectionMatrix(camera, resolutionData);
        }
        
        public Matrix4x4 CalculateZoomedProjectionMatrix(Camera camera, CameraResolutionData resolutionData)
        {
            // IMPORTANT TODO: Scaling native resolution X does NOT work properly! Any non 16:9 values will probably not work right
            float heightRatio =  (float)resolutionData.NativeResolution.y / (float)resolutionData.Resolution.y;
            
            // no idea how exactly this works, or why it's necessary, but we are jamming
            float amount = Mathf.Tan((camera.fieldOfView / 2f) * Mathf.Deg2Rad) * camera.nearClipPlane / heightRatio;

            // 0 = -xOffset
            // 0.5 = 0
            // 1 = xOffset
            float xOffsetMax = amount * XOffsetMultiplier;
            float yOffsetMax = amount * YOffsetMultiplier;
            
            // calc center
            float xOffset = (resolutionData.Center.x - 0.5f) * xOffsetMax;
            float yOffset = (resolutionData.Center.y - 0.5f) * yOffsetMax / resolutionData.NativeAspectRatio;

            // Matrix4x4 scaledMatrix = Matrix4x4.Scale(new Vector3(1 / resolutionData.Scale.x, 1 / resolutionData.Scale.y, 1));
            Matrix4x4 offsetMatrix = PerspectiveOffCenter((-amount * resolutionData.AspectRatio) + xOffset, (amount * resolutionData.AspectRatio) + xOffset, -amount + yOffset, amount + yOffset, camera.nearClipPlane, camera.farClipPlane);
            
            return offsetMatrix;
        }

        // https://forum.unity.com/threads/how-to-render-just-part-of-a-cams-view-to-the-full-screen.6486/
        private Matrix4x4 PerspectiveOffCenter(float left, float right, float bottom, float top, float near, float far)
        {
            float x = (2.0f * near) / (right - left);
            float y = (2.0f * near) / (top - bottom);
            float a = (right + left) / (right - left);
            float b = (top + bottom) / (top - bottom);
            float c = -(far + near) / (far - near);
            float d = -(2.0f * far * near) / (far - near);
            float e = -1.0f;

            var m = new Matrix4x4();
            m[0,0] = x; m[0,1] = 0; m[0,2] = a; m[0,3] = 0;
            m[1,0] = 0; m[1,1] = y; m[1,2] = b; m[1,3] = 0;
            m[2,0] = 0; m[2,1] = 0; m[2,2] = c; m[2,3] = d;
            m[3,0] = 0; m[3,1] = 0; m[3,2] = e; m[3,3] = 0;
            return m;
        }
        
        public RenderTexture? RenderScreenshot(CameraResolutionData resolutionData, bool saveToDisk)
        {
            Debug.Log("Taking a screenshot!");
            string directory = Path.Join(Application.persistentDataPath, "Photos");
            Directory.CreateDirectory(directory);

            string path = Path.Join(directory, $"{GetUnixTimestamp()}.png");
            var renderTexture = Render(resolutionData, true);
            if (renderTexture == null)
                return null;

            if (saveToDisk)
            {
                SaveRenderTexture(renderTexture, path);
            }

            return renderTexture;
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

        public void RemoveTextureFromCache(RenderTexture renderTexture)
        {
            var resolution = new Vector2Int(renderTexture.width, renderTexture.height);
            if (_renderTexturesByResolution.ContainsKey(resolution))
            {
                _renderTexturesByResolution.Remove(resolution);
            }
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
