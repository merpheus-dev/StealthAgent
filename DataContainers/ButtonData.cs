using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Subtegral.StealthAgent.GameCore;

public class ButtonData : DataContainer
{
    public List<IEnable> Relationships;
    public bool IsPressed = false;
}
