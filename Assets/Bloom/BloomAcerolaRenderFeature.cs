using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class BloomAcerolaRenderFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class Settings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        public Shader shader;
    }
    public Settings settings = new Settings();

    BloomAcerolaPass m_BloomAcerolaPass;
    private void OnEnable()
    {
        settings.shader = Shader.Find("Joseph&Acerola/Bloom");
    }
    public override void Create()
    {
        this.name = "Acerola Bloom Pass";
        if (settings.shader == null)
        {
            Debug.LogWarning("No Shader");
            return;
        }
        m_BloomAcerolaPass = new BloomAcerolaPass(settings.renderPassEvent, settings.shader);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(m_BloomAcerolaPass);
    }
}