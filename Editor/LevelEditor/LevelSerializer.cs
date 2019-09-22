using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Subtegral.StealthAgent.GameCore;
using Subtegral.StealthAgent.Interactions;
using UnityEngine.U2D;
using UnityEngine.Experimental.U2D;
using UnityEditor;
using SimpleJSON;
using Pathfinding;

public static class LevelSerializer
{
    public static LevelData Deserialize(string levelId)
    {
        LevelData data = Resources.Load<LevelData>(string.Format("Levels/{0}", levelId));
        return data;
    }

    public static LevelData Serialize(int levelId)
    {

        var data = ScriptableObject.CreateInstance<LevelData>();
        AssetDatabase.CreateAsset(data, string.Format("Assets/Resources/Levels/{0}.asset", levelId));
        AssetDatabase.SaveAssets();

        //Sprite Shape
        SpriteShapeSerialize(data);

        //Serialize Transform Data
        TransformSerialize(data);
        return data;
    }

    private static void SpriteShapeSerialize(LevelData data)
    {
        SpriteShapeController[] spriteShapes = Object.FindObjectsOfType<SpriteShapeController>();

        foreach (var spriteShape in spriteShapes)
        {
            //If door&stuff is the sprite shape, skip it. We already have prefab for it
            if (spriteShape.gameObject.GetComponent<IDataController>() != null)
                continue;
            SpriteShapeData spData = new SpriteShapeData
            {
                Pointer = spriteShape.spriteShape,
                Position = spriteShape.gameObject.transform.position,
                Rotation = spriteShape.gameObject.transform.rotation,
                SplineDetail = spriteShape.splineDetail,
                MaterialColor = spriteShape
                                .GetComponent<SpriteShapeRenderer>()
                                .sharedMaterial
                                .color
            };

            for (var i = 0; i < spriteShape.spline.GetPointCount(); i++)
            {
                spData.AddPoint(spriteShape.spline.GetPosition(i));
            }

            data.SpriteShapes.Add(spData);
        }
        EditorUtility.SetDirty(data);
    }

    public static void TransformSerialize(LevelData data)
    {
        foreach(var perObject in Object.FindObjectsOfType<Transform>())
        {
            perObject.gameObject.GetComponent<IDataController>()?.GetContainer().AppendControllerData(perObject.transform);
        }
    }

    public static void FinishZoneSerialize(LevelData data)
    {
        if (!GameObject.FindObjectOfType<FinishZone>())
            throw new System.Exception("Finish zone has not been planted!!!");
        FinishZone targetGameObject = GameObject.FindObjectOfType<FinishZone>();
        data.FinishZoneData.AppendControllerData(targetGameObject.gameObject.transform);
        data.FinishZoneData = (FinisZoneData)targetGameObject.GetContainer();
    }

    public static void GraphSerialize(LevelData data)
    {
        data.Graphs = AstarPath.active.data.SerializeGraphs();
    }

    public static void SerializeExisting(LevelData data, int levelId)
    {
        data.SpriteShapes.Clear();
        SpriteShapeSerialize(data);
        TransformSerialize(data);
        FinishZoneSerialize(data);
        GraphSerialize(data);
        string newPath = AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(data), levelId.ToString());
        AssetDatabase.ImportAsset(newPath);
    }
}


