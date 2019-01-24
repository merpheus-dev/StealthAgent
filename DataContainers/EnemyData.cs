using UnityEngine;
using Subtegral.StealthAgent.GameCore;
using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class EnemyData : DataContainer
{
    public List<Waypoint> Waypoints = new List<Waypoint>();

    //Movement Intervals
    public float MovementSpeed = .7f;
    public float SearchSpeed = .7f;
    public float ChaseSpeed = 1.5f;

    //Movement Coefficients
    public float EstimationThreshold = .1f;
    public float DefaultLookAtRatio = 1f;
    public float RandomPointRadius;

    //Time values
    public float ChaseTriggerInterval = 2f;
    public float EnemyTakeDownTime = 4f;

    public Vector2[] GetWayPoints()
    {
        return Waypoints.Select(x => x.Position).ToArray();

    }
}