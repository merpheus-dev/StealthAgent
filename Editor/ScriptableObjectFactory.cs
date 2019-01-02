using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Subtegral.StealthAgent.GameCore;
using UnityEditor;
using System.Reflection;
namespace Subtegral.StealthAgent.EditorSystem
{
    public class ScriptableObjectFactory
    {
        public static T CreateObject<T>(string assetName) where T : ScriptableObject
        {
            T item = ScriptableObject.CreateInstance<T>();
            //AssetDatabase.CreateAsset(item, string.Format("Assets/Resources/{0}.asset", assetName));
            //AssetDatabase.SaveAssets();
            //EditorUtility.FocusProjectWindow();
            //Selection.activeObject = item;
            return item;
        }

        [MenuItem("Subtegral/Create/KeyItem")]
        public static void CreateKeyItem()
        {
            CreateObject<KeyItem>("NewKeyHey");
        }

        [MenuItem("Subtegral/Create/Line Profile")]
        public static void CreateLineProfile()
        {
            CreateObject<LineProfile>("NewLineProfile");
        }

        [MenuItem("Subtegral/Create/Noise Item")]
        public static void CreateNoiseItem()
        {
            CreateObject<NoiseGeneratorItem>("NoiseItem");
        }

        [MenuItem("Subtegral/Create/Taser Item")]
        public static void CreateTaserItem()
        {
            CreateObject<TaserItem>("TaserItem");
        }
    }
}
