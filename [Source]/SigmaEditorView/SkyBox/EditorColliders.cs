using UnityEngine;


namespace SigmaEditorViewPlugin
{
    internal static class EditorColliders
    {
        internal static void Apply(EditorFacility editor)
        {
            Debug.Log("EditorColliders.Apply", "editor = " + editor);

            GameObject building;
            if (editor == EditorFacility.SPH)
                building = GameObject.Find("SPHlvl1") ?? GameObject.Find("SPHlvl2") ?? GameObject.Find("SPHmodern");
            else
                building = GameObject.Find("VABlvl2") ?? GameObject.Find("VABlvl3") ?? GameObject.Find("VABmodern");

            Debug.Log("EditorColliders.Apply", "building = " + building);

            switch (building?.name)
            {
                case "SPHlvl1":
                    SPHwalls(new Vector3(-0.5f, 4.3f, 36), new Vector3(20, 8, 1));
                    break;
                case "SPHlvl2":
                    SPHwalls(new Vector3(-0.5f, 10, 42), new Vector3(42.5f, 19.5f, 1));
                    break;
                case "SPHmodern":
                    SPHwalls(new Vector3(0, 17, 61), new Vector3(78, 34, 1));
                    SPHcorners(new Vector3(-35, 35, 61), new Vector3(16, 7, 1), new Vector3(0, 0, 16));
                    break;
                case "VABlvl2":
                    VABwalls(new Vector3(30, 30.9f, 0), new Vector3(1, 61.8f, 40));
                    break;
                case "VABlvl3":
                    VABwalls(new Vector3(37, 29.5f, 0), new Vector3(1, 59, 40));
                    break;
                case "VABmodern":
                    VABwalls(new Vector3(34, 25.2f, 0), new Vector3(1, 50, 45));
                    break;
                default:
                    return;
            }
        }

        static void VABwalls(Vector3 position, Vector3 scale)
        {
            Debug.Log("EditorColliders.VABwalls", "position = " + position + ", scale = " + scale);

            // Front Wall
            CreateCollider("VAB_Front_Wall", new Vector3(0, position.y, scale.z * 0.5f), new Vector3(position.x * 2, scale.y, 1));
            // Back Wall
            CreateCollider("VAB_Back_Wall", new Vector3(0, position.y, -scale.z * 0.5f), new Vector3(position.x * 2, scale.y, 1));
            // Left Wall
            CreateCollider("VAB_Left_Wall", new Vector3(-position.x, position.y, position.z), scale);
            // Top Wall
            CreateCollider("VAB_Top_Wall", new Vector3(0, position.y + scale.y * 0.5f, 0), new Vector3(position.x * 2, 1, scale.z));
            // Bottom Wall
            CreateCollider("VAB_Bottom_Wall", new Vector3(0, position.y - scale.y * 0.5f, 0), new Vector3(position.x * 2, 1, scale.z));
        }

        static void SPHwalls(Vector3 position, Vector3 scale)
        {
            Debug.Log("EditorColliders.SPHwalls", "position = " + position + ", scale = " + scale);

            // Back Wall
            CreateCollider("SPH_Back_Wall", new Vector3(position.x, position.y, -position.z), scale);
            // Left Wall
            CreateCollider("SPH_Left_Wall", new Vector3(-scale.x * 0.5f, position.y, 0), new Vector3(1, scale.y, position.z * 2));
            // Right Wall
            CreateCollider("SPH_Right_Wall", new Vector3(scale.x * 0.5f, position.y, 0), new Vector3(1, scale.y, position.z * 2));
            // Top Wall
            CreateCollider("SPH_Top_Wall", new Vector3(0, position.y + scale.y * 0.5f, 0), new Vector3(scale.x, 1, position.z * 2));
            // Bottom Wall
            CreateCollider("SPH_Bottom_Wall", new Vector3(0, position.y - scale.y * 0.5f, 0), new Vector3(scale.x, 1, position.z * 2));
        }

        static void SPHcorners(Vector3 position, Vector3 scale, Vector3 rotation)
        {
            Debug.Log("EditorColliders.SPHcorners", "position = " + position + ", scale = " + scale + ", rotation = " + rotation);

            // Left Corner
            CreateCollider("SPH_Left_Corner", position, scale, rotation);
            // Right Corner
            CreateCollider("SPH_Right_Corner", new Vector3(-position.x, position.y, position.z), scale, new Vector3(rotation.x, rotation.y, -rotation.z));
        }

        static void CreateCollider(string name, Vector3 position, Vector3 scale, Vector3? rotation = null)
        {
            Debug.Log("EditorColliders.CreateCollider", "name = " + name + "position = " + position + ", scale = " + scale + ", rotation = " + rotation);

            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Object.DestroyImmediate(cube.GetComponent<Renderer>());
            cube.AddOrGetComponent<MeshCollider>();
            cube.AddOrGetComponent<Remover>();
            cube.name = name;
            cube.transform.position = position;
            cube.transform.localScale = scale;
            cube.transform.eulerAngles = rotation ?? Vector3.zero;
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
                if (EditorDriver.editorFacility != editor)
                {
                    Debug.Log("EditorColliders.Remover", "Destryoing gameObject = " + gameObject);

                    DestroyImmediate(gameObject);
                }
            }
        }
    }
}
