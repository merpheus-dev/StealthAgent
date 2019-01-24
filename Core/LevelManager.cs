using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using Cinemachine;
using UnityEngine.U2D;
namespace Subtegral.StealthAgent.GameCore
{
    public class LevelManager : MonoBehaviour
    {
        public int ActiveLevel = 0;

        private AstarPath pathFinder;
        private CinemachineVirtualCamera camera;

        private void Awake()
        {
            pathFinder = FindObjectOfType<AstarPath>();
            camera = FindObjectOfType<CinemachineVirtualCamera>();
        }
        private void Start()
        {
            LevelData data = Resources.Load<LevelData>(string.Format("Levels/{0}", ActiveLevel.ToString()));
            List<GameObject> instances = LevelGenerator.GenerateLevel(data, true);
            camera.Follow = instances.Find((x) => x.GetComponent<Player>()).transform;
            instances.FindAll(x => x.GetComponent<SpriteShapeController>())
                    .ForEach((x) => x.GetComponent<SpriteShapeController>().BakeCollider());
            pathFinder.Scan();

        }
    }

}