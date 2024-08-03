using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DitheringPass : ScriptableRenderPass
{
    static readonly string RenderPassTag = "Dithering";

    private DitheringVolume DitheringVolume;
    private Material DitheringMaterial;

    public DitheringPass(RenderPassEvent evt, Shader dithershader)
    {
        renderPassEvent = evt;
        var shader = dithershader;
        if (shader == null)
        {
            Debug.LogError("No Shader");
            return;
        }
        DitheringMaterial = CoreUtils.CreateEngineMaterial(dithershader);
    }
    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if (DitheringMaterial == null || !renderingData.cameraData.postProcessEnabled)
        {
            if (DitheringMaterial == null) Debug.LogError("No Dithering Material");
            return;
        }

        VolumeStack stack = VolumeManager.instance.stack;
        DitheringVolume = stack.GetComponent<DitheringVolume>();

        if (!DitheringVolume.IsActive())
        {
            return;
        }

        var cmd = CommandBufferPool.Get(RenderPassTag);
        Render(cmd, ref renderingData);
        context.ExecuteCommandBuffer(cmd);

        CommandBufferPool.Release(cmd);
    }
    void Render(CommandBuffer cmd, ref RenderingData renderingData)
    {
        if (DitheringVolume.IsActive() == false) return;
        DitheringVolume.load(DitheringMaterial, ref renderingData);

        var cameraColorTarget = renderingData.cameraData.renderer.cameraColorTarget;
        int downSample = (int) DitheringVolume._downSamples.value;

        // Get the source and destination render textures
        RenderTargetIdentifier source = cameraColorTarget;
        RenderTargetIdentifier destination = cameraColorTarget;

        // Set the material properties (already done in DitheringVolume.load)

        int width = renderingData.cameraData.cameraTargetDescriptor.width;
        int height = renderingData.cameraData.cameraTargetDescriptor.height;

        RenderTextureDescriptor descriptor = renderingData.cameraData.cameraTargetDescriptor;
        descriptor.depthBufferBits = 0;

        RenderTargetIdentifier[] textures = new RenderTargetIdentifier[8];
        RenderTargetIdentifier currentSource = source;

        // Downsample
        for (int i = 0; i < downSample; ++i)
        {
            width /= 2;
            height /= 2;

            if (height < 2)
                break;

            descriptor.width = width;
            descriptor.height = height;

            textures[i] = new RenderTargetIdentifier("_TemporaryRT" + i);
            cmd.GetTemporaryRT(Shader.PropertyToID("_TemporaryRT" + i), descriptor, FilterMode.Point);

            if (i == 0)
            {
                // First pass, blit from the source
                cmd.Blit(currentSource, textures[i], DitheringMaterial, 1);
            }
            else
            {
                // Subsequent passes, blit from the previous result
                cmd.Blit(textures[i - 1], textures[i], DitheringMaterial, 1);
            }

            currentSource = textures[i];
        }

        // Apply dithering
        cmd.GetTemporaryRT(Shader.PropertyToID("_DitherRT"), descriptor, FilterMode.Point);
        RenderTargetIdentifier dither = new RenderTargetIdentifier("_DitherRT");
        cmd.Blit(currentSource, dither, DitheringMaterial, 0);

        // Final blit to the destination
        cmd.Blit(dither, destination, DitheringMaterial, 1);

        // Release temporary render textures
        cmd.ReleaseTemporaryRT(Shader.PropertyToID("_DitherRT"));
        for (int i = 0; i < downSample; ++i)
        {
          cmd.ReleaseTemporaryRT(Shader.PropertyToID("_TemporaryRT" + i));
        }
    }
}
