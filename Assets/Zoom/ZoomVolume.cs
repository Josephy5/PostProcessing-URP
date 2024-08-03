using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[Serializable, VolumeComponentMenu("Joseph_Acerola/Custom Zoom")]
public class ZoomVolume : VolumeComponent, IPostProcessComponent
{
    public ZoomModeParameter _zoomMode = new ZoomModeParameter(ZoomMode.Point, true);
    public FloatParameter _zoom = new ClampedFloatParameter(1f, 0f, 2f, true);
    public Vector2Parameter _offset = new Vector2Parameter(Vector2.zero, true);
    public FloatParameter _rotation = new ClampedFloatParameter(0f, -180f, 180f, true);

    public void load(Material material, ref RenderingData renderingData)
    {
        material.SetFloat("_Zoom", _zoom.value);
        material.SetFloat("_Rotation", _rotation.value);
        material.SetInt("_ZoomMode", (int)_zoomMode.value);
        material.SetVector("_Offset", _offset.value);
    }

    public bool IsActive() => true;
    public bool IsTileCompatible() => false;
}
public enum ZoomMode
{
    Point = 0,
    PixelArtAntiAlias,
    Linear
}
[Serializable]
public sealed class ZoomModeParameter : VolumeParameter<ZoomMode>
{
    public ZoomModeParameter(ZoomMode value, bool overrideState = false) : base(value, overrideState) { }
}