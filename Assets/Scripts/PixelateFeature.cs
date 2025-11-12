using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;



public class PixelateFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class PixelateSettings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRendering;
        public Material material;
        [Range(1, 64)] public float pixelSize = 4f;

        public string cameraTag = "Pixelated";
    }

    class PixelatePass : ScriptableRenderPass
    {
        private PixelateSettings settings;
        private RenderTargetIdentifier source;
        private RenderTargetHandle tempTexture;
        private string profilerTag;

        public PixelatePass(string tag, PixelateSettings settings)
        {
            profilerTag = tag;
            this.settings = settings;
            tempTexture.Init("_PixelateTempTex");
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            source = renderingData.cameraData.renderer.cameraColorTarget;

            var desc = renderingData.cameraData.cameraTargetDescriptor;
            desc.depthBufferBits = 0;

            cmd.GetTemporaryRT(tempTexture.id, desc);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (settings.material == null)
                return;


            CommandBuffer cmd = CommandBufferPool.Get(profilerTag);

            settings.material.SetFloat("_PixelSize", settings.pixelSize);

            cmd.Blit(source, tempTexture.Identifier(), settings.material);
            cmd.Blit(tempTexture.Identifier(), source);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            if (cmd == null) return;
            cmd.ReleaseTemporaryRT(tempTexture.id);
        }
    }

    public PixelateSettings settings = new PixelateSettings();
    PixelatePass pixelatePass;

    public override void Create()
    {
        pixelatePass = new PixelatePass("PixelatePass", settings);
        pixelatePass.renderPassEvent = settings.renderPassEvent;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (settings.material == null)
            return;

        renderer.EnqueuePass(pixelatePass);
    }
}

