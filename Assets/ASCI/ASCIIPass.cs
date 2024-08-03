using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ASCIIPass : ScriptableRenderPass
{
    static readonly string renderPassTag = "ASCII";

    private ASCIIVolume ASCIIVolume;
    private Material ASCIIMaterial;
    private ComputeShader asciiCompute;

    public ASCIIPass(RenderPassEvent evt, Shader ASCIIshader, ComputeShader ASCIIcomputeShader)
    {
        renderPassEvent = evt;
        var shader = ASCIIshader;
        if (shader == null)
        {
            Debug.LogError("No Shader");
            return;
        }
        ASCIIMaterial = CoreUtils.CreateEngineMaterial(ASCIIshader);
        asciiCompute = ASCIIcomputeShader;
    }


    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if (ASCIIMaterial == null)
        {
            Debug.LogError("No ASCII Material");
            return;
        }

        if (!renderingData.cameraData.postProcessEnabled)
        {
            //Debug.LogError("Post Processing in Camera not enabled");
            return;
        }

        VolumeStack stack = VolumeManager.instance.stack;
        ASCIIVolume = stack.GetComponent<ASCIIVolume>();

        var cmd = CommandBufferPool.Get(renderPassTag);
        Render(cmd, ref renderingData);
        context.ExecuteCommandBuffer(cmd);

        CommandBufferPool.Release(cmd);
    }

    void Render(CommandBuffer cmd, ref RenderingData renderingData)
    {
        if (ASCIIVolume.IsActive() == false) return;
        ASCIIVolume.load(ASCIIMaterial, ref renderingData);

        int width = renderingData.cameraData.cameraTargetDescriptor.width;
        int height = renderingData.cameraData.cameraTargetDescriptor.height;
        var source = renderingData.cameraData.renderer.cameraColorTarget;

        var ping = RenderTexture.GetTemporary(width, height, 0, renderingData.cameraData.cameraTargetDescriptor.colorFormat);

        var luminance = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.RHalf);
        var sobel = RenderTexture.GetTemporary(width, height, 0, renderingData.cameraData.cameraTargetDescriptor.colorFormat);
        var dog = RenderTexture.GetTemporary(width, height, 0, renderingData.cameraData.cameraTargetDescriptor.colorFormat);

        cmd.Blit(source, luminance, ASCIIMaterial, 1); //Luminance
        cmd.Blit(luminance, ping, ASCIIMaterial, 3); // Horizontal Blur
        cmd.Blit(ping, dog, ASCIIMaterial, 4); // Vertical Blur and Difference
        ASCIIMaterial.SetTexture("_LuminanceTex", luminance);

        cmd.Blit(dog, ping, ASCIIMaterial, 5); // Sobel Horizontal Pass
        cmd.Blit(ping, sobel, ASCIIMaterial, 6); // Sobel Vertical Pass
        cmd.Blit(source, ping, ASCIIMaterial, 2); // Pack luminance

        var downscale1 = RenderTexture.GetTemporary(width / 2, height / 2, 0, renderingData.cameraData.cameraTargetDescriptor.colorFormat);
        var downscale2 = RenderTexture.GetTemporary(width / 4, height / 4, 0, renderingData.cameraData.cameraTargetDescriptor.colorFormat);
        var downscale3 = RenderTexture.GetTemporary(width / 8, height / 8, 0, renderingData.cameraData.cameraTargetDescriptor.colorFormat);

        cmd.Blit(ping, downscale1, ASCIIMaterial, 0);
        cmd.Blit(downscale1, downscale2, ASCIIMaterial, 0);
        cmd.Blit(downscale2, downscale3, ASCIIMaterial, 0);

        var asciiRenderTex = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGB32);
        asciiRenderTex.enableRandomWrite = true;
        asciiRenderTex.Create();
        asciiCompute.SetTexture(0, "_SobelTex", sobel);
        asciiCompute.SetTexture(0, "_Result", asciiRenderTex);
        asciiCompute.SetTexture(0, "_EdgeAsciiTex", ASCIIVolume._edgeTex.value);
        asciiCompute.SetTexture(0, "_AsciiTex", ASCIIVolume._asciiTex.value);
        asciiCompute.SetTexture(0, "_LuminanceTex", downscale3);
        asciiCompute.SetInt("_ViewUncompressed", ASCIIVolume._viewUncompressedEdges.value ? 1 : 0);
        asciiCompute.SetInt("_DebugEdges", ASCIIVolume._debugEdges.value ? 1 : 0);
        asciiCompute.SetInt("_Grid", ASCIIVolume._viewGrid.value ? 1 : 0);
        asciiCompute.SetInt("_NoEdges", ASCIIVolume._noEdges.value ? 1 : 0);
        asciiCompute.SetInt("_NoFill", ASCIIVolume._noFill.value ? 1 : 0);
        asciiCompute.SetInt("_EdgeThreshold", ASCIIVolume._edgeThreshold.value);
        asciiCompute.SetFloat("_Exposure", ASCIIVolume._exposure.value);
        asciiCompute.SetFloat("_Attenuation", ASCIIVolume._attenuation.value);
        asciiCompute.Dispatch(0, Mathf.CeilToInt(width / 8), Mathf.CeilToInt(width / 8), 1);

        cmd.Blit(asciiRenderTex, source);

        if (ASCIIVolume._viewDog.value)
            cmd.Blit(dog, source, ASCIIMaterial, 0);

        if (ASCIIVolume._viewSobel.value)
            cmd.Blit(sobel, source, ASCIIMaterial, 0);

        if (ASCIIVolume._viewQuantizedSobel.value || ASCIIVolume._viewUncompressedEdges.value || ASCIIVolume._debugEdges.value || ASCIIVolume._viewGrid.value)
            cmd.Blit(asciiRenderTex, source, ASCIIMaterial, 0);

        RenderTexture.ReleaseTemporary(ping);
        RenderTexture.ReleaseTemporary(luminance);
        RenderTexture.ReleaseTemporary(sobel);
        RenderTexture.ReleaseTemporary(downscale1);
        RenderTexture.ReleaseTemporary(downscale2);
        RenderTexture.ReleaseTemporary(downscale3);
        RenderTexture.ReleaseTemporary(dog);
    }

}