using System.Collections.Generic;
using UnityEngine;


namespace SigmaEditorViewPlugin
{
    internal static class EditorColliders
    {
        static Dictionary<string, Vector3[]> walls = new Dictionary<string, Vector3[]>
        {
            { "SPHlvl1", new Vector3[] { new Vector3(0, 9.3f, 0), new Vector3(50, 18, 71.25f) } },
            { "SPHlvl2", new Vector3[] { new Vector3(0, 12, 0), new Vector3(65, 23.5f, 85) } },
            { "SPHmodern", new Vector3[] { new Vector3(0, 19, 0), new Vector3(100, 38, 122) } },
            { "VABlvl2", new Vector3[] { new Vector3(0, 32, 0), new Vector3(58.5f, 64, 60) } },
            { "VABlvl3", new Vector3[] { new Vector3(0, 42.5f, 0), new Vector3(77, 85, 70) } },
            { "VABmodern", new Vector3[] { new Vector3(0, 37.5f, 0), new Vector3(67.5f, 75, 55) } }
        };

        static Dictionary<string, Vector3[]> doors = new Dictionary<string, Vector3[]>
        {
            { "SPHlvl1", new Vector3[] { new Vector3(-0.55f, 4.3f, 36), new Vector3(19.9f, 8.15f, 0) } },
            { "SPHlvl2", new Vector3[] { new Vector3(-0.45f, 10, 42), new Vector3(42.75f, 19.36f, 0) } },
            { "SPHmodern", new Vector3[] { new Vector3(0, 17, 61), new Vector3(79, 34, 0) } },
            { "VABlvl2", new Vector3[] { new Vector3(30, 30.9f, 0.375f), new Vector3(0, 62, 27) } },
            { "VABlvl3", new Vector3[] { new Vector3(37.5f, 29.75f, 0), new Vector3(0, 59.5f, 39.9f) } },
            { "VABmodern", new Vector3[] { new Vector3(33.75f, 25.25f, 0), new Vector3(0, 50.5f, 43.5f) } }
        };

        static Dictionary<string, Vector3[]> corners = new Dictionary<string, Vector3[]>
        {
            { "SPHmodern", new Vector3[] { new Vector3(-35, 35, 61), new Vector3(16, 7, 1), new Vector3(0, 0, 16) } },
            { "VABlvl3", new Vector3[] { new Vector3(36.5f, 42.5f, 25.5f), new Vector3(8, 85, 10), new Vector3(0, 0, 0) } },
            { "VABmodern", new Vector3[] { new Vector3(35, 37.5f, 26.8f), new Vector3(10, 75, 10), new Vector3(-1, 0, 0) } }
        };

        internal static void Apply(EditorFacility editor)
        {
            Debug.Log("EditorColliders.Apply", "editor = " + editor);

            if (Debug.debug) { DebugColliders(); }

            GameObject building;
            if (editor == EditorFacility.SPH)
                building = GameObject.Find("SPHlvl1") ?? GameObject.Find("SPHlvl2") ?? GameObject.Find("SPHmodern");
            else
                building = GameObject.Find("VABlvl2") ?? GameObject.Find("VABlvl3") ?? GameObject.Find("VABmodern");

            Debug.Log("EditorColliders.Apply", "building = " + building);

            string name = building?.name;

            if (walls.ContainsKey(name) && doors.ContainsKey(name))
            {
                // Create Walls
                GameObject[] box = Box(name, walls[name][0], walls[name][1]);

                // Cut out Door
                switch (name)
                {
                    case "SPHlvl1":
                    case "SPHlvl2":
                    case "SPHmodern":
                        Door(box[5], doors[name][0], doors[name][1]);
                        break;
                    case "VABlvl2":
                    case "VABlvl3":
                    case "VABmodern":
                        Door(box[1], doors[name][0], doors[name][1]);
                        break;
                }

                if (corners.ContainsKey(name))
                {
                    Corners(name, corners[name][0], corners[name][1], corners[name][2]);
                }
            }
        }

        static GameObject[] Box(string name, Vector3 position, Vector3 scale)
        {
            Debug.Log("EditorColliders.Box", "position = " + position + ", scale = " + scale);

            return new GameObject[]
            {
                // Left Wall
                CreateCollider(name + "_Left_Wall", position.dX(-scale.x * 0.5f), scale.X(1)),
                // Right Wall
                CreateCollider(name + "_Right_Wall", position.dX(scale.x * 0.5f), scale.X(1)),
                // Bottom Wall
                CreateCollider(name + "_Bottom_Wall", position.Y(0), scale.Y(1)),
                // Top Wall
                CreateCollider(name + "_Top_Wall", position.Y(scale.y), scale.Y(1)),
                // Back Wall
                CreateCollider(name + "_Back_Wall", position.dZ(-scale.z * 0.5f), scale.Z(1)),
                // Front Wall
                CreateCollider(name + "_Front_Wall", position.dZ(scale.z * 0.5f), scale.Z(1))
            };
        }

        static void Door(GameObject wall, Vector3 position, Vector3 scale)
        {
            Debug.Log("EditorColliders.Door", "wall = " + wall + ", position = " + position + ", scale = " + scale);

            if (scale.y > 0)
            {
                Debug.Log("EditorColliders.Door", "wallPosition = " + wall.transform.position + ", wallScale = " + wall.transform.localScale);

                CreateCollider
                (
                    wall.name + "_A",
                    wall.transform.position.dY(scale.y * 0.5f),
                    wall.transform.localScale.dY(-scale.y)
                );

                // SPH
                if (scale.x > 0)
                {
                    CreateCollider
                    (
                        wall.name + "_B",
                        wall.transform.position.X((-wall.transform.localScale.x + position.x * 2 - scale.x) * 0.25f),
                        wall.transform.localScale.X((wall.transform.localScale.x + position.x * 2 - scale.x) * 0.5f)
                    );

                    wall.name += "_C";
                    wall.transform.position = wall.transform.position.X((wall.transform.localScale.x + position.x * 2 + scale.x) * 0.25f);
                    wall.transform.localScale = wall.transform.localScale.X((wall.transform.localScale.x - position.x * 2 - scale.x) * 0.5f);

                    Debug.Log("EditorColliders.Door", "name = " + wall.name + ", position = " + wall.transform.position + ", scale = " + wall.transform.localScale + ", rotation = " + wall.transform.eulerAngles);
                }

                else

                // VAB
                if (scale.z > 0)
                {
                    CreateCollider
                    (
                        wall.name + "_B",
                        wall.transform.position.Z((-wall.transform.localScale.z + position.z * 2 - scale.z) * 0.25f),
                        wall.transform.localScale.Z((wall.transform.localScale.z + position.z * 2 - scale.z) * 0.5f)
                    );

                    wall.name += "_C";
                    wall.transform.position = wall.transform.position.Z((wall.transform.localScale.z + position.z * 2 + scale.z) * 0.25f);
                    wall.transform.localScale = wall.transform.localScale.Z((wall.transform.localScale.z - position.z * 2 - scale.z) * 0.5f);

                    Debug.Log("EditorColliders.Door", "name = " + wall.name + ", position = " + wall.transform.position + ", scale = " + wall.transform.localScale + ", rotation = " + wall.transform.eulerAngles);
                }
            }
        }

        static void Corners(string name, Vector3 position, Vector3 scale, Vector3 rotation)
        {
            Debug.Log("EditorColliders.Corners", "position = " + position + ", scale = " + scale + ", rotation = " + rotation);

            // Corner A
            CreateCollider(name + "_Corner_A", position, scale, rotation);
            // Corner B
            if (name.StartsWith("SPH"))
                CreateCollider(name + "_Corner_B", position.X(-position.x), scale, rotation.Z(-rotation.z));
            else
                CreateCollider(name + "_Corner_B", position.Z(-position.z), scale, rotation.X(-rotation.x));
        }

        static GameObject CreateCollider(string name, Vector3 position, Vector3 scale, Vector3? rotation = null)
        {
            Debug.Log("EditorColliders.CreateCollider", "name = " + name + ", position = " + position + ", scale = " + scale + ", rotation = " + (rotation ?? Vector3.zero));

            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.GetComponent<Renderer>().enabled = false;
            cube.AddOrGetComponent<MeshCollider>();
            cube.AddOrGetComponent<Remover>();
            cube.name = name;
            cube.transform.position = position;
            cube.transform.localScale = scale;
            cube.transform.eulerAngles = rotation ?? Vector3.zero;

            return cube;
        }

        static void DebugColliders()
        {
            Debug.Log("EditorColliders.DebugColliders");

            string path = "GameData/Sigma/EditorView/Debug/";

            foreach (string name in walls.Keys)
            {
                if (!System.IO.File.Exists(path + name + ".txt"))
                {
                    if (!System.IO.Directory.Exists(path))
                        System.IO.Directory.CreateDirectory(path);

                    System.IO.File.WriteAllLines
                    (
                        path + name + ".txt",
                        new string[]
                        {
                            walls[name][0].x + ", " + walls[name][0].y + ", " + walls[name][0].z,
                            walls[name][1].x + ", " + walls[name][1].y + ", " + walls[name][1].z,
                            doors[name][0].x + ", " + doors[name][0].y + ", " + doors[name][0].z,
                            doors[name][1].x + ", " + doors[name][1].y + ", " + doors[name][1].z,
                            corners.ContainsKey(name) ? corners[name][0].x + ", " + corners[name][0].y + ", " + corners[name][0].z : "",
                            corners.ContainsKey(name) ? corners[name][1].x + ", " + corners[name][1].y + ", " + corners[name][1].z : "",
                            corners.ContainsKey(name) ? corners[name][2].x + ", " + corners[name][2].y + ", " + corners[name][2].z : ""
                        }
                    );
                }

                string[] values = System.IO.File.ReadAllLines(path + name + ".txt");
                walls[name][0] = ConfigNode.ParseVector3(values[0]);
                walls[name][1] = ConfigNode.ParseVector3(values[1]);
                doors[name][0] = ConfigNode.ParseVector3(values[2]);
                doors[name][1] = ConfigNode.ParseVector3(values[3]);
                if (corners.ContainsKey(name))
                {
                    corners[name][0] = ConfigNode.ParseVector3(values[4]);
                    corners[name][1] = ConfigNode.ParseVector3(values[5]);
                    corners[name][2] = ConfigNode.ParseVector3(values[6]);
                }
            }
        }

        class Remover : MonoBehaviour
        {
            EditorFacility editor;

            void Awake()
            {
                editor = EditorDriver.editorFacility;
            }

            void Update()
            {
                if (EditorDriver.editorFacility != editor || (Debug.debug && Input.GetKeyDown("space")))
                {
                    Debug.Log("EditorColliders.Remover", "Destryoing gameObject = " + gameObject);

                    DestroyImmediate(gameObject);
                }

                if (Debug.debug && Input.GetKeyDown(KeyCode.C))
                {
                    // Toggle Renderer by pressing 'C'
                    GetComponent<Renderer>().enabled ^= true;
                }
            }
        }
    }
}
