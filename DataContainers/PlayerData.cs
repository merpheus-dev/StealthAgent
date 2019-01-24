using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Subtegral.StealthAgent.GameCore
{
    public class PlayerData :DataContainer
    {
        public float MovementSpeed = 1.5f;
        public float EstimationThreshold = 0.1f;
        public float LookAtRatio = 5f;
        public SkinData Skin;
    }
}
