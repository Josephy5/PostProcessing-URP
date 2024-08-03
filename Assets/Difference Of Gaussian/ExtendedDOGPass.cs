using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ExtendedDoGPass : ScriptableRenderPass
{
    static readonly string renderPassTag = "Extended DoG";

    private ExtendedDoGVolume ExtendedDoGVolume;
    private Material ExtendedDoGMaterial;

    public ExtendedDoGPass(RenderPassEvent evt, Shader ExtendedDoGshader)
    {
        renderPassEvent = evt;
        var shader = ExtendedDoGshader;
        if (shader == null)
        {
            Debug.LogError("No Shader");
            return;
        }
        ExtendedDoGMaterial = CoreUtils.CreateEngineMaterial(ExtendedDoGshader);
    }


    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if (ExtendedDoGMaterial == null)
        {
            Debug.LogError("No Extended DoG Material");
            return;
        }

        if (!renderingData.cameraData.postProcessEnabled)
        {
            //Debug.LogError("Post Processing in Camera not enabled");
            return;
        }

        VolumeStack stack = VolumeManager.instance.stack;
        ExtendedDoGVolume = stack.GetComponent<ExtendedDoGVolume>();

        var cmd = CommandBufferPool.Get(renderPassTag);
        Render(cmd, ref renderingData);
        
        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }

    void Render(CommandBuffer cmd, ref RenderingData renderingData)
    {
        if (ExtendedDoGVolume.IsActive() == false) return;
        ExtendedDoGVolume.load(ExtendedDoGMaterial, ref renderingData);

        var source = renderingData.cameraData.renderer.cameraColorTarget;

        int supersample = (int) ExtendedDoGVolume._superSample.value;
        int width = renderingData.cameraData.cameraTargetDescriptor.width;
        int height = renderingData.cameraData.cameraTargetDescriptor.height;

        var rgbToLab = RenderTexture.GetTemporary(width * supersample, height * supersample, 0, renderingData.cameraData.cameraTargetDescriptor.colorFormat);
        cmd.Blit(source, rgbToLab, ExtendedDoGMaterial, 0);

        var structureTensor = RenderTexture.GetTemporary(width * supersample, height * supersample, 0, renderingData.cameraData.cameraTargetDescriptor.colorFormat);
        var eigenvectors1 = RenderTexture.GetTemporary(width * supersample, height * supersample, 0, renderingData.cameraData.cameraTargetDescriptor.colorFormat);
        var eigenvectors2 = RenderTexture.GetTemporary(width * supersample, height * supersample, 0, renderingData.cameraData.cameraTargetDescriptor.colorFormat);

        bool useFlow = (bool) ExtendedDoGVolume._useFlow.value, smoothEdges = (bool) ExtendedDoGVolume._smoothEdges.value;
        if (useFlow || smoothEdges)
        {
            cmd.Blit(rgbToLab, structureTensor, ExtendedDoGMaterial, 1);
            cmd.Blit(structureTensor, eigenvectors1, ExtendedDoGMaterial, 2);
            cmd.Blit(eigenvectors1, eigenvectors2, ExtendedDoGMaterial, 3);
            ExtendedDoGMaterial.SetTexture("_TFM", eigenvectors2);
        }

        var gaussian1 = RenderTexture.GetTemporary(width * supersample, height * supersample, 0, renderingData.cameraData.cameraTargetDescriptor.colorFormat);
        var gaussian2 = RenderTexture.GetTemporary(width * supersample, height * supersample, 0, renderingData.cameraData.cameraTargetDescriptor.colorFormat);
        if (useFlow)
        {
            cmd.Blit(rgbToLab, gaussian1, ExtendedDoGMaterial, 4);
            cmd.Blit(gaussian1, gaussian2, ExtendedDoGMaterial, 5);
        }
        else
        {
            cmd.Blit(rgbToLab, gaussian1, ExtendedDoGMaterial, 6);
            cmd.Blit(gaussian1, gaussian2, ExtendedDoGMaterial, 7);
        }

        var differenceOfGaussians = RenderTexture.GetTemporary(width * supersample, height * supersample, 0, renderingData.cameraData.cameraTargetDescriptor.colorFormat);
        if (smoothEdges)
        {
            cmd.Blit(gaussian2, differenceOfGaussians, ExtendedDoGMaterial, 8);
        }
        else
        {
            cmd.Blit(gaussian2, differenceOfGaussians);
        }

        ExtendedDoGMaterial.SetTexture("_DogTex", differenceOfGaussians);

        cmd.Blit(source, source, ExtendedDoGMaterial, 9);
        
        RenderTexture.ReleaseTemporary(rgbToLab);
        RenderTexture.ReleaseTemporary(structureTensor);
        RenderTexture.ReleaseTemporary(eigenvectors1);
        RenderTexture.ReleaseTemporary(eigenvectors2);
        RenderTexture.ReleaseTemporary(gaussian2);
        RenderTexture.ReleaseTemporary(gaussian1);
        RenderTexture.ReleaseTemporary(differenceOfGaussians);
    }
    
}