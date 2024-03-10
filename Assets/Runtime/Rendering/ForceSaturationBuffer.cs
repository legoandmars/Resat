using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Resat.Rendering
{
    public class ForceSaturationBuffer : MonoBehaviour
    {
        private static readonly int ForceSaturationMask = Shader.PropertyToID("_ForceSaturationMask");
        
        [SerializeField]
        private List<Renderer> _maskedRenderers = new();

        [SerializeField]
        private Camera? _camera;

        // used to render the actual renderer
        [SerializeField]
        private Material? _maskMaterial;

        [SerializeField]
        private CameraEvent _cameraEvent = CameraEvent.AfterEverything;
        
        private void Awake()
        {
            if (_camera == null)
                return;

            var commandBuffer = new CommandBuffer();
            commandBuffer.name = "Render force saturated objects";
            
            commandBuffer.GetTemporaryRT(ForceSaturationMask, -1, -1, 24, FilterMode.Bilinear);
            commandBuffer.SetRenderTarget(ForceSaturationMask);
            commandBuffer.ClearRenderTarget(true, true, Color.black);

            foreach (var maskedRenderer in _maskedRenderers)
            {
                if (maskedRenderer.gameObject == null) 
                    continue;
                
                commandBuffer.DrawRenderer(maskedRenderer, _maskMaterial);
            }
            
            commandBuffer.SetGlobalTexture(ForceSaturationMask, ForceSaturationMask);
            
            _camera.AddCommandBuffer(_cameraEvent, commandBuffer);
        }
    }
}