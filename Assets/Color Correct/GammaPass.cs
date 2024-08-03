using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GammaPass : ScriptableRenderPass
{
    static readonly string renderPassTag = "Gamma Correct";

    private GammaVolume GammaVolume;
    private Material GammaMaterial;

    public GammaPass(RenderPassEvent evt, Shader Gammashader)
    {
        renderPassEvent = evt;
        var shader = Gammashader;
        if (shader == null)
        {
            Debug.LogError("No Shader");
            return;
        }
        GammaMaterial = CoreUtils.CreateEngineMaterial(Gammashader);
    }


    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if (GammaMaterial == null)
        {
            Debug.LogError("No Gamma Material");
            return;
        }

        if (!renderingData.cameraData.postProcessEnabled || renderingData.cameraData.cameraType == CameraType.SceneView)
        {
            //Debug.LogError("Post Processing in Camera not enabled");
            return;
        }

        VolumeStack stack = VolumeManager.instance.stack;
        GammaVolume = stack.GetComponent<GammaVolume>();

        var cmd = CommandBufferPool.Get(renderPassTag);
        Render(cmd, ref renderingData);
        context.ExecuteCommandBuffer(cmd);

        CommandBufferPool.Release(cmd);
    }

    void Render(CommandBuffer cmd, ref RenderingData renderingData)
    {
        if (GammaVolume.IsActive() == false) return;
        GammaVolume.load(GammaMaterial, ref renderingData);

        var source = renderingData.cameraData.renderer.cameraColorTarget;

        cmd.Blit(source, source, GammaMaterial);
    }

}