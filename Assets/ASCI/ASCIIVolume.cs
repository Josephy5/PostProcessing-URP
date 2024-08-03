using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[Serializable, VolumeComponentMenu("Joseph_Acerola/Custom ASCII")]
public class ASCIIVolume : VolumeComponent, IPostProcessComponent
{
    [Header("Textures")]
    public TextureParameter _asciiTex = new TextureParameter(null, true);
    public TextureParameter _edgeTex = new TextureParameter(null, true);

    [Header("Parameters")]
    public ClampedIntParameter _gaussianKernelSize = new ClampedIntParameter(2, 1, 10, true);
    public ClampedFloatParameter _stdev = new ClampedFloatParameter(2.0f, 0.1f, 5f, true);
    public ClampedFloatParameter _stdevScale = new ClampedFloatParameter(1.6f,0.1f,5f, true);
    public ClampedFloatParameter _tau = new ClampedFloatParameter(1.0f,0.01f,5f, true);
    public ClampedFloatParameter _threshold = new ClampedFloatParameter(0.005f,0.001f, 0.1f, true);
    public ClampedIntParameter _edgeThreshold = new ClampedIntParameter(8, 0, 64, true);
    public ClampedFloatParameter _exposure = new ClampedFloatParameter(1.0f, 0f, 10f, true);
    public ClampedFloatParameter _attenuation = new ClampedFloatParameter(1.0f, 0f, 10f, true);

    [Header("Extras")]
    public BoolParameter _invert = new BoolParameter(false, true);
    public BoolParameter _viewDog = new BoolParameter(false, true);
    public BoolParameter _viewSobel = new BoolParameter(false, true);
    public BoolParameter _viewGrid = new BoolParameter(false, true);
    public BoolParameter _debugEdges = new BoolParameter(false, true);
    public BoolParameter _viewUncompressedEdges = new BoolParameter(false, true);
    public BoolParameter _viewQuantizedSobel = new BoolParameter(false, true);
    public BoolParameter _noEdges = new BoolParameter(false, true);
    public BoolParameter _noFill = new BoolParameter(false, true);

    public void load(Material material, ref RenderingData renderingData)
    {
        material.SetTexture("_AsciiTex", _asciiTex.value);
        material.SetFloat("_K", _stdevScale.value);
        material.SetFloat("_Sigma", _stdev.value);
        material.SetFloat("_Tau", _tau.value);
        material.SetInt("_GaussianKernelSize", _gaussianKernelSize.value);
        material.SetFloat("_Threshold", _threshold.value);
        material.SetInt("_Invert", _invert.value ? 1 : 0);
    }

    public bool IsActive() => true;
    public bool IsTileCompatible() => false;
}