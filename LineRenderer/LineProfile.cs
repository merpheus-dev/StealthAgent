using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Subtegral.StealthAgent.GameCore
{
    public class LineProfile : ScriptableObject
    {
        public Color StartColor;
        public Color EndColor;
        public Material LineMaterial;
        public float Width;

        public void AppendLine(LineRenderer lineRenderer)
        {
            lineRenderer.material = LineMaterial;
            lineRenderer.startColor = StartColor;
            lineRenderer.endColor = EndColor;
            lineRenderer.startWidth = Width;
            lineRenderer.endWidth = Width;

            //Set line rendering
            lineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            lineRenderer.receiveShadows = false;
        }
    } 
}
