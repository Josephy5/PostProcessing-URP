using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[Serializable, VolumeComponentMenu("Joseph_Acerola/Custom Vignette")]
public class VignetteVolume : VolumeComponent, IPostProcessComponent
{
    public ColorParameter _vignetteColor = new ColorParameter(Color.black, true, true, true, true);
    public Vector2Parameter _vignetteOffset = new Vector2Parameter(Vector2.zero, true);
    public Vector2Parameter _vignetteSize = new Vector2Parameter(Vector2.one, true);
    public ClampedFloatParameter _intensity = new ClampedFloatParameter(1.0f, 0.0f, 5.0f, true);
    public ClampedFloatParameter _roundness = new ClampedFloatParameter(1.0f, 0.0f, 10.0f, true);
    public ClampedFloatParameter _smoothness = new ClampedFloatParameter(1.0f, 0.0f, 10.0f, true);

    public void load(Material material, ref RenderingData renderingData)
    {
        material.SetColor("_VignetteColor", _vignetteColor.value);
        material.SetVector("_VignetteOffset", _vignetteOffset.value);
        material.SetVector("_VignetteSize", _vignetteSize.value);
        material.SetFloat("_Intensity", _intensity.value);
        material.SetFloat("_Roundness", _roundness.value);
        material.SetFloat("_Smoothness", _smoothness.value);
    }

    public bool IsActive() => true;
    public bool IsTileCompatible() => false;
}