using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ColorCorrectRenderFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class Settings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        public Shader shader;
    }
    public Settings settings = new Settings();

    ColorCorrectPass m_ColorCorrectPass;
    private void OnEnable()
    {
        settings.shader = Shader.Find("Joseph&Acerola/ColorCorrection");
    }
    public override void Create()
    {
        this.name = "Color Correct Pass";
        if (settings.shader == null)
        {
            Debug.LogWarning("No Shader");
            return;
        }
        m_ColorCorrectPass = new ColorCorrectPass(settings.renderPassEvent, settings.shader);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(m_ColorCorrectPass);
    }
}