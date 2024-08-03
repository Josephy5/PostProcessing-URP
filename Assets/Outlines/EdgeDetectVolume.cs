using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[Serializable, VolumeComponentMenu("Joseph_Acerola/Custom Edge Detect")]
public class EdgeDetectVolume : VolumeComponent, IPostProcessComponent
{
    public ColorParameter _borderColor = new ColorParameter(Color.black, true, true, true, true);

    public void load(Material material, ref RenderingData renderingData)
    {
        material.SetColor("_BorderColor", _borderColor.value);
    }

    public bool IsActive() => true;
    public bool IsTileCompatible() => false;
}