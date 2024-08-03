using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DifferenceOfGaussiansRenderFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class Settings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        public Shader shader;
    }
    public Settings settings = new Settings();

    DifferenceOfGaussiansPass m_DifferenceOfGaussiansPass;
    private void OnEnable()
    {
        settings.shader = Shader.Find("Joseph&Acerola/DifferenceOfGaussians");
    }
    public override void Create()
    {
        this.name = "DoG Pass";
        if (settings.shader == null)
        {
            Debug.LogWarning("No Shader");
            return;
        }
        m_DifferenceOfGaussiansPass = new DifferenceOfGaussiansPass(settings.renderPassEvent, settings.shader);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(m_DifferenceOfGaussiansPass);
    }
}