using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class BloomAcerolaPass : ScriptableRenderPass
{
    static readonly string renderPassTag = "Acerola Bloom";

    private BloomAcerolaVolume BloomAcerolaVolume;
    private Material BloomAcerolaMaterial;

    public BloomAcerolaPass(RenderPassEvent evt, Shader BloomAcerolashader)
    {
        renderPassEvent = evt;
        var shader = BloomAcerolashader;
        if (shader == null)
        {
            Debug.LogError("No Shader");
            return;
        }
        BloomAcerolaMaterial = CoreUtils.CreateEngineMaterial(BloomAcerolashader);
    }


    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if (BloomAcerolaMaterial == null)
        {
            Debug.LogError("No Acerola Bloom Material");
            return;
        }

        if (!renderingData.cameraData.postProcessEnabled)
        {
            //Debug.LogError("Post Processing in Camera not enabled");
            return;
        }

        VolumeStack stack = VolumeManager.instance.stack;
        BloomAcerolaVolume = stack.GetComponent<BloomAcerolaVolume>();

        var cmd = CommandBufferPool.Get(renderPassTag);
        Render(cmd, ref renderingData);
        context.ExecuteCommandBuffer(cmd);

        CommandBufferPool.Release(cmd);
    }

    void Render(CommandBuffer cmd, ref RenderingData renderingData)
    {
        if (BloomAcerolaVolume.IsActive() == false) return;
        BloomAcerolaVolume.load(BloomAcerolaMaterial, ref renderingData);

        int width = renderingData.cameraData.cameraTargetDescriptor.width;
        int height = renderingData.cameraData.cameraTargetDescriptor.height;
        var source = renderingData.cameraData.renderer.cameraColorTarget;

        width /= 2;
        height /= 2;

        RenderTexture[] textures = new RenderTexture[16];
        RenderTexture currentDestination = textures[0] = RenderTexture.GetTemporary(width, height, 0, renderingData.cameraData.cameraTargetDescriptor.colorFormat);

        cmd.Blit(source, currentDestination, BloomAcerolaMaterial, 0);
        RenderTexture currentSource = currentDestination;

        int downSamples = (int)BloomAcerolaVolume._downSamples.value;

        int i = 1;
        for (; i < downSamples; ++i)
        {
            width /= 2;
            height /= 2;

            if (height < 2)
                break;

            currentDestination = textures[i] = RenderTexture.GetTemporary(width, height, 0, renderingData.cameraData.cameraTargetDescriptor.colorFormat);
            Graphics.Blit(currentSource, currentDestination, BloomAcerolaMaterial, 1);
            currentSource = currentDestination;
        }

        for (i -= 2; i >= 0; --i)
        {
            currentDestination = textures[i];
            textures[i] = null;
            Graphics.Blit(currentSource, currentDestination, BloomAcerolaMaterial, 2);
            RenderTexture.ReleaseTemporary(currentSource);
            currentSource = currentDestination;
        }

        cmd.Blit(currentSource, source, BloomAcerolaMaterial, 3);
        RenderTexture.ReleaseTemporary(currentSource);
    }

}