using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[Serializable, VolumeComponentMenu("Joseph_Acerola/Custom Acerola Bloom")]
public class BloomAcerolaVolume : VolumeComponent, IPostProcessComponent
{
    public ClampedFloatParameter _threshold = new ClampedFloatParameter(1.0f, 0f, 10f, true);
    public ClampedFloatParameter _softThreshold = new ClampedFloatParameter(1f, 0f, 1f, true);
    public ClampedIntParameter _downSamples = new ClampedIntParameter(1, 1, 16, true);
    public ClampedFloatParameter _downSampleDelta = new ClampedFloatParameter(1.0f, 0.01f, 2.0f, true);
    public ClampedFloatParameter _upSampleDelta = new ClampedFloatParameter(0.5f, 0.01f, 2f, true);
    public ClampedFloatParameter _bloomIntensity = new ClampedFloatParameter(1f, 0f, 10f, true);

    public void load(Material material, ref RenderingData renderingData)
    {
        material.SetFloat("_Threshold", _threshold.value);
        material.SetFloat("_SoftThreshold", _softThreshold.value);
        material.SetFloat("_DownDelta", _downSampleDelta.value);
        material.SetFloat("_UpDelta", _upSampleDelta.value);
        //material.SetTexture("_OriginalTex", _source.value);
        material.SetFloat("_Intensity", _bloomIntensity.value);
    }

    public bool IsActive() => true;
    public bool IsTileCompatible() => false;
}