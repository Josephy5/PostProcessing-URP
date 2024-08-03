using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[Serializable, VolumeComponentMenu("Joseph_Acerola/Custom Difference Of Gaussian")]
public class DifferenceOfGaussiansVolume : VolumeComponent, IPostProcessComponent
{
    public ClampedIntParameter _gaussianKernelSize = new ClampedIntParameter(5,1,10, true);

    public ClampedFloatParameter _stdev = new ClampedFloatParameter(2.0f,0.1f,5f,true);

    public ClampedFloatParameter _stdevScale = new ClampedFloatParameter(1.6f,0.1f,5f,true);

    public ClampedFloatParameter _tau = new ClampedFloatParameter(1.0f,0.01f,5.0f,true);

    public BoolParameter _thresholding = new BoolParameter(true,true);

    public BoolParameter _tanh = new BoolParameter(false,true);

    public ClampedFloatParameter _phi = new ClampedFloatParameter(1.0f,0.01f, 100f,true);

    public ClampedFloatParameter _threshold = new ClampedFloatParameter(0.005f,-1f,1f,true);
    
    public BoolParameter _invert = new BoolParameter(false, true);

    public void load(Material material, ref RenderingData renderingData)
    {
        material.SetInt("_GaussianKernelSize", _gaussianKernelSize.value);
        material.SetFloat("_Sigma", _stdev.value);
        material.SetFloat("_K", _stdevScale.value);
        material.SetFloat("_Tau", _tau.value);
        material.SetFloat("_Phi", _phi.value);
        material.SetFloat("_Threshold", _threshold.value);
        material.SetInt("_Thresholding", _thresholding.value ? 1 : 0);
        material.SetInt("_Invert", _invert.value ? 1 : 0);
        material.SetInt("_Tanh", _tanh.value ? 1 : 0);
    }

    public bool IsActive() => true;
    public bool IsTileCompatible() => false;
}