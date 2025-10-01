using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class AnisotropicKuwaharaRendererFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class Settings
    {
        public RenderPassEvent RenderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
        //the effect's shader, will automatically be assigned
        public Shader shader;
        //having the effect on for scene view can be annoying. By default its on, you can turn it off if you want it on
        public bool showInSceneView = false;
    }
    public Settings settings = new Settings();

    AnisotropicKuwaharaPass KuwaharaPass;

    //When render feature object is enabled, set the shader
    private void OnEnable()
    {
        settings.shader = Shader.Find("Hidden/AnisotropicKuwahara");
    }
    //sets the hatching's render pass up
    public override void Create()
    {
        this.name = "Anisotropic Kuwahara Pass";
        if(settings.shader == null)
        {
            Debug.LogWarning("No Anisotropic Kuwahara Shader");
            return;
        }
        KuwaharaPass = new AnisotropicKuwaharaPass(settings.RenderPassEvent, settings.shader, settings.showInSceneView);
    }
    //call and adds the kuwahara render pass to the scriptable renderer's queue
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(KuwaharaPass);
    }
}
