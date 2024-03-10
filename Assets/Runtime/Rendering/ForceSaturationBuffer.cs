using System;
using System.Collections.Generic;
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
        
        private void Awake()
        {
            if (_camera == null)
                return;

            var commandBuffer = new CommandBuffer();
            commandBuffer.name = "Render force saturated objects";
            
            commandBuffer.GetTemporaryRT(ForceSaturationMask, -1, -1, 0, FilterMode.Bilinear);
            commandBuffer.SetRenderTarget(ForceSaturationMask);
            commandBuffer.ClearRenderTarget(true, true, Color.black);

            foreach (var maskedRenderer in _forceSaturateRenderers)
            {
                if (maskedRenderer.gameObject == null) 
                    continue;
                
                commandBuffer.DrawRenderer(maskedRenderer, _forceSaturateMaterial);
            }

            foreach (var maskedRenderer in _forceDesaturateRenderers)
            {
                if (maskedRenderer.gameObject == null) 
                    continue;
                
                commandBuffer.DrawRenderer(maskedRenderer, _forceDesaturateMaterial);
            }

            var list = new RendererList();
            
            _camera.AddCommandBuffer(_cameraEvent, commandBuffer);
        }
    }
}