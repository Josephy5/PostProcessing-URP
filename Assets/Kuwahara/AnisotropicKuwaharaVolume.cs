using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[VolumeComponentMenu("Joseph_Acerola/Custom Anisotropic Kuwahara")]
public class AnisotropicKuwaharaVolume : VolumeComponent, IPostProcessComponent
{
    public ClampedIntParameter _kernelSize = new ClampedIntParameter(2, 2, 20, true);
    public ClampedFloatParameter _sharpness = new ClampedFloatParameter(8.0f, 1.0f, 18.0f, true);
    public ClampedFloatParameter _hardness = new ClampedFloatParameter(8.0f, 1.0f, 100.0f, true);
    public ClampedFloatParameter _alpha = new ClampedFloatParameter(1.0f, 0.01f, 2.0f, true);
    public ClampedFloatParameter _zeroCrossing = new ClampedFloatParameter(0.58f, 0.01f, 2.0f, true);
    public BoolParameter _useZeta = new BoolParameter(false, true);
    public ClampedFloatParameter _zeta = new ClampedFloatParameter(1.0f, 0.01f, 3.0f, true);
    public ClampedIntParameter _passes = new ClampedIntParameter(1, 1, 4, true);

    public void load(Material material, ref RenderingData renderingData)
    {
        material.SetFloat("_KernelSize", _kernelSize.value);
        material.SetInt("_N", 8);
        material.SetFloat("_Q", _sharpness.value);
        material.SetFloat("_Hardness", _hardness.value);
        material.SetFloat("_Alpha", _alpha.value);
        material.SetFloat("_ZeroCrossing", _zeroCrossing.value);
        material.SetFloat("_Zeta", _useZeta.value ? _zeta.value : 2.0f / 2.0f / (_kernelSize.value / 2.0f));
    }
    public bool IsActive() => true;
    public bool IsTileCompatible() => false;
}
