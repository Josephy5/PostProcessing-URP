using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ASCIIRenderFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class Settings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        public Shader shader;
        public ComputeShader computeShader;
    }
    public Settings settings = new Settings();

    ASCIIPass m_ASCIIPass;
    private void OnEnable()
    {
        settings.shader = Shader.Find("Joseph&Acerola/ASCII");
    }
    public override void Create()
    {
        this.name = "ASCII Pass";
        if (settings.shader == null)
        {
            Debug.LogWarning("No Shader");
            return;
        }
        m_ASCIIPass = new ASCIIPass(settings.renderPassEvent, settings.shader, settings.computeShader);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(m_ASCIIPass);
    }
}