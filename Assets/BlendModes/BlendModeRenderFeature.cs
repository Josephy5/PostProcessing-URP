using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class BlendModeRenderFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class Settings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        public Shader shader;
        //[Range(2, 10)] public int sampleTimes = 2;
    }
    public Settings settings = new Settings();

    BlendModePass m_BlendModePass;
    private void OnEnable()
    {
        settings.shader = Shader.Find("Joseph&Acerola/BlendModes");
    }
    public override void Create()
    {
        this.name = "Blend Mode Pass";
        if (settings.shader == null)
        {
            Debug.LogWarning("No Shader");
            return;
        }
        m_BlendModePass = new BlendModePass(settings.renderPassEvent, settings.shader);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(m_BlendModePass);
    }
}