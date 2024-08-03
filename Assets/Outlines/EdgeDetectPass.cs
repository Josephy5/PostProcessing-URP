using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class EdgeDetectPass : ScriptableRenderPass
{
    static readonly string renderPassTag = "Edge Detect";

    private EdgeDetectVolume EdgeDetectVolume;
    private Material EdgeDetectMaterial;

    public EdgeDetectPass(RenderPassEvent evt, Shader EdgeDetectshader)
    {
        renderPassEvent = evt;
        var shader = EdgeDetectshader;
        if (shader == null)
        {
            Debug.LogError("No Shader");
            return;
        }
        EdgeDetectMaterial = CoreUtils.CreateEngineMaterial(EdgeDetectshader);
    }


    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if (EdgeDetectMaterial == null)
        {
            Debug.LogError("No Edge Detect Material");
            return;
        }

        if (!renderingData.cameraData.postProcessEnabled)
        {
            //Debug.LogError("Post Processing in Camera not enabled");
            return;
        }

        VolumeStack stack = VolumeManager.instance.stack;
        EdgeDetectVolume = stack.GetComponent<EdgeDetectVolume>();

        var cmd = CommandBufferPool.Get(renderPassTag);
        Render(cmd, ref renderingData);
        context.ExecuteCommandBuffer(cmd);

        CommandBufferPool.Release(cmd);
    }

    void Render(CommandBuffer cmd, ref RenderingData renderingData)
    {
        if (EdgeDetectVolume.IsActive() == false) return;
        EdgeDetectVolume.load(EdgeDetectMaterial, ref renderingData);

        var source = renderingData.cameraData.renderer.cameraColorTarget;

        cmd.Blit(source, source, EdgeDetectMaterial);
    }

}