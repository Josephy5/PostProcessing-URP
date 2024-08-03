using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class HalftonePass : ScriptableRenderPass
{
    static readonly string renderPassTag = "Half Tone";

    private HalftoneVolume HalftoneVolume;
    private Material HalftoneMaterial;

    public HalftonePass(RenderPassEvent evt, Shader halfToneshader)
    {
        renderPassEvent = evt;
        var shader = halfToneshader;
        if (shader == null)
        {
            Debug.LogError("No Shader");
            return;
        }
        HalftoneMaterial = CoreUtils.CreateEngineMaterial(halfToneshader);
    }


    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if (HalftoneMaterial == null)
        {
            Debug.LogError("No Half Tone Material");
            return;
        }

        if (!renderingData.cameraData.postProcessEnabled || renderingData.cameraData.cameraType == CameraType.SceneView)
        {
            //Debug.LogError("Post Processing in Camera not enabled");
            return;
        }

        VolumeStack stack = VolumeManager.instance.stack;
        HalftoneVolume = stack.GetComponent<HalftoneVolume>();

        var cmd = CommandBufferPool.Get(renderPassTag);
        Render(cmd, ref renderingData);
        context.ExecuteCommandBuffer(cmd);

        CommandBufferPool.Release(cmd);
    }

    void Render(CommandBuffer cmd, ref RenderingData renderingData)
    {
        if (HalftoneVolume.IsActive() == false) return;
        HalftoneVolume.load(HalftoneMaterial, ref renderingData);

        int width = renderingData.cameraData.cameraTargetDescriptor.width;
        int height = renderingData.cameraData.cameraTargetDescriptor.height;

        var source = renderingData.cameraData.renderer.cameraColorTarget;
        var cmyk = RenderTexture.GetTemporary(width, height, 0, renderingData.cameraData.cameraTargetDescriptor.colorFormat);

        cmd.Blit(source, cmyk, HalftoneMaterial, 0);
        cmd.Blit(cmyk, source, HalftoneMaterial, 1);
        RenderTexture.ReleaseTemporary(cmyk);
    }

}