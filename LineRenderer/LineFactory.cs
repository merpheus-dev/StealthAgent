using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Subtegral.StealthAgent.GameCore
{
    public static class LineFactory
    {
        public static GameObject Draw(Vector2 a, Vector2 b, LineProfile profile)
        {
            var (lineManager, gameObj) = Init();
            lineManager.RenderLine(a, b, profile);
            return gameObj;
        }
        
        public static GameObject DrawChunk(Vector2[] verts,LineProfile profile)
        {
            var (lineManager, gameObj) = Init();
            lineManager.RenderVerticies(verts, profile);
            return gameObj;
        }

        private static (LineManager lineManager,GameObject gameObj) Init()
        {
            GameObject gameObject = new GameObject("Line");
            LineManager lineManager = gameObject.AddComponent<LineManager>();
            return (lineManager,gameObject);
        }
    }

}