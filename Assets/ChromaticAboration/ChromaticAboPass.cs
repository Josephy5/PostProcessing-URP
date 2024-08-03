using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ChromaticAboPass : ScriptableRenderPass
{
    static readonly string renderPassTag = "Chromatic Aberration";

    private ChromaticAboVolume ChromaticVolume;
    private Material ChromaticMaterial;

    public ChromaticAboPass(RenderPassEvent evt, Shader chromaticshader)
    {
        renderPassEvent = evt;
        var shader = chromaticshader;
        if (shader == null)
        {
            Debug.LogError("No Shader");
            return;
        }
        ChromaticMaterial = CoreUtils.CreateEngineMaterial(chromaticshader);
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if (ChromaticMaterial == null)
        {
            Debug.LogError("No Chromatic Aboration Material");
            return;
        }

        if (!renderingData.cameraData.postProcessEnabled)
        {
            //Debug.LogError("Post Processing in Camera not enabled");
            return;
        }

        VolumeStack stack = VolumeManager.instance.stack;
        ChromaticVolume = stack.GetComponent<ChromaticAboVolume>();

        var cmd = CommandBufferPool.Get(renderPassTag);
        Render(cmd, ref renderingData);
        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }
    void Render(CommandBuffer cmd, ref RenderingData renderingData)
    {
        ChromaticVolume.load(ChromaticMaterial, ref renderingData);

        var cameraColorTarget = renderingData.cameraData.renderer.cameraColorTarget;

        // Apply chromatic aberration effect
        cmd.GetTemporaryRT(Shader.PropertyToID("_ChromaticRT"), renderingData.cameraData.cameraTargetDescriptor, FilterMode.Point);
        RenderTargetIdentifier chromatic = new RenderTargetIdentifier("_ChromaticRT");
        cmd.Blit(cameraColorTarget, chromatic, ChromaticMaterial, 0); // Use pass 0

        // Final blit to the destination
        cmd.Blit(chromatic, cameraColorTarget); // Use the default pass

        // Release temporary render texture
        cmd.ReleaseTemporaryRT(Shader.PropertyToID("_ChromaticRT"));
    }
}
