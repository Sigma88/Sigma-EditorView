using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using KSP.UI.TooltipTypes;


namespace SigmaEditorViewPlugin
{
    internal static class EditorLight
    {
        static Button newButton1;
        static Button newButton2;
        static SpriteState lightsON;
        static SpriteState lightsOFF;
        static Sprite spriteON;
        static Sprite spriteOFF;

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


                GameObject topBar = GameObject.Find("Top Bar");
                GameObject buttonCrew = topBar.GetChild("ButtonPanelCrew");
                GameObject buttonCargo = topBar.GetChild("ButtonPanelCargo");
                GameObject buttonEditor = topBar.GetChild("ButtonPanelEditor");

                Button oldButton = buttonCrew.GetComponent<Button>();

                GameObject buttonLight1 = Object.Instantiate(buttonCrew);
                buttonLight1.transform.SetParent(topBar.transform);
                buttonLight1.transform.position = buttonEditor.transform.position * 2 - buttonCargo.transform.position;
                buttonLight1.transform.localScale = buttonCargo.transform.localScale;
                buttonLight1.transform.rotation = buttonCargo.transform.rotation;

                GameObject buttonLight2 = Object.Instantiate(buttonCrew);
                buttonLight2.transform.SetParent(topBar.transform);
                buttonLight2.transform.position = buttonEditor.transform.position * 2 - buttonCargo.transform.position;
                buttonLight2.transform.localScale = buttonCargo.transform.localScale;
                buttonLight2.transform.rotation = buttonCargo.transform.rotation;

                Texture2D textureOFF = Resources.FindObjectsOfTypeAll<Texture2D>().FirstOrDefault(t => t.name == "Sigma/EditorView/Textures/LightsOFF");
                Texture2D textureON = Resources.FindObjectsOfTypeAll<Texture2D>().FirstOrDefault(t => t.name == "Sigma/EditorView/Textures/LightsON");

                Object.DestroyImmediate(buttonLight1.GetComponent<Button>());
                newButton1 = buttonLight1.AddOrGetComponent<Button>();
                newButton1.image = buttonLight1.GetComponent<Image>();

                Object.DestroyImmediate(buttonLight2.GetComponent<Button>());
                newButton2 = buttonLight2.AddOrGetComponent<Button>();
                newButton2.image = buttonLight2.GetComponent<Image>();

                buttonLight1.GetComponent<TooltipController_Text>().textString = buttonLight2.GetComponent<TooltipController_Text>().textString = "Toggle Lights";

                newButton1.transition = Selectable.Transition.SpriteSwap;
                newButton1.spriteState = lightsON = new SpriteState
                {
                    highlightedSprite = Sprite.Create(textureON, new Rect(128, 128, 128, 128), Vector2.zero),
                    pressedSprite = Sprite.Create(textureON, new Rect(0, 0, 128, 128), Vector2.zero),
                    disabledSprite = Sprite.Create(textureON, new Rect(128, 0, 128, 128), Vector2.zero)
                };
                newButton1.image.sprite = spriteON = Sprite.Create(textureON, new Rect(0, 128, 128, 128), Vector2.zero);

                newButton2.transition = Selectable.Transition.SpriteSwap;
                newButton2.spriteState = lightsOFF = new SpriteState
                {
                    highlightedSprite = Sprite.Create(textureOFF, new Rect(128, 128, 128, 128), Vector2.zero),
                    pressedSprite = Sprite.Create(textureOFF, new Rect(0, 0, 128, 128), Vector2.zero),
                    disabledSprite = Sprite.Create(textureOFF, new Rect(128, 0, 128, 128), Vector2.zero)
                };
                newButton2.image.sprite = spriteOFF = Sprite.Create(textureOFF, new Rect(0, 128, 128, 128), Vector2.zero);

                newButton1.onClick.AddListener(OnButtonClick);
                newButton1.onClick.AddListener(lightSwitch.Flip);
                newButton2.onClick.AddListener(OnButtonClick);
                newButton2.onClick.AddListener(lightSwitch.Flip);

                newButton1.gameObject.SetActive(true);
                newButton2.gameObject.SetActive(false);
            }
        }

        static void OnButtonClick()
        {
            bool state = !newButton1.isActiveAndEnabled;

            newButton1.gameObject.SetActive(state);
            newButton2.gameObject.SetActive(!state);
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
