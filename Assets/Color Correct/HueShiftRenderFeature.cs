using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class HueShiftRenderFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class Settings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        public Shader shader;
    }
    public Settings settings = new Settings();

    HueShiftPass m_HueShiftPass;
    private void OnEnable()
    {
        settings.shader = Shader.Find("Joseph&Acerola/HueShift");
    }
    public override void Create()
    {
        this.name = "Hue Shift Pass";
        if (settings.shader == null)
        {
            Debug.LogWarning("No Shader");
            return;
        }
        m_HueShiftPass = new HueShiftPass(settings.renderPassEvent, settings.shader);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(m_HueShiftPass);
    }
}