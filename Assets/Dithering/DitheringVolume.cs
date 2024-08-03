using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[VolumeComponentMenu("Joseph_Acerola/Custom Dithering")]
public class DitheringVolume : VolumeComponent, IPostProcessComponent
{
    public ClampedFloatParameter _spread = new ClampedFloatParameter(0.5f, 0.0f, 1.0f, true);

    public ClampedIntParameter _redColorCount = new ClampedIntParameter(2, 2, 16, true);
    public ClampedIntParameter _greenColorCount = new ClampedIntParameter(2, 2, 16, true);
    public ClampedIntParameter _blueColorCount = new ClampedIntParameter(2, 2, 16, true);

    public ClampedIntParameter _bayerLevel = new ClampedIntParameter(0, 0, 2, true);
    public ClampedIntParameter _downSamples = new ClampedIntParameter(0, 0, 8, true);

    public void load(Material material, ref RenderingData renderingData)
    {
        material.SetFloat("_Spread", _spread.value);
        material.SetInt("_RedColorCount", _redColorCount.value);
        material.SetInt("_GreenColorCount", _greenColorCount.value);
        material.SetInt("_BlueColorCount", _blueColorCount.value);
        material.SetInt("_BayerLevel", _bayerLevel.value);
    }
    public bool IsActive() => true;
    public bool IsTileCompatible() => false;
}
