using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class BlendModePass : ScriptableRenderPass
{
    static readonly string renderPassTag = "Blend Mode";

    private BlendModeVolume blendModeVolume;
    private Material blendModeMaterial;

    public BlendModePass(RenderPassEvent evt, Shader blendModeshader)
    {
        renderPassEvent = evt;
        var shader = blendModeshader;
        if (shader == null)
        {
            Debug.LogError("No Shader");
            return;
        }
        blendModeMaterial = CoreUtils.CreateEngineMaterial(blendModeshader);
    }


    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if (blendModeMaterial == null)
        {
            Debug.LogError("No Blend Mode Material");
            return;
        }

        if (!renderingData.cameraData.postProcessEnabled)
        {
            //Debug.LogError("Post Processing in Camera not enabled");
            return;
        }

        VolumeStack stack = VolumeManager.instance.stack;
        blendModeVolume = stack.GetComponent<BlendModeVolume>();

        var cmd = CommandBufferPool.Get(renderPassTag);
        Render(cmd, ref renderingData);
        context.ExecuteCommandBuffer(cmd);

        CommandBufferPool.Release(cmd);
    }

    void Render(CommandBuffer cmd, ref RenderingData renderingData)
    {
        if (blendModeVolume.IsActive() == false) return;
        blendModeVolume.load(blendModeMaterial, ref renderingData);

        int blendMode = (int)blendModeVolume._blendMode.value;

        var source = renderingData.cameraData.renderer.cameraColorTarget;

        var descriptor = renderingData.cameraData.cameraTargetDescriptor;
        descriptor.depthBufferBits = 0;

        var temporaryRT = Shader.PropertyToID("_TemporaryRT");
        cmd.GetTemporaryRT(temporaryRT, descriptor, FilterMode.Point);

        cmd.Blit(source, temporaryRT);
        cmd.Blit(temporaryRT, source, blendModeMaterial, blendMode) ;

        cmd.ReleaseTemporaryRT(temporaryRT);
    }

}