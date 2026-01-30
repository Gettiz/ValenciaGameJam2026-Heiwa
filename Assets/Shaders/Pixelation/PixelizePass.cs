using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;

public class PixelizePass : ScriptableRenderPass
{
    private PixelizeFeature.CustomPassSettings settings;
    private Material material;

    private class PassData
    {
        public TextureHandle colorBuffer;
        public TextureHandle pixelBuffer;
        public Material material;
    }

    public PixelizePass(PixelizeFeature.CustomPassSettings settings)
    {
        this.settings = settings;
        this.renderPassEvent = settings.renderPassEvent;
        
        // IMPORTANT: Tell URP that this pass needs the camera color texture
        ConfigureInput(ScriptableRenderPassInput.Color);
        
        if (material == null) material = CoreUtils.CreateEngineMaterial("Hidden/Pixelize");
    }

    public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
    {
        UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
        UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();

        // Ensure we have a valid target
        TextureHandle colorBuffer = resourceData.activeColorTexture;
        if (!colorBuffer.IsValid()) return;

        // Pixelation Math
        int pixelHeight = settings.screenHeight;
        int pixelWidth = Mathf.RoundToInt(pixelHeight * cameraData.camera.aspect);

        // Update material
        material.SetVector("_BlockCount", new Vector2(pixelWidth, pixelHeight));
        material.SetVector("_BlockSize", new Vector2(1.0f / pixelWidth, 1.0f / pixelHeight));
        material.SetVector("_HalfBlockSize", new Vector2(0.5f / pixelWidth, 0.5f / pixelHeight));

        // Create the descriptor for the small buffer
        TextureDesc desc = new TextureDesc(pixelWidth, pixelHeight);
        desc.format = cameraData.cameraTargetDescriptor.graphicsFormat;
        desc.filterMode = FilterMode.Point;
        desc.name = "PixelBuffer";

        TextureHandle pixelBuffer = renderGraph.CreateTexture(desc);

        // --- Pass 1: Downsample (Camera -> Pixel Buffer) ---
        using (var builder = renderGraph.AddRasterRenderPass<PassData>("Pixelize Downsample", out var passData))
        {
            passData.colorBuffer = colorBuffer;
            passData.pixelBuffer = pixelBuffer;
            passData.material = material;

            builder.UseTexture(passData.colorBuffer, AccessFlags.Read);
            builder.SetRenderAttachment(passData.pixelBuffer, 0, AccessFlags.Write);

            builder.SetRenderFunc((PassData data, RasterGraphContext context) => {
                // Unity 6: Blitter automatically binds data.colorBuffer to _BlitTexture
                Blitter.BlitTexture(context.cmd, data.colorBuffer, new Vector4(1, 1, 0, 0), data.material, 0);
            });
        }

        // --- Pass 2: Upscale (Pixel Buffer -> Camera) ---
        using (var builder = renderGraph.AddRasterRenderPass<PassData>("Pixelize Upscale", out var passData))
        {
            passData.colorBuffer = colorBuffer;
            passData.pixelBuffer = pixelBuffer;

            builder.UseTexture(passData.pixelBuffer, AccessFlags.Read);
            builder.SetRenderAttachment(passData.colorBuffer, 0, AccessFlags.Write);

            builder.SetRenderFunc((PassData data, RasterGraphContext context) => {
                // 'false' here prevents blurring during upscale
                Blitter.BlitTexture(context.cmd, data.pixelBuffer, new Vector4(1, 1, 0, 0), 0, false);
            });
        }
    }
}