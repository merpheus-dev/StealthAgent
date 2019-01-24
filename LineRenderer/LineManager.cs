using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Subtegral.StealthAgent.GameCore
{
    public class LineManager : MonoBehaviour
    {
        private LineRenderer _renderer;

        public static float LineInfinity = 10000f;
        private void Awake()
        {
            _renderer = gameObject.AddComponent<LineRenderer>();
        }

        private void RenderStartPoint(Vector2 a,LineProfile profile)
        {
            if (profile != null)
                profile.AppendLine(_renderer);

            _renderer.SetPosition(0, a);
        }

        public void RenderLine(Vector2 a,Vector2 b,LineProfile profile=null)
        {
            RenderStartPoint(a, profile);

            _renderer.SetPosition(1, b);
        }

        public void RenderLine(Vector2 a, RaycastHit2D hitPoint,Vector2 axis,LineProfile profile = null)
        {
            RenderStartPoint(a, profile);

            if (hitPoint.collider != null)
                _renderer.SetPosition(1, hitPoint.point);
            else
                _renderer.SetPosition(1, axis * LineInfinity);
        }

        public void RenderVerticies(Vector2[] verts, LineProfile profile = null)
        {
            Vector3[] vector3s= new Vector3[verts.Length];
            for (int i = 0; i < verts.Length; i++)
            {
                vector3s[i] = verts[i];
            }
            _renderer.SetPositions(vector3s);
            profile.AppendLine(_renderer);
        }

    }
}