using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[Serializable, VolumeComponentMenu("Joseph_Acerola/Custom Kuwahara")]
public class KuwaharaVolume : VolumeComponent, IPostProcessComponent
{
    public ClampedIntParameter _kernelSize = new ClampedIntParameter(1, 1, 20, true);
    public BoolParameter _animateKernelSize = new BoolParameter(false, true);
    public ClampedIntParameter _minKernelSize = new ClampedIntParameter(1, 1, 20, true);
    public ClampedFloatParameter _sizeAnimationSpeed = new ClampedFloatParameter(1f, 0.1f, 5f, true);
    public ClampedFloatParameter _noiseFrequency = new ClampedFloatParameter(10f, 0f, 30f, true);
    public BoolParameter _animateKernelOrigin = new BoolParameter(false, true);
    public ClampedIntParameter _passes = new ClampedIntParameter(1, 1, 4, true);

    public void load(Material material, ref RenderingData renderingData)
    {
        material.SetInt("_KernelSize", _kernelSize.value);
        material.SetInt("_MinKernelSize", _minKernelSize.value);
        material.SetInt("_AnimateSize", _animateKernelSize.value ? 1 : 0);
        material.SetFloat("_SizeAnimationSpeed", _sizeAnimationSpeed.value);
        material.SetFloat("_NoiseFrequency", _noiseFrequency.value);
        material.SetInt("_AnimateOrigin", _animateKernelOrigin.value ? 1 : 0);
    }

    public bool IsActive() => true;
    public bool IsTileCompatible() => false;
}