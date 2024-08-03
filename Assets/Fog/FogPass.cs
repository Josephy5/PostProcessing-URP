using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class FogPass : ScriptableRenderPass
{
    static readonly string renderPassTag = "Fog";

    private FogVolume fogVolume;
    private Material fogMaterial;

    public FogPass(RenderPassEvent evt, Shader blendModeshader)
    {
        renderPassEvent = evt;
        var shader = blendModeshader;
        if (shader == null)
        {
            Debug.LogError("No Shader");
            return;
        }
        fogMaterial = CoreUtils.CreateEngineMaterial(blendModeshader);
    }


    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if (fogMaterial == null)
        {
            Debug.LogError("No Fog Material");
            return;
        }

        if (!renderingData.cameraData.postProcessEnabled || renderingData.cameraData.cameraType == CameraType.SceneView)
        {
            //Debug.LogError("Post Processing in Camera not enabled");
            return;
        }

        VolumeStack stack = VolumeManager.instance.stack;
        fogVolume = stack.GetComponent<FogVolume>();

        var cmd = CommandBufferPool.Get(renderPassTag);
        Render(cmd, ref renderingData);
        context.ExecuteCommandBuffer(cmd);

        CommandBufferPool.Release(cmd);
    }

    void Render(CommandBuffer cmd, ref RenderingData renderingData)
    {
        if (fogVolume.IsActive() == false) return;
        fogVolume.load(fogMaterial, ref renderingData);

        var source = renderingData.cameraData.renderer.cameraColorTarget;

        cmd.Blit(source, source, fogMaterial);
    }

}