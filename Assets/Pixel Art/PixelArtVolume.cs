using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[Serializable, VolumeComponentMenu("Joseph_Acerola/Custom Pixel Art")]
public class PixelArtVolume : VolumeComponent, IPostProcessComponent
{
    public ClampedIntParameter _downSample = new ClampedIntParameter(0, 0, 8, true);

    /*
    public void load(Material material, ref RenderingData renderingData)
    {

    }
    */

    public bool IsActive() => true;
    public bool IsTileCompatible() => false;
}