using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using Cinemachine;
using UnityEngine.U2D;
using UnityEngine.SceneManagement;
namespace Subtegral.StealthAgent.GameCore
{
    public class LevelManager : MonoBehaviour
    {
        public int ActiveLevel = 0;

        private AstarPath pathFinder;
        private CinemachineVirtualCamera _camera;

        private void Start()
        {
            pathFinder = FindObjectOfType<AstarPath>();
            _camera = FindObjectOfType<CinemachineVirtualCamera>();
            //TO-DO REFACTOR AFTER GGJ:
            //if (PlayerPrefs.HasKey("level"))
            //{
            //    ActiveLevel = PlayerPrefs.GetInt("level");
            //}
            LevelData data = Resources.Load<LevelData>(string.Format("Levels/{0}", ActiveLevel.ToString()));
            List<GameObject> instances = LevelGenerator.GenerateLevel(data, true);
         //   camera.Follow = instances.Find((x) => x.GetComponent<Player>()).transform;
            instances.FindAll(x => x.GetComponent<SpriteShapeController>())
                    .ForEach((x) => x.GetComponent<SpriteShapeController>().BakeCollider());
        }
        //private void Start()
        //{

        //    pathFinder.Scan();

        //}

        public void LoadNext()
        {
            PlayerPrefs.SetInt("level",++ActiveLevel);
            PlayerPrefs.Save();
            Reload();
        }

        public void Reload()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

}