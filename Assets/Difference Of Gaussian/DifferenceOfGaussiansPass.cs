using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DifferenceOfGaussiansPass : ScriptableRenderPass
{
    static readonly string renderPassTag = "DoG";

    private DifferenceOfGaussiansVolume DifferenceOfGaussiansVolume;
    private Material DifferenceOfGaussiansMaterial;

    public DifferenceOfGaussiansPass(RenderPassEvent evt, Shader DifferenceOfGaussiansshader)
    {
        renderPassEvent = evt;
        var shader = DifferenceOfGaussiansshader;
        if (shader == null)
        {
            Debug.LogError("No Shader");
            return;
        }
        DifferenceOfGaussiansMaterial = CoreUtils.CreateEngineMaterial(DifferenceOfGaussiansshader);
    }


    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if (DifferenceOfGaussiansMaterial == null)
        {
            Debug.LogError("No Difference Of Gaussians Material");
            return;
        }

        if (!renderingData.cameraData.postProcessEnabled)
        {
            //Debug.LogError("Post Processing in Camera not enabled");
            return;
        }

        VolumeStack stack = VolumeManager.instance.stack;
        DifferenceOfGaussiansVolume = stack.GetComponent<DifferenceOfGaussiansVolume>();

        var cmd = CommandBufferPool.Get(renderPassTag);
        Render(cmd, ref renderingData);
        context.ExecuteCommandBuffer(cmd);

        CommandBufferPool.Release(cmd);
    }

    void Render(CommandBuffer cmd, ref RenderingData renderingData)
    {
        if (DifferenceOfGaussiansVolume.IsActive() == false) return;
        DifferenceOfGaussiansVolume.load(DifferenceOfGaussiansMaterial, ref renderingData);

        var source = renderingData.cameraData.renderer.cameraColorTarget;
        int width = renderingData.cameraData.cameraTargetDescriptor.width;
        int height = renderingData.cameraData.cameraTargetDescriptor.height;

        var gaussian1 = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.RG32);
        cmd.Blit(source, gaussian1, DifferenceOfGaussiansMaterial, 0);
        var gaussian2 = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.RG32);
        cmd.Blit(gaussian1, gaussian2, DifferenceOfGaussiansMaterial, 1);

        DifferenceOfGaussiansMaterial.SetTexture("_GaussianTex", gaussian2);

        cmd.Blit(source, source, DifferenceOfGaussiansMaterial, 2);
        RenderTexture.ReleaseTemporary(gaussian1);
        RenderTexture.ReleaseTemporary(gaussian2);
    }

}