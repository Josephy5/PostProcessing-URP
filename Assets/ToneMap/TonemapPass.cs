using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class TonemapPass : ScriptableRenderPass
{
    static readonly string renderPassTag = "Tonemapper";

    private TonemapVolume tonemapVolume;
    private Material tonemapMaterial;

    private RenderTexture grayscale;

    public TonemapPass(RenderPassEvent evt, Shader tonemapshader)
    {
        renderPassEvent = evt;
        var shader = tonemapshader;
        if (shader == null)
        {
            Debug.LogError("No Shader");
            return;
        }
        tonemapMaterial = CoreUtils.CreateEngineMaterial(tonemapshader);

        /*
        if(grayscale == null)
        {
            grayscale = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.RHalf, RenderTextureReadWrite.Linear);
            grayscale.useMipMap = true;
            grayscale.Create();
        }
        */
    }


    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if (tonemapMaterial == null)
        {
            Debug.LogError("No Tonemap Material");
            return;
        }

        if (!renderingData.cameraData.postProcessEnabled || renderingData.cameraData.cameraType == CameraType.SceneView)
        {
            //Debug.LogError("Post Processing in Camera not enabled");
            return;
        }

        VolumeStack stack = VolumeManager.instance.stack;
        tonemapVolume = stack.GetComponent<TonemapVolume>();

        var cmd = CommandBufferPool.Get(renderPassTag);
        Render(cmd, ref renderingData);
        context.ExecuteCommandBuffer(cmd);

        CommandBufferPool.Release(cmd);
    }
    
    void Render(CommandBuffer cmd, ref RenderingData renderingData)
    {
        if (tonemapVolume.IsActive() == false) return;
        tonemapVolume.load(tonemapMaterial, ref renderingData);

        if (grayscale == null)
        {
            grayscale = RenderTexture.GetTemporary(Screen.width, Screen.height, 0, RenderTextureFormat.RHalf, RenderTextureReadWrite.Linear);
            grayscale.useMipMap = true;
            grayscale.Create();
        }

        var source = renderingData.cameraData.renderer.cameraColorTarget;

        cmd.Blit(source, grayscale, tonemapMaterial, 0);

        tonemapMaterial.SetTexture("_LuminanceTex", grayscale);

        cmd.Blit(source, source, tonemapMaterial, (int)tonemapVolume._toneMapper.value);

        RenderTexture.ReleaseTemporary(grayscale);
    }

}