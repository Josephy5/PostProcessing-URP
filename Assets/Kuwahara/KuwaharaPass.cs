using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class KuwaharaPass : ScriptableRenderPass
{
    static readonly string renderPassTag = "Kuwahara";

    private KuwaharaVolume KuwaharaVolume;
    private Material KuwaharaMaterial;

    public KuwaharaPass(RenderPassEvent evt, Shader Kuwaharashader)
    {
        renderPassEvent = evt;
        var shader = Kuwaharashader;
        if (shader == null)
        {
            Debug.LogError("No Shader");
            return;
        }
        KuwaharaMaterial = CoreUtils.CreateEngineMaterial(Kuwaharashader);
    }


    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if (KuwaharaMaterial == null)
        {
            Debug.LogError("No Kuwahara Material");
            return;
        }

        if (!renderingData.cameraData.postProcessEnabled)
        {
            //Debug.LogError("Post Processing in Camera not enabled");
            return;
        }

        VolumeStack stack = VolumeManager.instance.stack;
        KuwaharaVolume = stack.GetComponent<KuwaharaVolume>();

        var cmd = CommandBufferPool.Get(renderPassTag);
        Render(cmd, ref renderingData);
        context.ExecuteCommandBuffer(cmd);

        CommandBufferPool.Release(cmd);
    }

    void Render(CommandBuffer cmd, ref RenderingData renderingData)
    {
        if (KuwaharaVolume.IsActive() == false) return;
        KuwaharaVolume.load(KuwaharaMaterial, ref renderingData);

        var source = renderingData.cameraData.renderer.cameraColorTarget;
        int width = renderingData.cameraData.cameraTargetDescriptor.width;
        int height = renderingData.cameraData.cameraTargetDescriptor.height;

        int passes = (int)KuwaharaVolume._passes.value;
        RenderTexture[] kuwaharaPasses = new RenderTexture[passes];

        for (int i = 0; i < passes; ++i)
        {
            kuwaharaPasses[i] = RenderTexture.GetTemporary(width, height, 0, renderingData.cameraData.cameraTargetDescriptor.colorFormat);
        }

        cmd.Blit(source, kuwaharaPasses[0], KuwaharaMaterial);

        for (int i = 1; i < passes; ++i)
        {
            cmd.Blit(kuwaharaPasses[i - 1], kuwaharaPasses[i], KuwaharaMaterial);
        }

        cmd.Blit(kuwaharaPasses[passes - 1], source);

        for (int i = 0; i < passes; ++i)
        {
            RenderTexture.ReleaseTemporary(kuwaharaPasses[i]);
        }
    }

}