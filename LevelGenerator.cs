using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Subtegral.StealthAgent.Interactions;
namespace Subtegral.StealthAgent.GameCore
{
    public class LevelGenerator : MonoBehaviour
    {
        public int ActiveLevel;
        private void Awake()
        {
            LevelData data = Resources.Load<LevelData>(string.Format("Levels/{0}",ActiveLevel));
            GenerateLevel(data.DataContainers);
        }

        public void GenerateLevel(List<DataContainer> containers)
        {
            foreach (var item in containers)
            {
                // ControllerFactory.CreateFactory<DataContainer>(item.GetType());
                //ControllerFactory.CreateFactory(item.GetController(), item);
                switch (item)
                {
                    case DoorData d:
                        ControllerFactory.CreateFactory<Door>(item);
                        break;
                    case TaserData t:
                        ControllerFactory.CreateFactory<Taser>(item);
                        break;
                    case HackableData h:
                        ControllerFactory.CreateFactory<HackableObject>(item);
                        break;
                    case CameraData c:
                        ControllerFactory.CreateFactory<CameraEnemy>(item);
                        break;
                    case EnemyData e:
                        ControllerFactory.CreateFactory<Enemy>(item);
                        break;
                    case HostileData h:
                        ControllerFactory.CreateFactory<Hostile>(item);
                        break;
                    case KeyData k:
                        ControllerFactory.CreateFactory<Key>(item);
                        break;
                    case LasersData l:
                        ControllerFactory.CreateFactory<Laser>(item);
                        break;

                }
            }
        }

    }








}