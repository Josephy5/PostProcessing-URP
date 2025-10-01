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

    private static ProfilingSampler ProfilingSampler;

    //If user wants the Kuwahara Effect to be viewable in scene view
    bool showInSceneView = false;
    public AnisotropicKuwaharaPass(RenderPassEvent evt, Shader kuwaharashader, bool val)
    {
        renderPassEvent = evt;
        if (kuwaharashader == null)
        {
            Debug.LogError("No Anisotropic Kuwahara Shader");
            return;
        }
        //to make profiling easier
        ProfilingSampler = new ProfilingSampler(RenderPassTag);

        KuwaharaMaterial = CoreUtils.CreateEngineMaterial(kuwaharashader);
        showInSceneView = val;
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if (KuwaharaMaterial == null)
        {
            Debug.LogError("No Anisotropic Kuwahara Material");
            return;
        }
        //in case if the camera doesn't have the post process option enabled and if the camera is not the game's camera
        if (renderingData.cameraData.cameraType != CameraType.Game && (showInSceneView == false && renderingData.cameraData.cameraType == CameraType.SceneView))
        {
            return;
        }

        VolumeStack stack = VolumeManager.instance.stack;
        KuwaharaVolume = stack.GetComponent<AnisotropicKuwaharaVolume>();

        var cmd = CommandBufferPool.Get(RenderPassTag);
        Render(cmd, context, ref renderingData);

        context.ExecuteCommandBuffer(cmd);
        cmd.Clear();

        CommandBufferPool.Release(cmd);
    }
    void Render(CommandBuffer cmd, ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if (!KuwaharaVolume.IsActive()) return;
        KuwaharaVolume.load(KuwaharaMaterial, ref renderingData);

        //for profiling
        using (new ProfilingScope(cmd, ProfilingSampler))
        {
            int passes = (int)KuwaharaVolume._passes.value;
            int width = renderingData.cameraData.cameraTargetDescriptor.width;
            int height = renderingData.cameraData.cameraTargetDescriptor.height;

            // Get the source and destination render textures
            var source = renderingData.cameraData.renderer.cameraColorTargetHandle;
            var destination = renderingData.cameraData.renderer.cameraColorTargetHandle;

            var structuretensor = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.ARGB32);
            cmd.Blit(source, structuretensor, KuwaharaMaterial, 0);

            var eigenvectors1 = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.ARGB32);
            cmd.Blit(structuretensor, eigenvectors1, KuwaharaMaterial, 1);

            var eigenvectors2 = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.ARGB32);
            cmd.Blit(eigenvectors1, eigenvectors2, KuwaharaMaterial, 2);

            KuwaharaMaterial.SetTexture("_TemporaryTexture", eigenvectors2);

            var kuwaharapasses = new RenderTexture[passes];
            for (int i = 0; i < passes; ++i)
            {
                kuwaharapasses[i] = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.ARGB32);
            }

            cmd.Blit(source, kuwaharapasses[0], KuwaharaMaterial, 3);

            for (int i = 1; i < passes; ++i)
            {
                cmd.Blit(kuwaharapasses[i - 1], kuwaharapasses[i], KuwaharaMaterial, 3);
            }

            cmd.Blit(kuwaharapasses[passes - 1], destination);

            RenderTexture.ReleaseTemporary(structuretensor);
            RenderTexture.ReleaseTemporary(eigenvectors1);
            RenderTexture.ReleaseTemporary(eigenvectors2);
            for (int i = 0; i < passes; ++i)
            {
                RenderTexture.ReleaseTemporary(kuwaharapasses[i]);
            }

        }
    }
}
