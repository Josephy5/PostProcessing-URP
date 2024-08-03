using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class FogRenderFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class Settings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        public Shader shader;
    }
    public Settings settings = new Settings();

    FogPass m_FogPass;
    private void OnEnable()
    {
        settings.shader = Shader.Find("Joseph&Acerola/SSFog");
    }
    public override void Create()
    {
        this.name = "Fog Pass";
        if (settings.shader == null)
        {
            Debug.LogWarning("No Shader");
            return;
        }
        m_FogPass = new FogPass(settings.renderPassEvent, settings.shader);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(m_FogPass);
    }
}