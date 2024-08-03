using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class HueShiftPass : ScriptableRenderPass
{
    static readonly string renderPassTag = "Hue Shift Correct";

    private HueShiftVolume HueShiftVolume;
    private Material HueShiftMaterial;

    //private int testRTID;
    //private int sampleTimes;

    public HueShiftPass(RenderPassEvent evt, Shader Gammashader)
    {
        renderPassEvent = evt;
        var shader = Gammashader;
        if (shader == null)
        {
            Debug.LogError("No Shader");
            return;
        }
        HueShiftMaterial = CoreUtils.CreateEngineMaterial(Gammashader);
        //this.sampleTimes = sampleTimes;
    }


    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if (HueShiftMaterial == null)
        {
            Debug.LogError("No Hue Shift Material");
            return;
        }

        if (!renderingData.cameraData.postProcessEnabled || renderingData.cameraData.cameraType == CameraType.SceneView)
        {
            //Debug.LogError("Post Processing in Camera not enabled");
            return;
        }

        VolumeStack stack = VolumeManager.instance.stack;
        HueShiftVolume = stack.GetComponent<HueShiftVolume>();

        var cmd = CommandBufferPool.Get(renderPassTag);
        Render(cmd, ref renderingData);
        context.ExecuteCommandBuffer(cmd);

        CommandBufferPool.Release(cmd);
    }

    void Render(CommandBuffer cmd, ref RenderingData renderingData)
    {
        if (HueShiftVolume.IsActive() == false) return;
        HueShiftVolume.load(HueShiftMaterial, ref renderingData);

        var source = renderingData.cameraData.renderer.cameraColorTarget;

        cmd.Blit(source, source, HueShiftMaterial);
    }

}