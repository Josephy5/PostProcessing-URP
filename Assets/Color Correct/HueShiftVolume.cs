using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[Serializable, VolumeComponentMenu("Joseph_Acerola/Custom Hue Shift Correct")]
public class HueShiftVolume : VolumeComponent, IPostProcessComponent
{
    public FloatParameter _shift = new ClampedFloatParameter(0f, 0f, 1f, true);

    public void load(Material material, ref RenderingData renderingData)
    {
        material.SetFloat("_HueShift", _shift.value);
    }

    public bool IsActive() => true;
    public bool IsTileCompatible() => false;
}