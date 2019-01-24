using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.Experimental.U2D;
using System;
using System.Collections.Generic;
using Subtegral.StealthAgent.GameCore;

[Serializable]
public class SpriteShapeData
{
    public Vector2 Position;
    public Quaternion Rotation;
    public SpriteShape Pointer;
    public int SplineDetail;
    public Color MaterialColor;
    [SerializeField]
    private List<Vector3> _vertices = new List<Vector3>();

    public void AddPoint(Vector3 vert)
    {
        _vertices.Add(vert);
    }

    public void RemovePoint(Vector3 vert)
    {
        if (_vertices.Contains(vert))
            _vertices.Remove(vert);
    }

    public void ShapeSpline(Spline spline)
    {
        spline.Clear();
        for (int i = 0; i < _vertices.Count; i++)
        {
            spline.InsertPointAt(i, _vertices[i]);
        }
    }
}