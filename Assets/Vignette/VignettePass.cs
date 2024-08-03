using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class VignettePass : ScriptableRenderPass
{
    static readonly string renderPassTag = "Vignette";

    private VignetteVolume VignetteVolume;
    private Material VignetteMaterial;

    public VignettePass(RenderPassEvent evt, Shader blendModeshader)
    {
        renderPassEvent = evt;
        var shader = blendModeshader;
        if (shader == null)
        {
            Debug.LogError("No Shader");
            return;
        }
        VignetteMaterial = CoreUtils.CreateEngineMaterial(blendModeshader);
    }


    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if (VignetteMaterial == null)
        {
            Debug.LogError("No Vignette Material");
            return;
        }

        if (!renderingData.cameraData.postProcessEnabled || renderingData.cameraData.cameraType == CameraType.SceneView)
        {
            //Debug.LogError("Post Processing in Camera not enabled");
            return;
        }

        VolumeStack stack = VolumeManager.instance.stack;
        VignetteVolume = stack.GetComponent<VignetteVolume>();

        var cmd = CommandBufferPool.Get(renderPassTag);
        Render(cmd, ref renderingData);
        context.ExecuteCommandBuffer(cmd);

        CommandBufferPool.Release(cmd);
    }

    void Render(CommandBuffer cmd, ref RenderingData renderingData)
    {
        if (VignetteVolume.IsActive() == false) return;
        VignetteVolume.load(VignetteMaterial, ref renderingData);

        var source = renderingData.cameraData.renderer.cameraColorTarget;

        cmd.Blit(source, source, VignetteMaterial);
    }

}