using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Pathfinding;
using Subtegral.StealthAgent.GameCore;
using System;
namespace Subtegral.StealthAgent.Skills
{
    public class CameraZoom : Skill, ISkill
    {
        //put them in ctor
        public float ZoomOutLevel;
        public float EstimationThreshold = .1f;
        private float defaultCameraLevel;
        public Vector2 CenterOfLevel;

        private CinemachineVirtualCamera virtualCamera;
        private AstarPath ai;
        private Transform cachedCenterObject;
        private Transform CachedCenter
        {
            get
            {
                if (cachedCenterObject == null)
                {
                    cachedCenterObject = new GameObject("CENTER").transform;
                    cachedCenterObject.position = (ai.graphs[0] as GridGraph).center;
                }
                return cachedCenterObject;
            }
        }

        public void Activate()
        {
            // virtualCamera.m_Follow = CachedCenter;
            StopAllCoroutines();
            StartCoroutine(ZoomOut());
        }

        public override void Deactivate()
        {
            StartCoroutine(ZoomBackIn());
        }

        public override void OnAwake()
        {
            virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
            ai = FindObjectOfType<AstarPath>();
            defaultCameraLevel = virtualCamera.m_Lens.OrthographicSize;
        }

        private IEnumerator ZoomBackIn()
        {
            while (virtualCamera.m_Lens.OrthographicSize > defaultCameraLevel+EstimationThreshold)
            {
                yield return null;
                virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(virtualCamera.m_Lens.OrthographicSize, defaultCameraLevel, EstimationThreshold);
            }
          //  virtualCamera.m_Follow = FindObjectOfType<Player>().transform;
        }

        private IEnumerator ZoomOut()
        {
            while (virtualCamera.m_Lens.OrthographicSize <= ZoomOutLevel-EstimationThreshold)
            {
                yield return null;
                virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(virtualCamera.m_Lens.OrthographicSize, ZoomOutLevel, EstimationThreshold);
            }
            IsActive = true;
        }

    }

}