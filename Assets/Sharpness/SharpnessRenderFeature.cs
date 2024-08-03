using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SharpnessRenderFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class Settings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        //public Shader shader;
    }
    public Settings settings = new Settings();

    SharpnessPass m_sharpnessPass;

    public override void Create()
    {
        this.name = "Sharpness Pass";
        //if (settings.shader == null)
        //{
            //Debug.LogWarning("No Shader");
            //return;
        //}
        m_sharpnessPass = new SharpnessPass(settings.renderPassEvent);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(m_sharpnessPass);
    }
}