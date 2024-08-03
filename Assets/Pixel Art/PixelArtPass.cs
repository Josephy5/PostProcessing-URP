using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PixelArtPass : ScriptableRenderPass
{
    static readonly string renderPassTag = "Pixel Art";

    private PixelArtVolume PixelArtVolume;
    private Material PixelArtMaterial;

    public PixelArtPass(RenderPassEvent evt, Shader halfToneshader)
    {
        renderPassEvent = evt;
        var shader = halfToneshader;
        if (shader == null)
        {
            Debug.LogError("No Shader");
            return;
        }
        PixelArtMaterial = CoreUtils.CreateEngineMaterial(halfToneshader);
    }


    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if (PixelArtMaterial == null)
        {
            Debug.LogError("No Pixel Art Material");
            return;
        }

        if (!renderingData.cameraData.postProcessEnabled || renderingData.cameraData.cameraType == CameraType.SceneView)
        {
            //Debug.LogError("Post Processing in Camera not enabled");
            return;
        }

        VolumeStack stack = VolumeManager.instance.stack;
        PixelArtVolume = stack.GetComponent<PixelArtVolume>();

        var cmd = CommandBufferPool.Get(renderPassTag);
        Render(cmd, ref renderingData);
        context.ExecuteCommandBuffer(cmd);

        CommandBufferPool.Release(cmd);
    }

    void Render(CommandBuffer cmd, ref RenderingData renderingData)
    {
        if (PixelArtVolume.IsActive() == false) return;
        //PixelArtVolume.load(HalftoneMaterial, ref renderingData);

        int width = renderingData.cameraData.cameraTargetDescriptor.width;
        int height = renderingData.cameraData.cameraTargetDescriptor.height;
        int downSample = (int) PixelArtVolume._downSample.value;

        var source = renderingData.cameraData.renderer.cameraColorTarget;
        RenderTexture[] textures = new RenderTexture[8];
        var currentSource = source;

        for (int i = 0; i < downSample; ++i)
        {
            width /= 2;
            height /= 2;

            if (height < 2)
                break;

            RenderTexture currentDestination = textures[i] = RenderTexture.GetTemporary(width, height, 0, renderingData.cameraData.cameraTargetDescriptor.colorFormat);
            cmd.Blit(source, currentDestination, PixelArtMaterial);
            currentSource = currentDestination;
        }

        cmd.Blit(currentSource, source, PixelArtMaterial);

        for (int i = 0; i < downSample; ++i)
        {
            RenderTexture.ReleaseTemporary(textures[i]);
        }
    }

}