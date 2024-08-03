using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[Serializable, VolumeComponentMenu("Joseph_Acerola/Custom Lens Flare")]
public class LensFlareVolume : VolumeComponent, IPostProcessComponent
{
    public FloatParameter _threshold = new ClampedFloatParameter(0.5f, 0f, 1f, true);
    public IntParameter _sampleCount = new ClampedIntParameter(16, 1, 32, true);
    public FloatParameter _sampleDistance = new ClampedFloatParameter(0.01f, 0f, 0.1f, true);
    public FloatParameter _haloRadius = new ClampedFloatParameter(1f, 0f, 1f, true);
    public FloatParameter _haloThickness = new ClampedFloatParameter(0.5f, 0f, 1f, true);
    public Vector3Parameter _channelOffsets = new Vector3Parameter(Vector3.zero, true);
    public IntParameter _kernelSize = new ClampedIntParameter(2, 1, 20, true);
    public FloatParameter _sigma = new ClampedFloatParameter(1f, 0f, 10f, true);

    public void load(Material material, ref RenderingData renderingData)
    {
        material.SetFloat("_Threshold", _threshold.value);
        material.SetVector("_ColorOffsets", _channelOffsets.value);
        material.SetInt("_KernelSize", _kernelSize.value);
        material.SetFloat("_Sigma", _sigma.value);
        material.SetInt("_SampleCount", _sampleCount.value);
        material.SetFloat("_SampleDistance", _sampleDistance.value);
        material.SetFloat("_HaloRadius", _haloRadius.value);
        material.SetFloat("_HaloThickness", _haloThickness.value);
    }

    public bool IsActive() => true;
    public bool IsTileCompatible() => false;
}