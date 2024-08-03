using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[Serializable, VolumeComponentMenu("Joseph_Acerola/Cutsom Tonemaps")]
public class TonemapVolume : VolumeComponent, IPostProcessComponent
{
    public TonemappersParameter _toneMapper = new TonemappersParameter(Tonemappers.RGBClamp,true);

    [Header("Tumblin Rushmeier & Ward Parameters")]
    // Tumblin Rushmeier Parameters
    public ClampedFloatParameter _Ldmax = new ClampedFloatParameter(0.0f, 0.0f, 10.0f,true);
    public ClampedFloatParameter _Cmax = new ClampedFloatParameter(0.0f, 0.0f, 10.0f,true);

    [Header("Schlick Parameters")]
    // Schlick Parameters
    public ClampedFloatParameter _p = new ClampedFloatParameter(0.0f, 0.0f, 10.0f,true);
    public ClampedFloatParameter _hiVal = new ClampedFloatParameter(0.0f, 0.0f, 10.0f,true);

    [Header("Reinhard Extended Parameters")]
    // Reinhard Extended Parameters
    public ClampedFloatParameter _Cwhite = new ClampedFloatParameter(0.0f, 0.0f, 10.0f,true);

    [Header("Hable Parameters")]
    // Hable Parameters
    public ClampedFloatParameter _shoulderStrength = new ClampedFloatParameter(0.0f, 0.0f, 10.0f,true);
    public ClampedFloatParameter _linearStrength = new ClampedFloatParameter(0.0f, 0.0f, 10.0f,true);
    public ClampedFloatParameter _linearAngle = new ClampedFloatParameter(0.0f, 0.0f, 10.0f,true);
    public ClampedFloatParameter _toeStrength = new ClampedFloatParameter(0.0f, 0.0f, 10.0f,true);
    public ClampedFloatParameter _toeNumerator = new ClampedFloatParameter(0.0f, 0.0f, 10.0f,true);
    public ClampedFloatParameter _toeDenominator = new ClampedFloatParameter(0.0f, 0.0f, 10.0f,true);
    public ClampedFloatParameter _linearWhitePoint = new ClampedFloatParameter(0.0f, 0.0f, 10.0f,true);

    [Header("Uchimura Parameters")]
    // Uchimura Parameters
    public ClampedFloatParameter _maxBrightness = new ClampedFloatParameter(0.0f, 0.0f, 10.0f,true);
    public ClampedFloatParameter _contrast = new ClampedFloatParameter(0.0f, 0.0f, 10.0f,true);
    public ClampedFloatParameter _linearStart = new ClampedFloatParameter(0.0f, 0.0f, 10.0f,true);
    public ClampedFloatParameter _linearLength = new ClampedFloatParameter(0.0f, 0.0f, 10.0f,true);
    public ClampedFloatParameter _blackTightnessShape = new ClampedFloatParameter(0.0f, 0.0f, 10.0f,true);
    public ClampedFloatParameter _blackTightnessOffset = new ClampedFloatParameter(0.0f, 0.0f, 10.0f,true);


    public void load(Material material, ref RenderingData renderingData)
    {
        material.SetFloat("_Ldmax", _Ldmax.value);
        material.SetFloat("_Cmax", _Cmax.value);
        material.SetFloat("_P", _p.value);
        material.SetFloat("_HiVal", _hiVal.value);
        material.SetFloat("_Cwhite", _Cwhite.value);
        material.SetFloat("_A", _shoulderStrength.value);
        material.SetFloat("_B", _linearStrength.value);
        material.SetFloat("_C", _linearAngle.value);
        material.SetFloat("_D", _toeStrength.value);
        material.SetFloat("_E", _toeNumerator.value);
        material.SetFloat("_F", _toeDenominator.value);
        material.SetFloat("_W", _linearWhitePoint.value);
        material.SetFloat("_M", _maxBrightness.value);
        material.SetFloat("_a", _contrast.value);
        material.SetFloat("_m", _linearStart.value);
        material.SetFloat("_l", _linearLength.value);
        material.SetFloat("_c", _blackTightnessShape.value);
        material.SetFloat("_b", _blackTightnessOffset.value);
    }

    public bool IsActive() => true;
    public bool IsTileCompatible() => false;
}
public enum Tonemappers
{
    DebugHDR = 1,
    RGBClamp,
    TumblinRushmeier,
    Schlick,
    Ward,
    Reinhard,
    ReinhardExtended,
    Hable,
    Uchimura,
    NarkowiczACES,
    HillACES
}
[Serializable]
public sealed class TonemappersParameter : VolumeParameter<Tonemappers>
{
    public TonemappersParameter(Tonemappers value, bool overrideState = false) : base(value, overrideState) { }
}