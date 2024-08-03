using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ZoomPass : ScriptableRenderPass
{
    static readonly string renderPassTag = "Zoom";

    private ZoomVolume ZoomVolume;
    private Material ZoomMaterial;

    public ZoomPass(RenderPassEvent evt, Shader blendModeshader)
    {
        renderPassEvent = evt;
        var shader = blendModeshader;
        if (shader == null)
        {
            Debug.LogError("No Shader");
            return;
        }
        ZoomMaterial = CoreUtils.CreateEngineMaterial(blendModeshader);
    }


    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if (ZoomMaterial == null)
        {
            Debug.LogError("No Color Correct Material");
            return;
        }

        if (!renderingData.cameraData.postProcessEnabled || renderingData.cameraData.cameraType == CameraType.SceneView)
        {
            //Debug.LogError("Post Processing in Camera not enabled");
            return;
        }

        VolumeStack stack = VolumeManager.instance.stack;
        ZoomVolume = stack.GetComponent<ZoomVolume>();

        var cmd = CommandBufferPool.Get(renderPassTag);
        Render(cmd, ref renderingData);
        context.ExecuteCommandBuffer(cmd);

        CommandBufferPool.Release(cmd);
    }

    void Render(CommandBuffer cmd, ref RenderingData renderingData)
    {
        if (ZoomVolume.IsActive() == false) return;
        ZoomVolume.load(ZoomMaterial, ref renderingData);

        var source = renderingData.cameraData.renderer.cameraColorTarget;

        cmd.Blit(source, source, ZoomMaterial);
    }

}