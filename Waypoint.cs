using System;
using System.Collections.Generic;
using UnityEngine;

namespace Subtegral.StealthAgent.GameCore
{
    [Serializable]
    public class Waypoint
    {
        public List<WaypointAngle> Angles = new List<WaypointAngle>();
        public Vector2 Position;
    } 

    [Serializable]
    public class WaypointAngle
    {
        public float Angle
        {
            get
            {
                return AngleVector.z-90;
            }
            set
            {
                Debug.Log("VALUE SET");
                AngleVector.z = value+90;
            }
        }
        public float TimeToWait;
        public float LookAtRatio=.1f;
        public Vector3 AngleVector;
    }
}
