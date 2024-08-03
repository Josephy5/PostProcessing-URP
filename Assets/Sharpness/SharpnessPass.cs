using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SharpnessPass : ScriptableRenderPass
{
    static readonly string renderPassTag = "Sharpness";

    private SharpnessVolume sharpnessVolume;
    private Material sharpnessMaterial;
    private Shader SharpnessShader;

    public SharpnessPass(RenderPassEvent evt)
    {
        renderPassEvent = evt;

        VolumeStack stack = VolumeManager.instance.stack;
        sharpnessVolume = stack.GetComponent<SharpnessVolume>();

        switch (sharpnessVolume._sharpnessMode.value)
        {
            case SharpnessMode.Normal: SharpnessShader = Shader.Find("Joseph&Acerola/Sharpness"); break;
            case SharpnessMode.Contrast: SharpnessShader = Shader.Find("Joseph&Acerola/ContrastAdaptiveSharpness"); break;
        }

        if (SharpnessShader == null)
        {
            Debug.LogError("No Shader");
            return;
        }

        sharpnessMaterial = CoreUtils.CreateEngineMaterial(SharpnessShader);
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if (sharpnessMaterial == null)
        {
            Debug.LogError("No Sharpness Material");
            return;
        }

        if (!renderingData.cameraData.postProcessEnabled)
        {
            //Debug.LogError("Post Processing in Camera not enabled");
            return;
        }

        VolumeStack stack = VolumeManager.instance.stack;
        sharpnessVolume = stack.GetComponent<SharpnessVolume>();

        var cmd = CommandBufferPool.Get(renderPassTag);
        Render(cmd, ref renderingData);
        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }
    void Render(CommandBuffer cmd, ref RenderingData renderingData)
    {
        sharpnessVolume.load(sharpnessMaterial, ref renderingData);

        var cameraColorTarget = renderingData.cameraData.renderer.cameraColorTarget;

        // Apply sharpness effect
        cmd.GetTemporaryRT(Shader.PropertyToID("_SharpnessRT"), renderingData.cameraData.cameraTargetDescriptor, FilterMode.Point);
        RenderTargetIdentifier sharpness = new RenderTargetIdentifier("_SharpnessRT");
        cmd.Blit(cameraColorTarget, sharpness, sharpnessMaterial, 0); // Use pass 0

        // Final blit to the destination
        cmd.Blit(sharpness, cameraColorTarget); // Use the default pass

        // Release temporary render texture
        cmd.ReleaseTemporaryRT(Shader.PropertyToID("_SharpnessRT"));
    }

}