using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[Serializable, VolumeComponentMenu("Joseph_Acerola/Custom Extended Difference of Gaussian")]
public class ExtendedDoGVolume : VolumeComponent, IPostProcessComponent
{
    public ClampedIntParameter _superSample = new ClampedIntParameter(1, 1, 4, true);

    [Header("Edge Tangent Flow Settings")]
    public BoolParameter _useFlow = new BoolParameter(true, true);
    public ClampedFloatParameter _structureTensorDeviation = new ClampedFloatParameter(2.0f,0f,5f, true);
    public ClampedFloatParameter _lineIntegralDeviation = new ClampedFloatParameter(2.0f,0f,20f, true);
    public Vector2Parameter _lineConvolutionStepSizes = new Vector2Parameter(Vector2.one, true);
    public BoolParameter _calcDiffBeforeConvolution = new BoolParameter(true, true);

    [Header("Difference Of Gaussians Settings")]
    public ClampedFloatParameter _differenceOfGaussiansDeviation = new ClampedFloatParameter(2.0f,0f,10f, true);
    public ClampedFloatParameter _stdevScale = new ClampedFloatParameter(1.6f,0.1f,5f, true);
    public ClampedFloatParameter _Sharpness = new ClampedFloatParameter(1.0f,0,100f, true);

    [Header("Threshold Settings")]
    public ThresholdModeParameter _thresholdMode = new ThresholdModeParameter(ThresholdMode.NoThreshold, true);
    public ClampedIntParameter _quantizerStep = new ClampedIntParameter(2, 1, 16, true);
    public ClampedFloatParameter _whitePoint = new ClampedFloatParameter(50.0f,0f,100f, true);
    public ClampedFloatParameter _softThreshold = new ClampedFloatParameter(1.0f, 0f, 10f, true);
    public BoolParameter _invert = new BoolParameter(false, true);
    
    [Header("Anti Aliasing Settings")]
    public BoolParameter _smoothEdges = new BoolParameter(true, true);
    public ClampedFloatParameter _edgeSmoothDeviation = new ClampedFloatParameter(1.0f, 0f, 10f, true);
    public Vector2Parameter _edgeSmoothStepSizes = new Vector2Parameter(Vector2.one, true);

    [Header("Cross Hatch Settings")]
    public BoolParameter _enableHatching = new BoolParameter(true, true);
    public TextureParameter _hatchTexture = new TextureParameter(null, true);

    [Space(10)]
    public ClampedFloatParameter _hatchResolution = new ClampedFloatParameter(1f, 0f, 8f, true);
    public ClampedFloatParameter _hatchRotation = new ClampedFloatParameter(90.0f, -180f, 180f, true);

    [Space(10)]
    public BoolParameter _enableSecondLayer = new BoolParameter(false, true);
    public ClampedFloatParameter _secondWhitePoint = new ClampedFloatParameter(20.0f,0f,100f, true);
    public ClampedFloatParameter _hatchResolution2 = new ClampedFloatParameter(1.0f,0f,8f, true);
    public ClampedFloatParameter _secondHatchRotation = new ClampedFloatParameter(60.0f, -180f, 180f, true);

    [Space(10)]
    public BoolParameter _enableThirdLayer = new BoolParameter(false, true);
    public ClampedFloatParameter _thirdWhitePoint = new ClampedFloatParameter(30.0f,0f,100f, true);
    public ClampedFloatParameter _hatchResolution3 = new ClampedFloatParameter(1.0f,0f,8f, true);
    public ClampedFloatParameter _thirdHatchRotation = new ClampedFloatParameter(120.0f,-180f, 180f, true);

    [Space(10)]
    public BoolParameter _enableFourthLayer = new BoolParameter(false, true);
    public ClampedFloatParameter _fourthWhitePoint = new ClampedFloatParameter(30.0f,0f,100f, true);
    public ClampedFloatParameter _hatchResolution4 = new ClampedFloatParameter(1.0f,0f,8f, true);
    public ClampedFloatParameter _fourthHatchRotation = new ClampedFloatParameter(120.0f,-180f,180f, true);

    [Space(10)]
    public BoolParameter _enableColoredPencil = new BoolParameter(false, true);
    public ClampedFloatParameter _brightnessOffset = new ClampedFloatParameter(0.0f,-1f,1f, true);
    public ClampedFloatParameter _saturation = new ClampedFloatParameter(1.0f,0f,5f, true);

    [Header("Blend Settings")]
    public ClampedFloatParameter _termStrength = new ClampedFloatParameter(1.0f, 0f, 5f, true);
    public BlendModeDoGParameter _blendMode = new BlendModeDoGParameter(BlendMode_DoG.NoBlend, true);
    public ColorParameter _minColor = new ColorParameter(Color.black, true);
    public ColorParameter _maxColor = new ColorParameter(Color.white, true);
    public ClampedFloatParameter _blendStrength = new ClampedFloatParameter(1f,0f,2f, true);

    public void load(Material material, ref RenderingData renderingData)
    {
        material.SetFloat("_SigmaC", _structureTensorDeviation.value);
        material.SetFloat("_SigmaE", _differenceOfGaussiansDeviation.value);
        material.SetFloat("_SigmaM", _lineIntegralDeviation.value);
        material.SetFloat("_SigmaA", _edgeSmoothDeviation.value);
        material.SetFloat("_K", _stdevScale.value);
        material.SetFloat("_Tau", _Sharpness.value);
        material.SetFloat("_Phi", _softThreshold.value);
        material.SetFloat("_Threshold", _whitePoint.value);
        material.SetFloat("_Threshold2", _secondWhitePoint.value);
        material.SetFloat("_Threshold3", _thirdWhitePoint.value);
        material.SetFloat("_Threshold4", _fourthWhitePoint.value);
        material.SetFloat("_Thresholds", _quantizerStep.value);
        material.SetFloat("_BlendStrength", _blendStrength.value);
        material.SetFloat("_DoGStrength", _termStrength.value);
        material.SetFloat("_HatchTexRotation", _hatchRotation.value);
        material.SetFloat("_HatchTexRotation1", _secondHatchRotation.value);
        material.SetFloat("_HatchTexRotation2", _thirdHatchRotation.value);
        material.SetFloat("_HatchTexRotation3", _fourthHatchRotation.value);
        material.SetFloat("_HatchRes1", _hatchResolution.value);
        material.SetFloat("_HatchRes2", _hatchResolution2.value);
        material.SetFloat("_HatchRes3", _hatchResolution3.value);
        material.SetFloat("_HatchRes4", _hatchResolution4.value);
        material.SetInt("_EnableSecondLayer", _enableSecondLayer.value ? 1 : 0);
        material.SetInt("_EnableThirdLayer", _enableThirdLayer.value ? 1 : 0);
        material.SetInt("_EnableFourthLayer", _enableFourthLayer.value ? 1 : 0);
        material.SetInt("_EnableColoredPencil", _enableColoredPencil.value ? 1 : 0);
        material.SetFloat("_BrightnessOffset", _brightnessOffset.value);
        material.SetFloat("_Saturation", _saturation.value);
        material.SetVector("_IntegralConvolutionStepSizes", new Vector4(_lineConvolutionStepSizes.value.x, _lineConvolutionStepSizes.value.y, _edgeSmoothStepSizes.value.x, _edgeSmoothStepSizes.value.y));
        material.SetVector("_MinColor", _minColor.value);
        material.SetVector("_MaxColor", _maxColor.value);
        material.SetInt("_Thresholding", (int)_thresholdMode.value);
        material.SetInt("_BlendMode", (int)_blendMode.value);
        material.SetInt("_Invert", _invert.value ? 1 : 0);
        material.SetInt("_CalcDiffBeforeConvolution", _calcDiffBeforeConvolution.value ? 1 : 0);
        material.SetInt("_HatchingEnabled", _enableHatching.value ? 1 : 0);
        material.SetTexture("_HatchTex", _hatchTexture.value);
    }

    public bool IsActive() => true;
    public bool IsTileCompatible() => false;
}
public enum ThresholdMode
{
    NoThreshold = 0,
    Tanh,
    Quantization,
    SmoothQuantization
}
[Serializable]
public sealed class ThresholdModeParameter : VolumeParameter<ThresholdMode>
{
    public ThresholdModeParameter(ThresholdMode value, bool overrideState = false) : base(value, overrideState) { }
}
public enum BlendMode_DoG
{
    NoBlend = 0,
    Interpolate,
    TwoPointInterpolate
}
[Serializable]
public sealed class BlendModeDoGParameter : VolumeParameter<BlendMode_DoG>
{
    public BlendModeDoGParameter(BlendMode_DoG value, bool overrideState = false) : base(value, overrideState) { }
}