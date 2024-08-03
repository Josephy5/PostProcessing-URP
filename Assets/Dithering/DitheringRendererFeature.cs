using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DitheringRendererFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class Settings
    {
        public RenderPassEvent RenderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        public Shader shader;
    }

    public Settings settings = new Settings();

    DitheringPass DitheringPass;
    private void OnEnable()
    {
        settings.shader = Shader.Find("Joseph&Acerola/Dither");
    }
    public override void Create()
    {
        this.name = "Dithering Pass";
        if(settings.shader == null)
        {
            Debug.LogWarning("No Shader");
            return;
        }
        DitheringPass = new DitheringPass(settings.RenderPassEvent, settings.shader);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(DitheringPass);
    }
}
