using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class BodyDepthMaskFeature : ScriptableRendererFeature
{
    class BodyDepthMaskPass : ScriptableRenderPass
    {
        private FilteringSettings filteringSettings;
        private ShaderTagId shaderTagId = new ShaderTagId("UniversalForward");

        public BodyDepthMaskPass(LayerMask layerMask)
        {
            filteringSettings = new FilteringSettings(RenderQueueRange.all, layerMask);
            renderPassEvent = RenderPassEvent.BeforeRenderingOpaques; // 👈 antes de todo
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var sortFlags = renderingData.cameraData.defaultOpaqueSortFlags;
            var drawSettings = CreateDrawingSettings(shaderTagId, ref renderingData, sortFlags);

            // Forzar a no escribir color
            drawSettings.overrideMaterial = new Material(Shader.Find("Hidden/DepthOnlyColorMask0"));

            context.DrawRenderers(renderingData.cullResults, ref drawSettings, ref filteringSettings);
        }
    }

    [System.Serializable]
    public class BodyDepthMaskSettings
    {
        public LayerMask layerMask; // 👈 pon aquí el "Body" Layer
    }

    public BodyDepthMaskSettings settings = new BodyDepthMaskSettings();
    BodyDepthMaskPass bodyDepthMaskPass;

    public override void Create()
    {
        bodyDepthMaskPass = new BodyDepthMaskPass(settings.layerMask);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(bodyDepthMaskPass);
    }
}
