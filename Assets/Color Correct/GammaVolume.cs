using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[Serializable, VolumeComponentMenu("Joseph_Acerola/Custom Gamma Correct")]
public class GammaVolume : VolumeComponent, IPostProcessComponent
{
    public FloatParameter _gamma = new ClampedFloatParameter(1f, 0f, 10f, true);

    public void load(Material material, ref RenderingData renderingData)
    {
        material.SetFloat("_Gamma", _gamma.value);
    }

    public bool IsActive() => true;
    public bool IsTileCompatible() => false;
}