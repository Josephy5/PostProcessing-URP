using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class LensFlarePass : ScriptableRenderPass
{
    static readonly string renderPassTag = "Lens Flare";

    private LensFlareVolume LensFlareVolume;
    private Material LensFlareMaterial;

    public LensFlarePass(RenderPassEvent evt, Shader LensFlareshader)
    {
        renderPassEvent = evt;
        var shader = LensFlareshader;
        if (shader == null)
        {
            Debug.LogError("No Shader");
            return;
        }
        LensFlareMaterial = CoreUtils.CreateEngineMaterial(LensFlareshader);
    }


    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if (LensFlareMaterial == null)
        {
            Debug.LogError("No Lens Flare Material");
            return;
        }

        if (!renderingData.cameraData.postProcessEnabled || renderingData.cameraData.cameraType == CameraType.SceneView)
        {
            //Debug.LogError("Post Processing in Camera not enabled");
            return;
        }

        VolumeStack stack = VolumeManager.instance.stack;
        LensFlareVolume = stack.GetComponent<LensFlareVolume>();

        var cmd = CommandBufferPool.Get(renderPassTag);
        Render(cmd, ref renderingData);
        context.ExecuteCommandBuffer(cmd);

        CommandBufferPool.Release(cmd);
    }
    private readonly int temporaryRTID = Shader.PropertyToID("_TemporaryRT");
    void Render(CommandBuffer cmd, ref RenderingData renderingData)
    {
        if (LensFlareVolume.IsActive() == false) return;
        LensFlareVolume.load(LensFlareMaterial, ref renderingData);

        var cameraColorTarget = renderingData.cameraData.renderer.cameraColorTarget;
        int width = renderingData.cameraData.cameraTargetDescriptor.width;
        int height = renderingData.cameraData.cameraTargetDescriptor.height;

        RenderTextureDescriptor descriptor = renderingData.cameraData.cameraTargetDescriptor;
        descriptor.depthBufferBits = 0;

        var noise = new RenderTargetIdentifier("_NoiseTex");
        cmd.GetTemporaryRT(Shader.PropertyToID("_NoiseTex"), 256, 64, 0, FilterMode.Bilinear, descriptor.colorFormat);
        cmd.Blit(noise, noise, LensFlareMaterial, 6);

        var downsample = new RenderTargetIdentifier("_Downsample");
        var featureGen = new RenderTargetIdentifier("_FeatureGen");
        var chromatic = new RenderTargetIdentifier("_Chromatic");
        var blur1 = new RenderTargetIdentifier("_Blur1");
        var blur2 = new RenderTargetIdentifier("_Blur2");

        cmd.GetTemporaryRT(Shader.PropertyToID("_Downsample"), width / 2, height / 2, 0, FilterMode.Bilinear, descriptor.colorFormat);
        cmd.GetTemporaryRT(Shader.PropertyToID("_FeatureGen"), width / 2, height / 2, 0, FilterMode.Bilinear, descriptor.colorFormat);
        cmd.GetTemporaryRT(Shader.PropertyToID("_Chromatic"), width / 2, height / 2, 0, FilterMode.Bilinear, descriptor.colorFormat);
        cmd.GetTemporaryRT(Shader.PropertyToID("_Blur1"), width / 2, height / 2, 0, FilterMode.Bilinear, descriptor.colorFormat);
        cmd.GetTemporaryRT(Shader.PropertyToID("_Blur2"), width / 2, height / 2, 0, FilterMode.Bilinear, descriptor.colorFormat);

        cmd.Blit(cameraColorTarget, downsample, LensFlareMaterial, 0);
        cmd.Blit(downsample, featureGen, LensFlareMaterial, 1);
        cmd.Blit(featureGen, chromatic, LensFlareMaterial, 2);
        cmd.Blit(chromatic, blur1, LensFlareMaterial, 3);
        cmd.Blit(blur1, blur2, LensFlareMaterial, 4);

        // Resolve blur2 to a RenderTexture
        RenderTexture tempTexture = RenderTexture.GetTemporary(width / 2, height / 2, 0, descriptor.colorFormat);
        cmd.Blit(blur2, tempTexture);

        LensFlareMaterial.SetTexture("_LensFlareTex", tempTexture);
        cmd.Blit(cameraColorTarget, cameraColorTarget, LensFlareMaterial, 5);

        cmd.ReleaseTemporaryRT(Shader.PropertyToID("_NoiseTex"));
        cmd.ReleaseTemporaryRT(Shader.PropertyToID("_Downsample"));
        cmd.ReleaseTemporaryRT(Shader.PropertyToID("_FeatureGen"));
        cmd.ReleaseTemporaryRT(Shader.PropertyToID("_Chromatic"));
        cmd.ReleaseTemporaryRT(Shader.PropertyToID("_Blur1"));
        cmd.ReleaseTemporaryRT(Shader.PropertyToID("_Blur2"));
    }
}