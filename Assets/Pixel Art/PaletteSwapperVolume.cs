using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[Serializable, VolumeComponentMenu("Joseph_Acerola/Custom Palette Swap")]
public class PaletteSwapperVolume : VolumeComponent, IPostProcessComponent
{
    public TextureParameter _colorPalette = new TextureParameter(null, true);
    public BoolParameter _invert = new BoolParameter(true, true);

    public void load(Material material, ref RenderingData renderingData)
    {
        material.SetTexture("_ColorPalette", _colorPalette.value);
        material.SetInt("_Invert", _invert.value ? 1 : 0);
    }

    public bool IsActive() => true;
    public bool IsTileCompatible() => false;
}