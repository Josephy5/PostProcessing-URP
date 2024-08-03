using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[Serializable, VolumeComponentMenu("Joseph_Acerola/Custom Color Blindness")]
public class ColorBlindnessVolume : VolumeComponent, IPostProcessComponent
{
    public BlindTypeParameter _blindType = new BlindTypeParameter(BlindTypes.Protanomaly, true);
    public FloatParameter _severity = new ClampedFloatParameter(0f, 0f, 1f, true);
    public BoolParameter _difference = new BoolParameter(false, true);

    public void load(Material material, ref RenderingData renderingData)
    {
        material.SetFloat("_Severity", _severity.value);
        material.SetInt("_Difference", _difference.value ? 1 : 0);
    }

    public bool IsActive() => true;
    public bool IsTileCompatible() => false;
}
public enum BlindTypes
{
    Protanomaly = 0,
    Deuteranomaly,
    Tritanomaly
}
[Serializable]
public sealed class BlindTypeParameter : VolumeParameter<BlindTypes>
{
    public BlindTypeParameter(BlindTypes value, bool overrideState = false) : base(value, overrideState) { }
}