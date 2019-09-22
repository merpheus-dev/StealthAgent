using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Subtegral.StealthAgent.Interactions;
using UnityEngine.Experimental.U2D;
using UnityEngine.U2D;
namespace Subtegral.StealthAgent.GameCore
{
    public static class LevelGenerator
    {
        public static List<GameObject> GenerateLevel(LevelData data, bool runtime = false)
        {
            if (data.Graphs != null)
               AstarPath.active.data.DeserializeGraphsAdditive(data.Graphs);
            AstarPath.active.Scan();

            List<GameObject> instances = new List<GameObject>();
            List<DataContainer> containers = data.DataContainers;
            foreach (var item in containers)
            {
                switch (item)
                {
                    case DoorData d:
                        instances.Add(ControllerFactory.CreateFactory<Door>(item).gameObj);
                        break;
                    case TaserData t:
                        instances.Add(ControllerFactory.CreateFactory<Taser>(item).gameObj);
                        break;
                    case HackableData h:
                        instances.Add(ControllerFactory.CreateFactory<HackableObject>(item).gameObj);
                        break;
                    case CameraData c:
                        instances.Add(ControllerFactory.CreateFactory<CameraEnemy>(item).gameObj);
                        break;
                    case EnemyData e:
                        instances.Add(ControllerFactory.CreateFactory<Enemy>(item).gameObj);
                        break;
                    case HostileData h:
                        instances.Add(ControllerFactory.CreateFactory<Hostile>(item).gameObj);
                        break;
                    case KeyData k:
                        instances.Add(ControllerFactory.CreateFactory<Key>(item).gameObj);
                        break;
                    case LasersData l:
                        instances.Add(ControllerFactory.CreateFactory<Laser>(item).gameObj);
                        break;
                }

            }

            GameObject imitationObject = Resources.Load<GameObject>("Editor/ShapeBase");
            foreach (var shape in data.SpriteShapes)
            {
                imitationObject = MonoBehaviour.Instantiate(imitationObject, shape.Position, shape.Rotation);
                imitationObject.transform.localScale = Camera.main.gameObject.transform.lossyScale;
                SpriteShapeRenderer ssr = imitationObject.GetComponent<SpriteShapeRenderer>();
                SpriteShapeController c2 = imitationObject.GetComponent<SpriteShapeController>();
                c2.spriteShape = shape.Pointer;
                c2.splineDetail = shape.SplineDetail;
                ssr.material.color = shape.MaterialColor;
                shape.ShapeSpline(c2.spline);
                instances.Add(imitationObject);
            }

            GameObject playerObject = Resources.Load<GameObject>("Editor/Player");
            playerObject = MonoBehaviour.Instantiate(playerObject, data.PlayerData.GetTransform().position, data.PlayerData.GetTransform().rotation);
            Player _player = playerObject.GetComponent<Player>();
            _player.Inject(data.PlayerData);
            instances.Add(playerObject);

            GameObject finishZone = Resources.Load<GameObject>("Editor/FinishZone");
            finishZone = MonoBehaviour.Instantiate(finishZone, data.FinishZoneData.GetTransform().position, data.FinishZoneData.GetTransform().rotation);
            finishZone.GetComponent<FinishZone>().Inject(data.FinishZoneData);
            instances.Add(finishZone);

            //if(runtime)
            //    AstarPath.active.Scan();

            /*
            if (runtime)
                for (int i = 0; i < 3; i++)
                {
                    UserSaveManager.GetSkillElement(i).Skill.AppendPlayer(_player);
                }
                */
            return instances;
        }

    }








}