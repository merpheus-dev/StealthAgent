using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Subtegral.StealthAgent.GameCore;
using Subtegral.StealthAgent.Interactions;
public class LevelData : ScriptableObject
{
    public int Id;
    public string Name="";
    public List<SpriteShapeData> SpriteShapes = new List<SpriteShapeData>();
    public List<DataContainer> DataContainers = new List<DataContainer>();
    public PlayerData PlayerData = null;
    public FinisZoneData FinishZoneData = null;
    public byte[] Graphs;
}
