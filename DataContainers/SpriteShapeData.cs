using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.Experimental.U2D;
using System;
using System.Collections.Generic;

[Serializable]
public class SpriteShapeData
{
    public SpriteShape Pointer;
    public Spline Spline;
    public int SplineDetail;
    public Color MaterialColor;
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
        for (int i = 0; i < _vertices.Count; i++)
        {
            spline.InsertPointAt(i, _vertices[i]);
        }
    }
}