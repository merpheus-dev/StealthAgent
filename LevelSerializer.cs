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

public static class LevelSerializer
{
    public static LevelData Deserialize(string levelName)
    {
        LevelData data = Resources.Load<LevelData>(string.Format("Levels/{0}", levelName));
        return data;
    }

    public static void Serialize(string levelName)
    {

        var data = ScriptableObject.CreateInstance<LevelData>();
        AssetDatabase.CreateAsset(data, string.Format("Assets/Resources/Levels/{0}.asset", levelName));
        AssetDatabase.SaveAssets();

        //Sprite Shape
        SpriteShapeSerialize(data);
    }

    private static void SpriteShapeSerialize(LevelData data)
    {
        SpriteShapeController[] spriteShapes = Object.FindObjectsOfType<SpriteShapeController>();
        foreach (var spriteShape in spriteShapes)
        {
            data.SpriteShapes.Add(new SpriteShapeData
            {
                Pointer = spriteShape.spriteShape,
                Spline = spriteShape.spline,
                SplineDetail = spriteShape.splineDetail,
                MaterialColor = spriteShape
                                .GetComponent<SpriteShapeRenderer>()
                                .materials[0]
                                .color
            });
        }
    }

    public static void SerializeExisting(LevelData data,string rename)
    {
        data.SpriteShapes.Clear();
        SpriteShapeSerialize(data);
        AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(data), rename);
    }
}


