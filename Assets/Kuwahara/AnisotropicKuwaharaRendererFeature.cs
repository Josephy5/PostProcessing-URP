using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class AnisotropicKuwaharaRendererFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class Settings
    {
        public RenderPassEvent RenderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        public Shader shader;
    }

    public Settings settings = new Settings();

    AnisotropicKuwaharaPass KuwaharaPass;
    private void OnEnable()
    {
        settings.shader = Shader.Find("Joseph&Acerola/AnisotropicKuwahara");
    }
    public override void Create()
    {
        this.name = "Anisotropic Kuwahara Pass";
        if(settings.shader == null)
        {
            Debug.LogWarning("No Shader");
            return;
        }
        KuwaharaPass = new AnisotropicKuwaharaPass(settings.RenderPassEvent, settings.shader);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(KuwaharaPass);
    }
}
