using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;


namespace SigmaEditorViewPlugin
{
    internal static class EditorColliders
    {
        static Dictionary<string, Vector3[]> innerWalls = new Dictionary<string, Vector3[]>
        {
            { "SPHlvl1", new Vector3[] { new Vector3(0, 9f, 0), new Vector3(50, 18, 71.25f) } },
            { "SPHlvl2", new Vector3[] { new Vector3(0, 11.75f, 0), new Vector3(65, 23.5f, 85) } },
            { "SPHmodern", new Vector3[] { new Vector3(0, 19, 0), new Vector3(100, 38, 122) } },
            { "VABlvl2", new Vector3[] { new Vector3(0, 32, 0), new Vector3(58.5f, 64, 60) } },
            { "VABlvl3", new Vector3[] { new Vector3(0, 42.5f, 0), new Vector3(77, 85, 70) } },
            { "VABmodern", new Vector3[] { new Vector3(0, 37.5f, 0), new Vector3(67.5f, 75, 55) } }
        };
        static Dictionary<string, Vector3[]> outerWalls = new Dictionary<string, Vector3[]>
        {
            { "SPHlvl1", new Vector3[] { new Vector3(0, 9f, 0), new Vector3(51, 19, 71.25f) } },
            { "SPHlvl2", new Vector3[] { new Vector3(0, 13, 0), new Vector3(75, 32, 85) } },
            { "SPHmodern", new Vector3[] { new Vector3(0, 19, 0), new Vector3(110, 42, 122) } },
            { "VABlvl2", new Vector3[] { new Vector3(0, 32, 0), new Vector3(58.5f, 68, 65) } },
            { "VABlvl3", new Vector3[] { new Vector3(0, 43, 0), new Vector3(77, 88, 75) } },
            { "VABmodern", new Vector3[] { new Vector3(0, 43f, 0), new Vector3(67.5f, 90, 65) } }
        };

        static Dictionary<string, Vector3[]> doors = new Dictionary<string, Vector3[]>
        {
            { "SPHlvl1", new Vector3[] { new Vector3(-0.6f, 4.2f, 35.5f), new Vector3(20, 8.4f, 0) } },
            { "SPHlvl2", new Vector3[] { new Vector3(-0.45f, 9.85f, 42.5f), new Vector3(42.75f, 19.4f, 0) } },
            { "SPHmodern", new Vector3[] { new Vector3(0, 17, 61), new Vector3(79, 34, 0) } },
            { "VABlvl2", new Vector3[] { new Vector3(29, 30.9f, 0.5f), new Vector3(0, 62, 28) } },
            { "VABlvl3", new Vector3[] { new Vector3(37.5f, 29.75f, 0), new Vector3(0, 59.5f, 40.5f) } },
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

            if (Debug.debug) { DebugColliders(); DebugShadows(); }

            GameObject building;
            if (editor == EditorFacility.SPH)
                building = GameObject.Find("SPHlvl1") ?? GameObject.Find("SPHlvl2") ?? GameObject.Find("SPHmodern");
            else
                building = GameObject.Find("VABlvl2") ?? GameObject.Find("VABlvl3") ?? GameObject.Find("VABmodern");

            Debug.Log("EditorColliders.Apply", "building = " + building);

            string name = building?.name;

            if (innerWalls.ContainsKey(name) && doors.ContainsKey(name))
            {
                // Create Walls
                GameObject[] box = Box(name, innerWalls[name][0], innerWalls[name][1]);
                GameObject[] shadows = Box(name, outerWalls[name][0], outerWalls[name][1], true);

                // Cut out Door
                switch (name)
                {
                    case "SPHlvl1":
                    case "SPHlvl2":
                    case "SPHmodern":
                        Door(box[5], doors[name][0], doors[name][1]);
                        Door(shadows[5], doors[name][0], doors[name][1], true);
                        break;
                    case "VABlvl2":
                    case "VABlvl3":
                    case "VABmodern":
                        Door(box[1], doors[name][0], doors[name][1]);
                        Door(shadows[1], doors[name][0], doors[name][1], true);
                        break;
                }

                if (corners.ContainsKey(name))
                {
                    Corners(name, corners[name][0], corners[name][1], corners[name][2]);
                    Corners(name, corners[name][0], corners[name][1], corners[name][2], true);
                }
            }
        }

        static GameObject[] Box(string name, Vector3 position, Vector3 scale, bool shadows = false)
        {
            Debug.Log("EditorColliders.Box", "position = " + position + ", scale = " + scale + ", shadows = " + shadows);

            return new GameObject[]
            {
                // Left Wall
                CreateCollider(name + "_Left_Wall", position.dX(position.x - scale.x * 0.5f), scale.X(1), null, shadows),
                // Right Wall
                CreateCollider(name + "_Right_Wall", position.dX(position.x + scale.x * 0.5f), scale.X(1), null, shadows),
                // Bottom Wall
                CreateCollider(name + "_Bottom_Wall", position.Y(position.y - scale.y * 0.5f), scale.Y(1), null, shadows),
                // Top Wall
                CreateCollider(name + "_Top_Wall", position.Y(position.y + scale.y * 0.5f), scale.Y(1), null, shadows),
                // Back Wall
                CreateCollider(name + "_Back_Wall", position.dZ(position.z - scale.z * 0.5f), scale.Z(1), null, shadows),
                // Front Wall
                CreateCollider(name + "_Front_Wall", position.dZ(position.z + scale.z * 0.5f), scale.Z(1), null, shadows)
            };
        }

        static void Door(GameObject wall, Vector3 position, Vector3 scale, bool shadows = false)
        {
            Debug.Log("EditorColliders.Door", "wall = " + wall + ", position = " + position + ", scale = " + scale + ", shadows = " + shadows);

            if (scale.y > 0)
            {
                Debug.Log("EditorColliders.Door", "wallPosition = " + wall.transform.position + ", wallScale = " + wall.transform.localScale);

                CreateCollider
                (
                    wall.name + "_A",
                    wall.transform.position.Y(((wall.transform.position.y + wall.transform.localScale.y * 0.5f) + (position.y + scale.y * 0.5f)) * 0.5f),
                    wall.transform.localScale.Y((wall.transform.position.y + wall.transform.localScale.y * 0.5f) - (position.y + scale.y * 0.5f)),
                    null, shadows
                );

                // SPH
                if (scale.x > 0)
                {
                    CreateCollider
                    (
                        wall.name + "_B",
                        wall.transform.position.X((-wall.transform.localScale.x + position.x * 2 - scale.x) * 0.25f),
                        wall.transform.localScale.X((wall.transform.localScale.x + position.x * 2 - scale.x) * 0.5f),
                        null, shadows
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
                        wall.transform.localScale.Z((wall.transform.localScale.z + position.z * 2 - scale.z) * 0.5f),
                        null, shadows
                    );

                    wall.name += "_C";
                    wall.transform.position = wall.transform.position.Z((wall.transform.localScale.z + position.z * 2 + scale.z) * 0.25f);
                    wall.transform.localScale = wall.transform.localScale.Z((wall.transform.localScale.z - position.z * 2 - scale.z) * 0.5f);

                    Debug.Log("EditorColliders.Door", "name = " + wall.name + ", position = " + wall.transform.position + ", scale = " + wall.transform.localScale + ", rotation = " + wall.transform.eulerAngles);
                }

                // DOORS
                if (!shadows)
                {
                    if (EditorDoor.shadeL) Object.DestroyImmediate(EditorDoor.shadeL);
                    EditorDoor.shadeL = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    EditorDoor.shadeL.GetComponent<Renderer>().shadowCastingMode = ShadowCastingMode.ShadowsOnly;
                    if (EditorDoor.shadeR) Object.DestroyImmediate(EditorDoor.shadeR);
                    EditorDoor.shadeR = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    EditorDoor.shadeR.GetComponent<Renderer>().shadowCastingMode = ShadowCastingMode.ShadowsOnly;

                    if (EditorDoor.doorL) Object.DestroyImmediate(EditorDoor.doorL);
                    EditorDoor.doorL = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    if (EditorDoor.doorR) Object.DestroyImmediate(EditorDoor.doorR);
                    EditorDoor.doorR = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    MeshRenderer mrL = EditorDoor.doorL.AddOrGetComponent<MeshRenderer>();
                    MeshRenderer mrR = EditorDoor.doorR.AddOrGetComponent<MeshRenderer>();
                    mrL.material.mainTexture = mrR.material.mainTexture = Settings.doorTex;
                    mrL.material.mainTextureScale = mrR.material.mainTextureScale = Settings.doorTexScale;
                    mrL.material.SetTexture("_BumpMap", Settings.doorBump);
                    mrR.material.SetTexture("_BumpMap", Settings.doorBump);
                    mrL.material.SetTextureScale("_BumpMap", Settings.doorTexScale);
                    mrR.material.SetTextureScale("_BumpMap", Settings.doorTexScale);
                    mrL.material.SetFloat("_Glossiness", Settings.doorGloss);
                    mrR.material.SetFloat("_Glossiness", Settings.doorGloss);

                    EditorDoor.doorL.layer = EditorDoor.doorR.layer = 15;
                    EditorDoor.doorL.transform.localScale = EditorDoor.doorR.transform.localScale = scale.x == 0 ? scale.X(0.1f).Z(scale.z / 2f) : scale.Z(0.1f).X(scale.x / 2f);
                    EditorDoor.shadeL.transform.localScale = EditorDoor.shadeR.transform.localScale = scale.x == 0 ? EditorDoor.doorL.transform.localScale.X(0) : EditorDoor.doorL.transform.localScale.Z(0);
                    EditorDoor.doorL.transform.position = EditorDoor.shadeL.transform.position = scale.x == 0 ? position.dZ(-EditorDoor.doorL.transform.localScale.z / 2f) : position.dX(-EditorDoor.doorL.transform.localScale.x / 2f);
                    EditorDoor.doorR.transform.position = EditorDoor.shadeR.transform.position = scale.x == 0 ? position.dZ(EditorDoor.doorL.transform.localScale.z / 2f) : position.dX(EditorDoor.doorL.transform.localScale.x / 2f);

                    EditorDoor.doorL.AddOrGetComponent<EditorDoor.Mover>().open = EditorDoor.shadeL.AddOrGetComponent<EditorDoor.Mover>().open = scale.x == 0 ? EditorDoor.doorL.transform.position.dZ(-EditorDoor.doorL.transform.localScale.z) : EditorDoor.doorL.transform.position.dX(-EditorDoor.doorL.transform.localScale.x);
                    EditorDoor.doorR.AddOrGetComponent<EditorDoor.Mover>().open = EditorDoor.shadeR.AddOrGetComponent<EditorDoor.Mover>().open = scale.x == 0 ? EditorDoor.doorR.transform.position.dZ(EditorDoor.doorL.transform.localScale.z) : EditorDoor.doorR.transform.position.dX(EditorDoor.doorL.transform.localScale.x);
                }
            }
        }

        static void Corners(string name, Vector3 position, Vector3 scale, Vector3 rotation, bool shadows = false)
        {
            Debug.Log("EditorColliders.Corners", "position = " + position + ", scale = " + scale + ", rotation = " + rotation + ", shadows = " + shadows);

            // Corner A
            CreateCollider(name + "_Corner_A", position, scale, rotation, shadows);
            // Corner B
            if (name.StartsWith("SPH"))
                CreateCollider(name + "_Corner_B", position.X(-position.x), scale, rotation.Z(-rotation.z), shadows);
            else
                CreateCollider(name + "_Corner_B", position.Z(-position.z), scale, rotation.X(-rotation.x), shadows);
        }

        static GameObject CreateCollider(string name, Vector3 position, Vector3 scale, Vector3? rotation = null, bool shadows = false)
        {
            Debug.Log("EditorColliders.CreateCollider", "name = " + name + ", position = " + position + ", scale = " + scale + ", rotation = " + (rotation ?? Vector3.zero));

            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = name;

            Renderer renderer = cube.GetComponent<Renderer>();
            if (shadows)
            {
                cube.name += "_Shadows";
                renderer.material.color = Color.magenta;
                renderer.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
            }
            else
            {
                renderer.enabled = false;
                renderer.shadowCastingMode = ShadowCastingMode.Off;
                cube.AddOrGetComponent<MeshCollider>();
            }

            cube.AddOrGetComponent<Remover>();
            cube.transform.position = position;
            cube.transform.localScale = scale;
            cube.transform.eulerAngles = rotation ?? Vector3.zero;

            return cube;
        }

        static int debug = 0;

        static void DebugColliders()
        {
            Debug.Log("EditorColliders.DebugColliders");

            string path = "GameData/Sigma/EditorView/Debug/Colliders/";

            foreach (string name in innerWalls.Keys)
            {
                if (debug == 0 || !System.IO.File.Exists(path + name + ".txt"))
                {
                    if (!System.IO.Directory.Exists(path))
                        System.IO.Directory.CreateDirectory(path);

                    System.IO.File.WriteAllLines
                    (
                        path + name + ".txt",
                        new string[]
                        {
                            innerWalls[name][0].x + ", " + innerWalls[name][0].y + ", " + innerWalls[name][0].z,
                            innerWalls[name][1].x + ", " + innerWalls[name][1].y + ", " + innerWalls[name][1].z,
                            doors[name][0].x + ", " + doors[name][0].y + ", " + doors[name][0].z,
                            doors[name][1].x + ", " + doors[name][1].y + ", " + doors[name][1].z,
                            corners.ContainsKey(name) ? corners[name][0].x + ", " + corners[name][0].y + ", " + corners[name][0].z : "",
                            corners.ContainsKey(name) ? corners[name][1].x + ", " + corners[name][1].y + ", " + corners[name][1].z : "",
                            corners.ContainsKey(name) ? corners[name][2].x + ", " + corners[name][2].y + ", " + corners[name][2].z : ""
                        }
                    );
                }

                string[] values = System.IO.File.ReadAllLines(path + name + ".txt");
                innerWalls[name][0] = ConfigNode.ParseVector3(values[0]);
                innerWalls[name][1] = ConfigNode.ParseVector3(values[1]);
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

        static void DebugShadows()
        {
            Debug.Log("EditorColliders.DebugShadows");

            string path = "GameData/Sigma/EditorView/Debug/Shadows/";

            foreach (string name in outerWalls.Keys)
            {
                if (debug++ == 0 || !System.IO.File.Exists(path + name + ".txt"))
                {
                    if (!System.IO.Directory.Exists(path))
                        System.IO.Directory.CreateDirectory(path);

                    System.IO.File.WriteAllLines
                    (
                        path + name + ".txt",
                        new string[]
                        {
                            outerWalls[name][0].x + ", " + outerWalls[name][0].y + ", " + outerWalls[name][0].z,
                            outerWalls[name][1].x + ", " + outerWalls[name][1].y + ", " + outerWalls[name][1].z
                        }
                    );
                }

                string[] values = System.IO.File.ReadAllLines(path + name + ".txt");
                outerWalls[name][0] = ConfigNode.ParseVector3(values[0]);
                outerWalls[name][1] = ConfigNode.ParseVector3(values[1]);
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
                if (EditorDriver.editorFacility != editor || (Debug.debug && Input.GetKeyDown(KeyCode.Space)))
                {
                    Debug.Log("EditorColliders.Remover", "Destryoing gameObject = " + gameObject);

                    DestroyImmediate(gameObject);
                }

                if (Debug.debug && Input.GetKeyDown(KeyCode.C))
                {
                    // Toggle Renderer by pressing 'C'
                    Renderer renderer = GetComponent<Renderer>();
                    switch (renderer.shadowCastingMode)
                    {
                        case ShadowCastingMode.Off:
                            renderer.enabled ^= true;
                            break;
                        case ShadowCastingMode.On:
                            renderer.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
                            break;
                        case ShadowCastingMode.ShadowsOnly:
                            renderer.shadowCastingMode = ShadowCastingMode.On;
                            break;
                    }
                }
            }
        }
    }
}
