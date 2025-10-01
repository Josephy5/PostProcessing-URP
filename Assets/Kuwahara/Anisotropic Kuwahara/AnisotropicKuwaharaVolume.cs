using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[VolumeComponentMenu("Anisotropic Kuwahara")]
public class AnisotropicKuwaharaVolume : VolumeComponent, IPostProcessComponent
{
    public ClampedIntParameter _kernelSize = new ClampedIntParameter(2, 2, 20, true);
    public ClampedFloatParameter _sharpness = new ClampedFloatParameter(8.0f, 1.0f, 18.0f, true);
    public ClampedFloatParameter _hardness = new ClampedFloatParameter(8.0f, 1.0f, 100.0f, true);
    public ClampedFloatParameter _anisotropyStrength = new ClampedFloatParameter(2.0f, 0.01f, 2.0f, true);
    public ClampedFloatParameter _zeroCrossing = new ClampedFloatParameter(0.58f, 0.01f, 2.0f, true);
    public BoolParameter _enableFilterShapeFineTuning = new BoolParameter(false, true);
    public ClampedFloatParameter _FilterShapeFineTuning = new ClampedFloatParameter(1.0f, 0.01f, 3.0f, true);
    public ClampedIntParameter _passes = new ClampedIntParameter(1, 1, 4, true);

    public void load(Material material, ref RenderingData renderingData)
    {
        material.SetFloat("_KernelSize", _kernelSize.value);
        material.SetInt("_NumOfSectorsInKernel", 8);
        material.SetFloat("_Sharpness", _sharpness.value);
        material.SetFloat("_Hardness", _hardness.value);
        material.SetFloat("_AnisotropyStrength", _anisotropyStrength.value);
        material.SetFloat("_ZeroCrossing", _zeroCrossing.value);
        material.SetFloat("_Zeta", _enableFilterShapeFineTuning.value ? _FilterShapeFineTuning.value : 2.0f / 2.0f / (_kernelSize.value / 2.0f));
    }
    public bool IsActive() => true;
    public bool IsTileCompatible() => false;
}
