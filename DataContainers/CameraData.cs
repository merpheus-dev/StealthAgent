using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Subtegral.StealthAgent.GameCore;
using System;

[Serializable]
public class CameraData : DataContainer
{
    public WaypointAngle[] Angles;
    public float LookAtRatio;
}
