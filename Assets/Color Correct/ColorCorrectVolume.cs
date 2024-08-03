using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[Serializable, VolumeComponentMenu("Joseph_Acerola/Custom Color Correct")]
public class ColorCorrectVolume : VolumeComponent, IPostProcessComponent
{
    public ColorParameter _colorFilter = new ColorParameter(Color.white, true, true, true, true);
    public Vector3Parameter _exposure = new Vector3Parameter(Vector3.one, true);
    public FloatParameter _temperature = new ClampedFloatParameter(0f, -100f, 100f, true);
    public FloatParameter _tint = new ClampedFloatParameter(0f, -100f, 100f, true);
    public Vector3Parameter _contrast = new Vector3Parameter(Vector3.one, true);
    public Vector3Parameter _brightness = new Vector3Parameter(Vector3.zero, true);
    public Vector3Parameter _saturation = new Vector3Parameter(Vector3.one, true);
    public Vector3Parameter _linearMidPoint = new Vector3Parameter(new Vector3(0.5f, 0.5f, 0.5f), true);

    public void load(Material material, ref RenderingData renderingData)
    {
        material.SetVector("_Exposure", _exposure.value);
        material.SetVector("_Contrast", _contrast.value);
        material.SetVector("_MidPoint", _linearMidPoint.value);
        material.SetVector("_Brightness", _brightness.value);
        material.SetVector("_ColorFilter", _colorFilter.value);
        material.SetVector("_Saturation", _saturation.value);
        material.SetFloat("_Temperature", _temperature.value / 100.0f);
        material.SetFloat("_Tint", _tint.value / 100.0f);
    }

    public bool IsActive() => true;
    public bool IsTileCompatible() => false;
}