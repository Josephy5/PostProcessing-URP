using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class AnisotropicKuwaharaPass : ScriptableRenderPass
{
    static readonly string RenderPassTag = "Anisotropic Kuwahara";

    private AnisotropicKuwaharaVolume KuwaharaVolume;
    private Material KuwaharaMaterial;

    public AnisotropicKuwaharaPass(RenderPassEvent evt, Shader kuwaharashader)
    {
        renderPassEvent = evt;
        if (kuwaharashader == null)
        {
            Debug.LogError("No Anisotropic Kuwahara Shader");
            return;
        }
        KuwaharaMaterial = CoreUtils.CreateEngineMaterial(kuwaharashader);
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if (KuwaharaMaterial == null || !renderingData.cameraData.postProcessEnabled)
        {
            if (KuwaharaMaterial == null) Debug.LogError("No Anisotropic Kuwahara Material");
            return;
        }

        VolumeStack stack = VolumeManager.instance.stack;
        KuwaharaVolume = stack.GetComponent<AnisotropicKuwaharaVolume>();

        if (!KuwaharaVolume.IsActive())
        {
            return;
        }

        var cmd = CommandBufferPool.Get(RenderPassTag);
        Render(cmd, context, ref renderingData);
        context.ExecuteCommandBuffer(cmd);

        CommandBufferPool.Release(cmd);
    }
    void Render(CommandBuffer cmd, ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if (!KuwaharaVolume.IsActive()) return;
        KuwaharaVolume.load(KuwaharaMaterial, ref renderingData);

        int passes = (int) KuwaharaVolume._passes.value;
        int width = renderingData.cameraData.cameraTargetDescriptor.width;
        int height = renderingData.cameraData.cameraTargetDescriptor.height;

        // Get the source and destination render textures
        RenderTargetIdentifier source = renderingData.cameraData.renderer.cameraColorTarget;
        RenderTargetIdentifier destination = renderingData.cameraData.renderer.cameraColorTarget;

        //RenderTextureDescriptor descriptor = renderingData.cameraData.cameraTargetDescriptor;
        //descriptor.depthBufferBits = 0;

        var structuretensor = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.ARGB32);
        cmd.Blit(source, structuretensor, KuwaharaMaterial, 0);
        //Blit(cmd, source, structuretensor, KuwaharaMaterial, 0);

        var eigenvectors1 = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.ARGB32);
        cmd.Blit(structuretensor, eigenvectors1, KuwaharaMaterial, 1);
        //Blit(cmd, structuretensor, eigenvectors1, KuwaharaMaterial, 1);

        var eigenvectors2 = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.ARGB32);
        cmd.Blit(eigenvectors1, eigenvectors2, KuwaharaMaterial, 2);
        //Blit(cmd, eigenvectors1, eigenvectors2, KuwaharaMaterial, 2);

        KuwaharaMaterial.SetTexture("_TFM", eigenvectors2);

        var kuwaharapasses = new RenderTexture[passes];
        for (int i = 0; i < passes; ++i)
        {
            kuwaharapasses[i] = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.ARGB32);
        }

        cmd.Blit(source, kuwaharapasses[0], KuwaharaMaterial, 3);
        //Blit(cmd, source, kuwaharapasses[0], KuwaharaMaterial, 3);

        for (int i = 1; i < passes; ++i)
        {
            cmd.Blit(kuwaharapasses[i - 1], kuwaharapasses[i], KuwaharaMaterial, 3);
            //Blit(cmd, kuwaharapasses[i - 1], kuwaharapasses[i], KuwaharaMaterial, 3);
        }

        cmd.Blit(kuwaharapasses[passes - 1], destination);
        //Blit(cmd, kuwaharapasses[passes - 1], destination);

        RenderTexture.ReleaseTemporary(structuretensor);
        RenderTexture.ReleaseTemporary(eigenvectors1);
        RenderTexture.ReleaseTemporary(eigenvectors2);
        for (int i = 0; i < passes; ++i)
        {
            RenderTexture.ReleaseTemporary(kuwaharapasses[i]);
        }
    }
}
