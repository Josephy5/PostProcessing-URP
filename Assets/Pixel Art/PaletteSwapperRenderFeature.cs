using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PaletteSwapperRenderFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class Settings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        public Shader shader;
    }
    public Settings settings = new Settings();

    PaletteSwapperPass m_PaletteSwapperPass;
    private void OnEnable()
    {
        settings.shader = Shader.Find("Joseph&Acerola/PaletteSwapper");
    }
    public override void Create()
    {
        this.name = "Palette Swapper Pass";
        if (settings.shader == null)
        {
            Debug.LogWarning("No Shader");
            return;
        }
        m_PaletteSwapperPass = new PaletteSwapperPass(settings.renderPassEvent, settings.shader);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(m_PaletteSwapperPass);
    }
}