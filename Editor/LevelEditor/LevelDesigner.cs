using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using Subtegral.StealthAgent.Interactions;
using Subtegral.StealthAgent.GameCore;
using Pathfinding;
using UnityEditor.IMGUI.Controls;
namespace Subtegral.StealthAgent.EditorSystem
{
    public class LevelDesigner : EditorWindow
    {
        static bool displayNewLevel = true;
        static bool levelWasNull = true;
        List<GameObject> CreatedObjects = new List<GameObject>();
        static Transform doorAnchored = null;
        AstarPath Path
        {
            get
            {
                return FindObjectOfType<AstarPath>();
            }
        }

        static bool editEnemyRadius;

        Vector3 angle = Vector3.zero;
        Vector3 radius = Vector3.one * 2f;

        int levelId = -1;
        readonly string prefabPath = "Prefabs/";


        public Vector3 facingDirection
        {
            get
            {
                Vector3 result = Vector3.forward;
                result.y = 0f;
                return result.sqrMagnitude == 0f ? Vector3.forward : result.normalized;
            }
        }

        LevelData _levelData;

        [MenuItem("Subtegral/Level Editor")]
        public static void OpenEditor()
        {
            LevelDesigner designer = GetWindow<LevelDesigner>("Level Editor");
        }

        void OnEnable()
        {
            SceneView.onSceneGUIDelegate += this.OnSceneGUI;
        }

        void OnDisable()
        {
            SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
        }

        void OnSceneGUI(SceneView sceneView)
        {

            if (EditorApplication.isPlaying || _levelData == null)
                return;
            Handles.BeginGUI();
            GUILayout.BeginArea(new Rect(5, 5, 100, 300), EditorStyles.helpBox);
            if (GUILayout.Button("New Block"))
            {
                GameObject shape = Resources.Load<GameObject>("Editor/ShapeBase");
                shape = Instantiate(shape);
                CreatedObjects.Add(shape);
               // shape.transform.SetParent(obstacleRoot.transform, true);
                shape.name = "Obstacle";
                Selection.activeGameObject = shape;
                Tools.current = Tool.Move;
            }

            if (GUILayout.Button("FinishZone"))
            {
                var finishZoneData = ScriptableObject.CreateInstance<FinisZoneData>();
                finishZoneData.name = "FinishZone";
                AssetDatabase.AddObjectToAsset(finishZoneData, _levelData);
                AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(_levelData));
                _levelData.FinishZoneData = finishZoneData;
                EditorUtility.SetDirty(_levelData);
                GameObject shape = Resources.Load<GameObject>("Editor/FinishZone");
                shape = Instantiate(shape);
                shape.GetComponent<FinishZone>().Inject(finishZoneData);
                CreatedObjects.Add(shape);
                Selection.activeGameObject = shape;
                Tools.current = Tool.Move;
            }

            DisplayPlayerButton(_levelData.PlayerData != null);

            GUILayout.Label("Prefabs", EditorStyles.boldLabel);
            DisplayEditorPrefab<CameraData>("CameraEnemy", prefabPath);
            DisplayEditorPrefab<DoorData>("Door", prefabPath, (Transform g) => { doorAnchored = g; });
            DisplayEditorPrefab<EnemyData>("Enemy", prefabPath);
            DisplayEditorPrefab<LasersData>("Laser", prefabPath);


            GUILayout.Label("Items", EditorStyles.boldLabel);
            DisplayEditorPrefab<HackableData>("HackableObject", prefabPath);
            DisplayEditorPrefab<KeyData>("Key", prefabPath);
            DisplayEditorPrefab<NoiseGeneratorData>("NoiseItem", prefabPath);
            DisplayEditorPrefab<TaserData>("TaserItem", prefabPath);

            GUI.color = Color.red;
            if (GUILayout.Button("DEL"))
            {
                if (Selection.activeGameObject != null)
                {
                    DataContainer container = Selection.activeGameObject.GetComponent<IDataController>().GetContainer() as DataContainer;
                    _levelData.DataContainers.Remove(container);
                    AssetDatabase.RemoveObjectFromAsset(container);
                    AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(_levelData));
                    DestroyImmediate(Selection.activeGameObject);
                }
            }
            GUI.color = Color.white;

            GUILayout.EndArea();



            if (doorAnchored != null)
            {
                GUILayout.BeginArea(new Rect(Camera.current.pixelRect.width / 2f, 5, 150, 25), EditorStyles.helpBox);
                if (GUILayout.Button("Create Anchor Point"))
                {
                    if (Selection.activeGameObject.GetComponent<Door>() && Selection.activeGameObject.transform.childCount==1 &&  Selection.activeGameObject.transform.GetChild(0).name=="Door Anchor") {
                        (Selection.activeGameObject.GetComponent<Door>().GetContainer() as DoorData).AnchorPoint = Selection.activeGameObject.transform.GetChild(0).position;
                    }
                    else{

                        GameObject gm = new GameObject("Door Anchor");
                        ((DoorData)doorAnchored.GetComponent<Door>().GetContainer()).AnchorPoint = gm.transform.position;
                        gm.transform.SetParent(doorAnchored, true);
                        doorAnchored = gm.transform;
                        Selection.activeGameObject = gm;
                    }
                }
                GUILayout.EndArea();
            }

            if (Selection.activeGameObject != null)
            {
                if (Selection.activeGameObject.GetComponent<Enemy>())
                {
                    Debug.Log("called");
                    GUILayout.BeginArea(new Rect(Camera.current.pixelRect.width / 2f, 5, 250, 25), EditorStyles.helpBox);
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Add WayPoint"))
                    {
                        Enemy selectedEnemy = Selection.activeGameObject.GetComponent<Enemy>();
                        ((EnemyData)selectedEnemy.GetContainer()).Waypoints.Add(new Waypoint()
                        {
                            Angles = new List<WaypointAngle>()
                            {
                                new WaypointAngle()
                                {
                                    Angle = 0f,
                                    TimeToWait =1f,
                                    LookAtRatio = 1f
                                },
                                new WaypointAngle()
                                {
                                    Angle = 60f,
                                    TimeToWait =1f,
                                    LookAtRatio = 1f
                                }
                            }
                        });
                        EditorUtility.SetDirty(selectedEnemy);
                    }
                    if (editEnemyRadius)
                        GUI.color = Color.gray;
                    if (GUILayout.Button("Set Search Radius"))
                    {
                        editEnemyRadius ^= true;
                    }
                    GUI.color = Color.white;
                    EditorGUILayout.EndHorizontal();
                    GUILayout.EndArea();
                }
            }


            Handles.EndGUI();



            if (Selection.activeGameObject != null)
            {
                if (Selection.activeGameObject.GetComponent<Enemy>())
                {

                    Enemy selectedEnemy = Selection.activeGameObject.GetComponent<Enemy>();
                    if (editEnemyRadius)
                    {
                        ((EnemyData)selectedEnemy.GetContainer()).RandomPointRadius = Handles.RadiusHandle(Quaternion.identity, Path.data.gridGraph.center, ((EnemyData)selectedEnemy.GetContainer()).RandomPointRadius);
                    }
                    else
                    {
                        List<Waypoint> waypoints = ((EnemyData)selectedEnemy.GetContainer()).Waypoints;
                        List<Vector3> positions = new List<Vector3>();
                        for (int i = 0; i < waypoints.Count; i++)
                        {
                            Handles.color = Color.red;
                            if (Handles.Button(waypoints[i].Position + new Vector2(1f, 1f), Quaternion.identity, .3f, .3f, Handles.SphereHandleCap))
                            {
                                waypoints.Remove(waypoints[i]);
                                break;
                            }
                            Handles.color = Color.white;
                            waypoints[i].Position = Handles.PositionHandle(waypoints[i].Position, Quaternion.identity);

                            positions.Add(waypoints[i].Position);
                            if (waypoints[i].Angles.Count > 1)
                            {
                                JointAngularLimitHandle(waypoints[i].Position, Quaternion.identity, Vector3.one, ref waypoints[i].Angles[0].AngleVector, ref waypoints[i].Angles[1].AngleVector);
                            }
                        }
                        Handles.color = Color.red;
                        if (positions.Count > 0)
                            Handles.DrawAAPolyLine(10, positions.Concat(new Vector3[] { positions[0] }).ToArray());
                        Handles.color = Color.white;
                    }
                    EditorUtility.SetDirty(selectedEnemy);

                }

            }
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.LabelField("LEVEL EDITOR", EditorStyles.boldLabel);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("NEW LEVEL"))
            {
                displayNewLevel = true;
            }

            if (GUILayout.Button("LOAD LEVEL"))
            {
                displayNewLevel = false;
            }
            if (!displayNewLevel)
            {
                if (GUILayout.Button("CLOSE LEVEL"))
                {
                    CloseLevel();
                }
            }
            EditorGUILayout.EndHorizontal();

            if (!displayNewLevel)
            {
                if (_levelData != null && _levelData.PlayerData == null)
                    EditorGUILayout.HelpBox("[!]Player not spawned! Player create a player.", MessageType.Error);

                EditorGUILayout.Space();
            }

            if (displayNewLevel || (!displayNewLevel && _levelData != null))
            {
                if (_levelData != null)
                    levelId = _levelData.Id != -1 ? _levelData.Id : levelId;
                levelId = EditorGUILayout.IntField("Level ID:", levelId);
            }

            if (!displayNewLevel)
            {
                if (_levelData == null)
                    levelWasNull = true;
                _levelData = (LevelData)EditorGUILayout.ObjectField("Level SO:", _levelData, typeof(LevelData), false);
                //Frame null check
                if (levelWasNull && _levelData != null)
                {
                    levelWasNull = false;
                    LoadLevelData();
                }
                if (_levelData != null)
                {
                    _levelData.Name = EditorGUILayout.TextField("Level Name:", _levelData.Name);
                    EditorGUILayout.Space();
                    foreach (var dataPiece in _levelData.DataContainers)
                    {
                        EditorGUILayout.LabelField(new GUIContent(dataPiece.name));
                    }
                }
            }


            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (_levelData == null)
            {
                if (GUILayout.Button("SAVE"))
                {
                    if (!displayNewLevel && !string.IsNullOrEmpty(_levelData.Name) || displayNewLevel)
                    {
                        _levelData = LevelSerializer.Serialize(levelId);
                        displayNewLevel = false;
                    }
                }
            }
            else
            {
                if (GUILayout.Button("OVERWRITE SAVE"))
                    LevelSerializer.SerializeExisting(_levelData, levelId);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        private void DisplayPlayerButton(bool isTherePlayerData)
        {
            if (isTherePlayerData)
            {
                GUI.color = Color.red;
                if (GUILayout.Button("REMOVE PLAYER"))
                {
                    Player _player = FindObjectOfType<Player>();
                    if (_player != null)
                        DestroyImmediate(_player.gameObject);
                    if (_levelData.PlayerData == null)
                        return;
                    AssetDatabase.RemoveObjectFromAsset(_levelData.PlayerData);
                    AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(_levelData));
                    _levelData.PlayerData = null;
                }
                GUI.color = Color.white;
            }
            else
            {
                if (GUILayout.Button("ADD PLAYER"))
                {
                    var playerData = ScriptableObject.CreateInstance<PlayerData>();
                    playerData.name = "Player";
                    AssetDatabase.AddObjectToAsset(playerData, _levelData);
                    AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(_levelData));
                    _levelData.PlayerData = playerData;
                    EditorUtility.SetDirty(_levelData);
                    GameObject playerPrefab = Resources.Load<GameObject>("Editor/Player");
                    playerPrefab = Instantiate(playerPrefab);
                    Selection.activeGameObject = playerPrefab;
                    Tools.current = Tool.Move;
                    playerPrefab.GetComponent<Player>().Inject(_levelData.PlayerData);
                }
            }
        }

        void DisplayEditorPrefab<T>(string itemName, string path, Action<Transform> action = null) where T : ScriptableObject
        {
            if (GUILayout.Button(itemName))
            {
                GameObject shape = Resources.Load<GameObject>(path + itemName);
                shape = Instantiate(shape);
                //Add to destruction queue
                CreatedObjects.Add(shape);
                shape.name = itemName;
                var soInstance = ScriptableObjectFactory.CreateObject<T>(itemName);
                soInstance.name = itemName;
                AssetDatabase.AddObjectToAsset(soInstance, _levelData);
                AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(_levelData));
                _levelData.DataContainers.Add(soInstance as DataContainer);
                shape.GetComponent<IDataController>().Inject(soInstance as DataContainer);
                Selection.activeGameObject = shape;
                Tools.current = Tool.Move;
                action?.Invoke(shape.transform);
            }
        }

        void CloseLevel()
        {
            _levelData = null;
            levelId = -1;
            displayNewLevel = true;
            DestroyControllers();
            //GameObject[] objects = 
            //for (var i = 0; i < objects.Length; i++)
            //{
            //    DestroyImmediate(objects[i]);
            //}
        }

        void LoadLevelData()
        {
            DestroyControllers();
            CreatedObjects = LevelGenerator.GenerateLevel(_levelData);
        }

        private void DestroyControllers()
        {
            foreach (var created in CreatedObjects)
            {
                DestroyImmediate(created);
            }
        }

        static JointAngularLimitHandle jointAngularLimitHandle = new JointAngularLimitHandle();

        public static void JointAngularLimitHandle(Vector3 center, Quaternion rotation, Vector3 size, ref Vector3 minAngles, ref Vector3 maxAngles) { JointAngularLimitHandle(center, rotation, size, ref minAngles, ref maxAngles, jointAngularLimitHandle.xHandleColor, jointAngularLimitHandle.yHandleColor, jointAngularLimitHandle.zHandleColor); }
        public static void JointAngularLimitHandle(Vector3 center, Quaternion rotation, Vector3 size, ref Vector3 minAngles, ref Vector3 maxAngles, Color xHandleColor, Color yHandleColor, Color zHandleColor)
        {
            Matrix4x4 trs = Matrix4x4.TRS(center, rotation, size);

            using (new Handles.DrawingScope(trs))
            {
                jointAngularLimitHandle.xHandleColor = xHandleColor;
                jointAngularLimitHandle.yHandleColor = yHandleColor;
                jointAngularLimitHandle.zHandleColor = zHandleColor;

                jointAngularLimitHandle.xMin = minAngles.x;
                jointAngularLimitHandle.yMin = minAngles.y;
                jointAngularLimitHandle.zMin = minAngles.z;
                jointAngularLimitHandle.xMax = maxAngles.x;
                jointAngularLimitHandle.yMax = maxAngles.y;
                jointAngularLimitHandle.zMax = maxAngles.z;

                jointAngularLimitHandle.DrawHandle();

                minAngles.x = jointAngularLimitHandle.xMin;
                minAngles.y = jointAngularLimitHandle.yMin;
                minAngles.z = jointAngularLimitHandle.zMin;
                maxAngles.x = jointAngularLimitHandle.xMax;
                maxAngles.y = jointAngularLimitHandle.yMax;
                maxAngles.z = jointAngularLimitHandle.zMax;
            }
        }
    }


}