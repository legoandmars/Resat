using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RendererUtils;

namespace Resat.Rendering
{
    public class ForceSaturationBuffer : MonoBehaviour
    {
        private static readonly int ForceSaturationMask = Shader.PropertyToID("_ForceSaturationMask");
        
        // TODO: Add ability to like, disable these, changing the list doesn't remove the serialized DrawRenderer calls
        [SerializeField]
        private List<Renderer> _forceSaturateRenderers = new();

        [SerializeField]
        private List<Renderer> _forceDesaturateRenderers = new();

        [SerializeField]
        private Camera? _camera;

        // used to render the actual renderer
        [SerializeField]
        private Material? _forceSaturateMaterial;

        [SerializeField]
        private Material? _forceDesaturateMaterial;

        [SerializeField]
        private CameraEvent _cameraEvent = CameraEvent.AfterEverything;

        private CommandBuffer? _commandBuffer;
        
        private void Awake()
        {
            CreateCommandBuffer();
        }
        
        private void CreateCommandBuffer()
        {
            if (_camera == null)
                return;

            if (_commandBuffer != null)
            {
                _camera.RemoveCommandBuffer(_cameraEvent, _commandBuffer);
                _commandBuffer.Dispose();
            }

            if (_forceSaturateRenderers.Count == 0 && _forceDesaturateRenderers.Count == 0)
            {
                Debug.Log("Force saturation buffer is currently unnecessary, not creating...");
                return;
            }
            
            var commandBuffer = new CommandBuffer();
            commandBuffer.name = "Render force saturated objects";
            commandBuffer.GetTemporaryRT(ForceSaturationMask, -1, -1, 0, FilterMode.Bilinear);
            commandBuffer.SetRenderTarget(ForceSaturationMask);
            commandBuffer.ClearRenderTarget(true, true, Color.black);

            foreach (var maskedRenderer in _forceSaturateRenderers)
            {
                commandBuffer.DrawRenderer(maskedRenderer, _forceSaturateMaterial);
            }
            foreach (var maskedRenderer in _forceDesaturateRenderers)
            {
                commandBuffer.DrawRenderer(maskedRenderer, _forceDesaturateMaterial);
            }

            _camera.AddCommandBuffer(_cameraEvent, commandBuffer);

        }

        public void AddRenderers(List<Renderer> renderers, bool resaturate)
        {
            // TODO: Dedupe
            if (resaturate)
            {
                _forceSaturateRenderers.AddRange(renderers);
            }
            else
            {
                _forceDesaturateRenderers.AddRange(renderers);
            }
            
            CreateCommandBuffer();
        }

        private void RemoveRenderers(List<Renderer> renderers, bool resaturate)
        {
            if (resaturate)
            {
                _forceSaturateRenderers = _forceSaturateRenderers.Except(renderers).ToList();
            }
            else
            {
                _forceDesaturateRenderers = _forceDesaturateRenderers.Except(renderers).ToList();
            }
            
            CreateCommandBuffer();
        }
    }
}