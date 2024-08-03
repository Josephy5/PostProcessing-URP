using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[VolumeComponentMenu("Joseph_Acerola/Custom Chromatic Aboration")]
public class ChromaticAboVolume : VolumeComponent, IPostProcessComponent
{
    public Vector2Parameter _focalOffset = new Vector2Parameter(new Vector2(0.0f, 0.0f),true);
    public Vector2Parameter _radius = new Vector2Parameter(new Vector2(1.0f, 1.0f), true);

    public ClampedFloatParameter _hardness = new ClampedFloatParameter(1f, 0.01f, 5.0f, true);
    public ClampedFloatParameter _intensity = new ClampedFloatParameter(1f, 0.01f, 5.0f, true);

    public Vector3Parameter _channelOffsets = new Vector3Parameter(new Vector3(0.0f, 0.0f, 0.0f), true);

    public void load(Material material, ref RenderingData renderingData)
    {
        material.SetVector("_FocalOffset", _focalOffset.value);
        material.SetVector("_Radius", _radius.value);
        material.SetVector("_ColorOffsets", _channelOffsets.value);
        material.SetFloat("_Hardness", _hardness.value);
        material.SetFloat("_Intensity", _intensity.value);
    }
    public bool IsActive() => true;
    public bool IsTileCompatible() => false;
}
