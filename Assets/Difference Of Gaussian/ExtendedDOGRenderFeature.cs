using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ExtendedDOGFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class Settings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        public Shader shader;
    }
    public Settings settings = new Settings();

    ExtendedDoGPass m_ExtendedDoGPass;
    private void OnEnable()
    {
        settings.shader = Shader.Find("Joseph&Acerola/ExtendedDoG");
    }
    public override void Create()
    {
        this.name = "Extended DoG Pass";
        if (settings.shader == null)
        {
            Debug.LogWarning("No Shader");
            return;
        }
        m_ExtendedDoGPass = new ExtendedDoGPass(settings.renderPassEvent, settings.shader);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(m_ExtendedDoGPass);
    }
}