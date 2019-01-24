using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Subtegral.StealthAgent.GameCore;
using Subtegral.StealthAgent.Interactions;
using UnityEditor;

public class LevelData : ScriptableObject
{
    public int Id;
    public string Name="";
    public List<SpriteShapeData> SpriteShapes = new List<SpriteShapeData>();
    public List<DataContainer> DataContainers = new List<DataContainer>();
    public PlayerData PlayerData = null;
}

[CustomEditor(typeof(LevelData))]
public class LevelDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
    }
}