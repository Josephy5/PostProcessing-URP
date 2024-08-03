using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[VolumeComponentMenu("Joseph_Acerola/Custom Sharpness")]
public class SharpnessVolume : VolumeComponent, IPostProcessComponent
{
    public ClampedFloatParameter _amount = new ClampedFloatParameter(0.0f, -10.0f, 10.0f, true);
    public SharpnessModeParameter _sharpnessMode = new SharpnessModeParameter(SharpnessMode.Normal, true);

    public void load(Material material, ref RenderingData renderingData)
    {
        material.SetFloat("_Amount", _amount.value);
    }

    public bool IsActive() => true;
    public bool IsTileCompatible() => false;
}
public enum SharpnessMode
{
    Normal,
    Contrast
}

[Serializable]
public sealed class SharpnessModeParameter : VolumeParameter<SharpnessMode>
{
    public SharpnessModeParameter(SharpnessMode value, bool overrideState = false) : base(value, overrideState) { }
}