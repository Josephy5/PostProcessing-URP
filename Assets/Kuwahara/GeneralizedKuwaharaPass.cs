using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GeneralizedKuwaharaPass : ScriptableRenderPass
{
    static readonly string renderPassTag = "Generalized Kuwahara";

    private GeneralizedKuwaharaVolume GeneralizedKuwaharaVolume;
    private Material GeneralizedKuwaharaMaterial;

    public GeneralizedKuwaharaPass(RenderPassEvent evt, Shader GeneralizedKuwaharashader)
    {
        renderPassEvent = evt;
        var shader = GeneralizedKuwaharashader;
        if (shader == null)
        {
            Debug.LogError("No Shader");
            return;
        }
        GeneralizedKuwaharaMaterial = CoreUtils.CreateEngineMaterial(GeneralizedKuwaharashader);
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if (GeneralizedKuwaharaMaterial == null)
        {
            Debug.LogError("No Generalized Kuwahara Material");
            return;
        }

        if (!renderingData.cameraData.postProcessEnabled)
        {
            //Debug.LogError("Post Processing in Camera not enabled");
            return;
        }

        VolumeStack stack = VolumeManager.instance.stack;
        GeneralizedKuwaharaVolume = stack.GetComponent<GeneralizedKuwaharaVolume>();

        var cmd = CommandBufferPool.Get(renderPassTag);
        Render(cmd, ref renderingData);
        context.ExecuteCommandBuffer(cmd);

        CommandBufferPool.Release(cmd);
    }

    void Render(CommandBuffer cmd, ref RenderingData renderingData)
    {
        if (GeneralizedKuwaharaVolume.IsActive() == false) return;
        GeneralizedKuwaharaVolume.load(GeneralizedKuwaharaMaterial, ref renderingData);

        var source = renderingData.cameraData.renderer.cameraColorTarget;
        int width = renderingData.cameraData.cameraTargetDescriptor.width;
        int height = renderingData.cameraData.cameraTargetDescriptor.height;

        int passes = (int)GeneralizedKuwaharaVolume._passes.value;
        RenderTexture[] kuwaharaPasses = new RenderTexture[passes];

        for (int i = 0; i < passes; ++i)
        {
            kuwaharaPasses[i] = RenderTexture.GetTemporary(width, height, 0, renderingData.cameraData.cameraTargetDescriptor.colorFormat);
        }

        cmd.Blit(source, kuwaharaPasses[0], GeneralizedKuwaharaMaterial);

        for (int i = 1; i < passes; ++i)
        {
            cmd.Blit(kuwaharaPasses[i - 1], kuwaharaPasses[i], GeneralizedKuwaharaMaterial);
        }

        cmd.Blit(kuwaharaPasses[passes - 1], source);

        for (int i = 0; i < passes; ++i)
        {
            RenderTexture.ReleaseTemporary(kuwaharaPasses[i]);
        }
    }

}