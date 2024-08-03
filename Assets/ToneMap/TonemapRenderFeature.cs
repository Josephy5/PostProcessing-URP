using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class TonemapRenderFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class Settings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        public Shader shader;
        //[Range(2, 10)] public int sampleTimes = 2;
    }
    public Settings settings = new Settings();

    TonemapPass m_tonemapPass;
    private void OnEnable()
    {
        settings.shader = Shader.Find("Joseph&Acerola/Tonemapping");
    }
    public override void Create()
    {
        this.name = "Tonemap Pass";
        if (settings.shader == null)
        {
            Debug.LogWarning("No Shader");
            return;
        }
        m_tonemapPass = new TonemapPass(settings.renderPassEvent, settings.shader);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(m_tonemapPass);
    }
}