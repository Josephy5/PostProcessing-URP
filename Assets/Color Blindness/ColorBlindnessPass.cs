using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ColorBlindnessPass : ScriptableRenderPass
{
    static readonly string renderPassTag = "Color Blindness";

    private ColorBlindnessVolume ColorBlindnessVolume;
    private Material ColorBlindnessMaterial;

    public ColorBlindnessPass(RenderPassEvent evt, Shader ColorBlindnessshader)
    {
        renderPassEvent = evt;
        var shader = ColorBlindnessshader;
        if (shader == null)
        {
            Debug.LogError("No Shader");
            return;
        }
        ColorBlindnessMaterial = CoreUtils.CreateEngineMaterial(ColorBlindnessshader);
    }


    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if (ColorBlindnessMaterial == null)
        {
            Debug.LogError("No Color Blindness Material");
            return;
        }

        if (!renderingData.cameraData.postProcessEnabled || renderingData.cameraData.cameraType == CameraType.SceneView)
        {
            //Debug.LogError("Post Processing in Camera not enabled");
            return;
        }

        VolumeStack stack = VolumeManager.instance.stack;
        ColorBlindnessVolume = stack.GetComponent<ColorBlindnessVolume>();

        var cmd = CommandBufferPool.Get(renderPassTag);
        Render(cmd, ref renderingData);
        context.ExecuteCommandBuffer(cmd);

        CommandBufferPool.Release(cmd);
    }

    void Render(CommandBuffer cmd, ref RenderingData renderingData)
    {
        if (ColorBlindnessVolume.IsActive() == false) return;
        ColorBlindnessVolume.load(ColorBlindnessMaterial, ref renderingData);

        int blindType = (int)ColorBlindnessVolume._blindType.value;

        var source = renderingData.cameraData.renderer.cameraColorTarget;

        cmd.Blit(source, source, ColorBlindnessMaterial, blindType);
    }

}