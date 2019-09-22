using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Subtegral.StealthAgent.Interactions;
using Subtegral.StealthAgent.GameCore;

[CreateAssetMenu]
public class DoorData : DataContainer
{
    public DoorType DoorType;
    public float TargetAngle;
    public Vector3 AnchorPoint;
    public float Interval;
}