using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ColorCorrectPass : ScriptableRenderPass
{
    static readonly string renderPassTag = "Color Correct";

    private ColorCorrectVolume colorCorrectVolume;
    private Material colorCorrectMaterial;

    //private int testRTID;
    //private int sampleTimes;

    public ColorCorrectPass(RenderPassEvent evt, Shader blendModeshader)
    {
        renderPassEvent = evt;
        var shader = blendModeshader;
        if (shader == null)
        {
            Debug.LogError("No Shader");
            return;
        }
        colorCorrectMaterial = CoreUtils.CreateEngineMaterial(blendModeshader);
        //this.sampleTimes = sampleTimes;
    }


    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if (colorCorrectMaterial == null)
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
        colorCorrectVolume = stack.GetComponent<ColorCorrectVolume>();

        var cmd = CommandBufferPool.Get(renderPassTag);
        Render(cmd, ref renderingData);
        context.ExecuteCommandBuffer(cmd);

        CommandBufferPool.Release(cmd);
    }

    void Render(CommandBuffer cmd, ref RenderingData renderingData)
    {
        if (colorCorrectVolume.IsActive() == false) return;
        colorCorrectVolume.load(colorCorrectMaterial, ref renderingData);

        var source = renderingData.cameraData.renderer.cameraColorTarget;

        cmd.Blit(source, source, colorCorrectMaterial);
    }

}