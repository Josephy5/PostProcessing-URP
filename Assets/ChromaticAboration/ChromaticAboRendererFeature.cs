using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ChromaticAboRendererFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class Settings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        public Shader shader;
    }

    public Settings settings = new Settings();

    ChromaticAboPass m_ChromaticPass;

    private void OnEnable()
    {
        settings.shader = Shader.Find("Joseph&Acerola/ChromaticAberration");
    }
    public override void Create()
    {
        this.name = "Chromatic Aboration Pass";
        if(settings.shader == null)
        {
            Debug.LogWarning("No Shader");
            return;
        }
        m_ChromaticPass = new ChromaticAboPass(settings.renderPassEvent, settings.shader);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(m_ChromaticPass);
    }
}
