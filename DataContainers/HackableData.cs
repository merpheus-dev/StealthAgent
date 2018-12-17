using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Subtegral.StealthAgent.GameCore;
using System;

[Serializable]
public class HackableData :DataContainer
{
    [Range(0f, 1f)]
    public float HackBlinkDamping = .5f;

    public float HackTime = 5f;

    public Color HackSuccessColor;
    public Color HackInProgressColor;

}
