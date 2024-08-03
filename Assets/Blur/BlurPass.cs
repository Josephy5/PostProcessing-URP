using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class BlurPass : ScriptableRenderPass
{
    static readonly string renderPassTag = "Blur";

    private BlurVolume blurVolume;
    private Material blurMaterial;

    //private int testRTID;
    private int sampleTimes;

    public BlurPass(RenderPassEvent evt, Shader blurshader)
    {
        renderPassEvent = evt;
        var shader = blurshader;
        if (shader == null)
        {
            Debug.LogError("No Shader");
            return;
        }
        blurMaterial = CoreUtils.CreateEngineMaterial(blurshader);
        //this.sampleTimes = sampleTimes;
    }


    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if (blurMaterial == null)
        {
            Debug.LogError("No Blur Material");
            return;
        }

        if (!renderingData.cameraData.postProcessEnabled)
        {
            //Debug.LogError("Post Processing in Camera not enabled");
            return;
        }

        VolumeStack stack = VolumeManager.instance.stack;
        blurVolume = stack.GetComponent<BlurVolume>();

        var cmd = CommandBufferPool.Get(renderPassTag);
        Render(cmd, ref renderingData);
        context.ExecuteCommandBuffer(cmd);

        CommandBufferPool.Release(cmd);

    }

    void Render(CommandBuffer cmd, ref RenderingData renderingData)
    {
        if (blurVolume.IsActive() == false) return;
        blurVolume.load(blurMaterial, ref renderingData);
        int blurPasses = blurVolume._blurPasses.value;
        int blurOperator = (int) blurVolume._blurOperator.value;

        var cameraColorTarget = renderingData.cameraData.renderer.cameraColorTarget;

        var descriptor = renderingData.cameraData.cameraTargetDescriptor;
        var temp1 = Shader.PropertyToID("_Temp1");
        var temp2 = Shader.PropertyToID("_Temp2");

        cmd.GetTemporaryRT(temp1, descriptor);
        cmd.GetTemporaryRT(temp2, descriptor);

        cmd.Blit(cameraColorTarget, temp1, blurMaterial, blurOperator * 2);
        cmd.Blit(temp1, temp2, blurMaterial, blurOperator * 2 + 1);

        for (int i = 1; i < blurPasses; ++i)
        {
            cmd.Blit(temp2, temp1, blurMaterial, blurOperator * 2);
            cmd.Blit(temp1, temp2, blurMaterial, blurOperator * 2 + 1);
        }

        cmd.Blit(temp2, cameraColorTarget);

        cmd.ReleaseTemporaryRT(temp1);
        cmd.ReleaseTemporaryRT(temp2);
    }

}