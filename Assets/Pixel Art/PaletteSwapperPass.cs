using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PaletteSwapperPass : ScriptableRenderPass
{
    static readonly string renderPassTag = "Palette Swapper";

    private PaletteSwapperVolume PaletteSwapperVolume;
    private Material PaletteSwapperMaterial;

    public PaletteSwapperPass(RenderPassEvent evt, Shader halfToneshader)
    {
        renderPassEvent = evt;
        var shader = halfToneshader;
        if (shader == null)
        {
            Debug.LogError("No Shader");
            return;
        }
        PaletteSwapperMaterial = CoreUtils.CreateEngineMaterial(halfToneshader);
    }


    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if (PaletteSwapperMaterial == null)
        {
            Debug.LogError("No Palette Swapper Material");
            return;
        }

        if (!renderingData.cameraData.postProcessEnabled || renderingData.cameraData.cameraType == CameraType.SceneView)
        {
            //Debug.LogError("Post Processing in Camera not enabled");
            return;
        }

        VolumeStack stack = VolumeManager.instance.stack;
        PaletteSwapperVolume = stack.GetComponent<PaletteSwapperVolume>();

        var cmd = CommandBufferPool.Get(renderPassTag);
        Render(cmd, ref renderingData);
        context.ExecuteCommandBuffer(cmd);

        CommandBufferPool.Release(cmd);
    }

    void Render(CommandBuffer cmd, ref RenderingData renderingData)
    {
        if (PaletteSwapperVolume.IsActive() == false) return;
        PaletteSwapperVolume.load(PaletteSwapperMaterial, ref renderingData);

        var source = renderingData.cameraData.renderer.cameraColorTarget;

        cmd.Blit(source, source, PaletteSwapperMaterial);
    }

}