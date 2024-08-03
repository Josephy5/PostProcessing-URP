using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[Serializable, VolumeComponentMenu("Joseph_Acerola/Custom Half Tone")]
public class HalftoneVolume : VolumeComponent, IPostProcessComponent
{
    [Header("Cyan")]
    public BoolParameter _printCyan = new BoolParameter(true, true);
    public ClampedFloatParameter _cyanDotSize = new ClampedFloatParameter(1.0f,0f,3f,true);
    public ClampedFloatParameter _cyanBias = new ClampedFloatParameter(0.0f,-1f,1f,true);
    public Vector2Parameter _cyanOffset = new Vector2Parameter(Vector2.zero, true);

    [Header("Magenta")]
    public BoolParameter _printMagenta = new BoolParameter(true,true);
    public ClampedFloatParameter _magentaDotSize = new ClampedFloatParameter(1.0f, 0f, 3f, true);
    public ClampedFloatParameter _magentaBias = new ClampedFloatParameter(0.0f, -1f, 1f, true);
    public Vector2Parameter _magentaOffset = new Vector2Parameter(Vector2.zero, true);

    [Header("Yellow")]
    public BoolParameter _printYellow = new BoolParameter(true, true);
    public ClampedFloatParameter _yellowDotSize = new ClampedFloatParameter(1.0f,0f,3f, true);
    public ClampedFloatParameter _yellowBias = new ClampedFloatParameter(0.0f,-1f,1f,true);
    public Vector2Parameter _yellowOffset = new Vector2Parameter(Vector2.zero,true);

    [Header("Black")]
    public BoolParameter _printBlack = new BoolParameter(false,true);
    public ClampedFloatParameter _blackDotSize = new ClampedFloatParameter(1.0f,0f,3f, true);
    public ClampedFloatParameter _blackBias = new ClampedFloatParameter(0.0f,-1f,1f, true);
    public Vector2Parameter _blackOffset = new Vector2Parameter(Vector2.zero,true);

    public void load(Material material, ref RenderingData renderingData)
    {
        material.SetFloat("_CyanDotSize", _cyanDotSize.value);
        material.SetFloat("_MagentaDotSize", _magentaDotSize.value);
        material.SetFloat("_YellowDotSize", _yellowDotSize.value);
        material.SetFloat("_BlackDotSize", _blackDotSize.value);
        material.SetInt("_PrintCyan", _printCyan.value ? 1 : 0);
        material.SetFloat("_CyanBias", _cyanBias.value);
        material.SetVector("_CyanOffset", _cyanOffset.value);
        material.SetInt("_PrintMagenta", _printMagenta.value ? 1 : 0);
        material.SetFloat("_MagentaBias", _magentaBias.value);
        material.SetVector("_MagentaOffset", _magentaOffset.value);
        material.SetInt("_PrintYellow", _printYellow.value ? 1 : 0);
        material.SetFloat("_YellowBias", _yellowBias.value);
        material.SetVector("_YellowOffset", _yellowOffset.value);
        material.SetInt("_PrintBlack", _printBlack.value ? 1 : 0);
        material.SetFloat("_BlackBias", _blackBias.value);
        material.SetVector("_BlackOffset", _blackOffset.value);
    }

    public bool IsActive() => true;
    public bool IsTileCompatible() => false;
}