using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[Serializable, VolumeComponentMenu("Joseph_Acerola/Custom Blur")]
public class BlurVolume : VolumeComponent, IPostProcessComponent
{
    public BlurOperatorsParameter _blurOperator = new BlurOperatorsParameter(BlurOperators.Box, true);
    public IntParameter _kernelSize = new IntParameter(3, true);
    public ClampedFloatParameter _sigma = new ClampedFloatParameter(2.0f, 0.1f, 10.0f,true);
    public ClampedIntParameter _blurPasses = new ClampedIntParameter(1, 1, 10,true);

    public void load(Material material, ref RenderingData renderingData)
    {
        material.SetFloat("_KernelSize", _kernelSize.value);
        material.SetFloat("_Sigma", _sigma.value);
    }

    public bool IsActive() => true;
    public bool IsTileCompatible() => false;
}
public enum BlurOperators
{
    Box,
    Gaussian
}
[Serializable]
public sealed class BlurOperatorsParameter : VolumeParameter<BlurOperators>
{
    public BlurOperatorsParameter(BlurOperators value, bool overrideState = false) : base(value, overrideState) { }
}