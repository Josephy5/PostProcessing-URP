using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[Serializable, VolumeComponentMenu("Joseph_Acerola/Custom Blend Modes")]
public class BlendModeVolume : VolumeComponent, IPostProcessComponent
{
    public BlendModeParameter _blendMode = new BlendModeParameter(BlendMode.NoBlend, true);
    public BlendTypeParameter _blendType = new BlendTypeParameter(BlendType.BlendOnItself, true);
    public ClampedFloatParameter _strength = new ClampedFloatParameter(1.0f, 0.0f, 1.0f, true);
    public TextureParameter _blendTexture = new TextureParameter(null, true);
    public ColorParameter _blendColor = new ColorParameter(Color.white, true);

    public void load(Material material, ref RenderingData renderingData)
    {
        //material.SetInt("_BlendMode", (int) _blendMode.value);
        material.SetInt("_BlendType", (int) _blendType.value);
        material.SetTexture("_BlendTex", _blendTexture.value);
        material.SetColor("_BlendColor", _blendColor.value);
        material.SetFloat("_Strength", _strength.value);
    }

    public bool IsActive() => true;
    public bool IsTileCompatible() => false;
}
public enum BlendMode
{
    NoBlend = 0,
    Add,
    Subtract,
    Multiply,
    Screen,
    Overlay,
    SoftLight,
    ColorDodge,
    ColorBurn,
    VividLight
}

[Serializable]
public sealed class BlendModeParameter : VolumeParameter<BlendMode>
{
    public BlendModeParameter(BlendMode value, bool overrideState = false) : base(value, overrideState) { }
}
public enum BlendType
{
    BlendOnItself,
    PickedTexture,
    PickedColor
}
[Serializable]
public sealed class BlendTypeParameter : VolumeParameter<BlendType>
{
    public BlendTypeParameter(BlendType value, bool overrideState = false) : base(value, overrideState) { }
}