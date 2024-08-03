using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GeneralizedKuwaharaRenderFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class Settings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        public Shader shader;
    }
    public Settings settings = new Settings();

    GeneralizedKuwaharaPass m_GeneralizedKuwaharaPass;
    private void OnEnable()
    {
        settings.shader = Shader.Find("Joseph&Acerola/GeneralizedKuwahara");
    }
    public override void Create()
    {
        this.name = "Generalized Kuwahara Pass";
        if (settings.shader == null)
        {
            Debug.LogWarning("No Shader");
            return;
        }
        m_GeneralizedKuwaharaPass = new GeneralizedKuwaharaPass(settings.renderPassEvent, settings.shader);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(m_GeneralizedKuwaharaPass);
    }
}