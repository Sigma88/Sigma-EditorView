using System.Linq;
using UnityEngine;


namespace SigmaEditorViewPlugin
{
    internal static class EditorLight
    {
        internal static void Apply(EditorFacility editor)
        {
            Debug.Log("EditorLight.Apply", "editor = " + editor);

            if (Settings.toggleLights)
            {
                // Fix Light
                GameObject lightObject = GameObject.Find("SpotlightSun") ?? GameObject.Find("ExteriorSun") ?? GameObject.Find("Realtime_ExteriorSun");
                Debug.Log("EditorLight.Apply", "lightObject = " + lightObject);

                if (lightObject != null)
                {
                    Light light = lightObject.GetComponent<Light>();
                    Debug.Log("EditorLight.Apply", "light = " + light);

                    if (light != null)
                    {
                        light.range = 3500;
                        light.intensity = 5;
                        light.spotAngle = 89;
                        light.cullingMask = 1 << 0 | 1 << 15;

                        if (editor == EditorFacility.SPH)
                        {
                            light.transform.position = new Vector3(0, 1800, 400);
                            light.transform.eulerAngles = new Vector3(61, 0, 0);
                        }
                        else
                        {
                            light.transform.position = new Vector3(400, 1800, 0);
                            light.transform.eulerAngles = new Vector3(61, 90, 0);
                        }
                    }
                }


                // LightSwitch

                GameObject building;
                if (EditorDriver.editorFacility == EditorFacility.SPH)
                    building = GameObject.Find("SPHlvl1") ?? GameObject.Find("SPHlvl2") ?? GameObject.Find("SPHmodern");
                else
                    building = GameObject.Find("VABlvl2") ?? GameObject.Find("VABlvl3") ?? GameObject.Find("VABmodern");

                Switch lightSwitch = building.AddOrGetComponent<Switch>();

                EditorButtons.lightsOnBtn.onClick.AddListener(lightSwitch.Flip);
                EditorButtons.lightsOffBtn.onClick.AddListener(lightSwitch.Flip);
            }
        }

        internal class Switch : MonoBehaviour
        {
            GameObject[] toggableObjects;
            Material[] toggableMaterials;

            bool state = true;

            void Start()
            {
                // Toggable 'GameObject's
                if (name == "VABlvl2")
                {
                    toggableObjects = new[]
                        {
                            gameObject.GetChild("SpotlightFlares")
                        };
                }
                else if (name == "VABlvl3")
                {
                    toggableObjects = new[]
                        {
                            gameObject.GetChild("VAB_Interior_BakeLights"),
                            gameObject.GetChild("FloosLights")
                        };
                }
                else if (name == "VABmodern")
                {
                    toggableObjects = new[]
                        {
                            gameObject.GetChild("model_vab_interior_floor_cover_v20"),
                            gameObject.GetChild("model_vab_interior_lights_accent_v16"),
                            gameObject.GetChild("lights_bottom1"),
                            gameObject.GetChild("lights_floor1"),
                            gameObject.GetChild("lights_top1")
                        };
                }
                else if (name == "SPHlvl1")
                {
                    toggableObjects = new[]
                        {
                            gameObject.GetChild("Bakelights")
                        };
                }
                else if (name == "SPHmodern")
                {
                    toggableObjects = new[]
                        {
                            gameObject.GetChild("rearWindowLightSplashes"),
                            gameObject.GetChild("Component_749_1"),
                            gameObject.GetChild("Component_750_1")
                        };
                }

                // Toggable 'Material's
                toggableMaterials = gameObject.GetComponentsInChildren<Renderer>(true).SelectMany(r => r.materials).Where(m => m.HasProperty("_EmissiveColor")).ToArray();
            }

            internal void Flip()
            {
                state ^= true;

                if (toggableObjects != null)
                {
                    int n = toggableObjects.Length;
                    for (int i = 0; i < n; i++)
                    {
                        toggableObjects[i].SetActive(state);
                    }
                }

                if (toggableMaterials != null)
                {
                    int n = toggableMaterials.Length;
                    for (int i = 0; i < n; i++)
                    {
                        toggableMaterials[i].SetColor("_EmissiveColor", toggableMaterials[i].GetColor("_EmissiveColor") * (state ? 2.5f : 0.4f));
                    }
                }
            }
        }
    }
}
