using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[Serializable, VolumeComponentMenu("Joseph_Acerola/Custom Fog")]
public class FogVolume : VolumeComponent, IPostProcessComponent
{
    public ColorParameter _fogColor = new ColorParameter(Color.white, true, true, true, true);
    public FloatParameter _fogDensity = new ClampedFloatParameter(0f, 0f, 1f, true);
    public FloatParameter _fogOffset = new ClampedFloatParameter(0f, 0f, 100f, true);

    public void load(Material material, ref RenderingData renderingData)
    {
        material.SetVector("_FogColor", _fogColor.value);
        material.SetFloat("_FogDensity", _fogDensity.value);
        material.SetFloat("_FogOffset", _fogOffset.value);
    }

    public bool IsActive() => true;
    public bool IsTileCompatible() => false;
}