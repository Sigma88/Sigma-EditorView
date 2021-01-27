using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using KSP.UI.TooltipTypes;


namespace SigmaEditorViewPlugin
{
    internal static class EditorDoor
    {
        internal static GameObject shadeL;
        internal static GameObject doorL;
        internal static GameObject shadeR;
        internal static GameObject doorR;

        static Button newButton1;
        static Button newButton2;
        static SpriteState doorON;
        static SpriteState doorOFF;
        static Sprite spriteON;
        static Sprite spriteOFF;

        static Dictionary<EditorFacility, bool> doorState = null;

        internal static void Apply(EditorFacility editor)
        {
            Debug.Log("EditorDoor.Apply", "editor = " + editor);

            // Renderer
            Debug.Log("EditorDoor.Apply", "doorL = " + doorL + ", shadeL = " + shadeL);
            Debug.Log("EditorDoor.Apply", "doorR = " + doorR + ", shadeR = " + shadeR);

            if (doorState == null)
            {
                doorState = new Dictionary<EditorFacility, bool>() { { EditorFacility.SPH, !Settings.closeDoors }, { EditorFacility.VAB, !Settings.closeDoors } };
            }

            if (Settings.toggleDoors)
            {
                GameObject topBar = GameObject.Find("Top Bar");
                GameObject buttonCrew = topBar.GetChild("ButtonPanelCrew");
                GameObject buttonEditor = topBar.GetChild("ButtonPanelEditor");

                Button oldButton = buttonCrew.GetComponent<Button>();

                GameObject buttonDoor1 = Object.Instantiate(buttonCrew);
                buttonDoor1.transform.SetParent(topBar.transform);
                GameObject buttonDoor2 = Object.Instantiate(buttonCrew);
                buttonDoor2.transform.SetParent(topBar.transform);

                buttonDoor1.transform.position = buttonDoor2.transform.position = buttonEditor.transform.position + (buttonEditor.transform.position - buttonCrew.transform.position) * 2;
                buttonDoor1.transform.localScale = buttonDoor2.transform.localScale = buttonCrew.transform.localScale;
                buttonDoor1.transform.rotation = buttonDoor2.transform.rotation = buttonCrew.transform.rotation;


                Texture2D textureOFF = Resources.FindObjectsOfTypeAll<Texture2D>().FirstOrDefault(t => t.name == "Sigma/EditorView/Textures/DoorOFF");
                Texture2D textureON = Resources.FindObjectsOfTypeAll<Texture2D>().FirstOrDefault(t => t.name == "Sigma/EditorView/Textures/DoorON");

                Object.DestroyImmediate(buttonDoor1.GetComponent<Button>());
                newButton1 = buttonDoor1.AddOrGetComponent<Button>();
                newButton1.image = buttonDoor1.GetComponent<Image>();

                Object.DestroyImmediate(buttonDoor2.GetComponent<Button>());
                newButton2 = buttonDoor2.AddOrGetComponent<Button>();
                newButton2.image = buttonDoor2.GetComponent<Image>();

                buttonDoor1.GetComponent<TooltipController_Text>().textString = "Close Doors";
                buttonDoor2.GetComponent<TooltipController_Text>().textString = "Open Doors";

                newButton1.transition = Selectable.Transition.SpriteSwap;
                newButton1.spriteState = doorON = new SpriteState
                {
                    highlightedSprite = Sprite.Create(textureON, new Rect(128, 128, 128, 128), Vector2.zero),
                    pressedSprite = Sprite.Create(textureON, new Rect(0, 0, 128, 128), Vector2.zero),
                    disabledSprite = Sprite.Create(textureON, new Rect(128, 0, 128, 128), Vector2.zero)
                };
                newButton1.image.sprite = spriteON = Sprite.Create(textureON, new Rect(0, 128, 128, 128), Vector2.zero);

                newButton2.transition = Selectable.Transition.SpriteSwap;
                newButton2.spriteState = doorOFF = new SpriteState
                {
                    highlightedSprite = Sprite.Create(textureOFF, new Rect(128, 128, 128, 128), Vector2.zero),
                    pressedSprite = Sprite.Create(textureOFF, new Rect(0, 0, 128, 128), Vector2.zero),
                    disabledSprite = Sprite.Create(textureOFF, new Rect(128, 0, 128, 128), Vector2.zero)
                };
                newButton2.image.sprite = spriteOFF = Sprite.Create(textureOFF, new Rect(0, 128, 128, 128), Vector2.zero);

                newButton1.onClick.AddListener(OnButtonClick);
                newButton2.onClick.AddListener(OnButtonClick);

                newButton1.gameObject.SetActive(doorState[editor]);
                newButton2.gameObject.SetActive(!doorState[editor]);
            }
            else
            {
                doorL.SetActive(Settings.closeDoors);
                shadeL.SetActive(Settings.closeDoors);
                doorR.SetActive(Settings.closeDoors);
                shadeR.SetActive(Settings.closeDoors);
            }
        }

        static void OnButtonClick()
        {
            bool state = !newButton1.isActiveAndEnabled;

            newButton1.gameObject.SetActive(state);
            newButton2.gameObject.SetActive(!state);
        }

        internal class Mover : MonoBehaviour
        {
            internal Vector3 open = Vector3.one;
            internal Vector3 close = Vector3.one;
            internal int direction = 1;  // -1 = open, 1 = close
            internal float position = 0; //  0 = open, 1 = close
            AudioSource doorSound;
            AudioClip doorStop;

            internal void Reverse()
            {
                direction *= -1;
                doorSound.Stop();
            }

            void Start()
            {
                direction = Settings.closeDoors ? 1 : -1;
                position = Settings.closeDoors ? 1 : 0;
                newButton1.onClick.AddListener(Reverse);
                newButton2.onClick.AddListener(Reverse);
                close = transform.position;
                transform.position = Settings.closeDoors ? close : open;

                doorSound = gameObject.AddOrGetComponent<AudioSource>();
                doorSound.clip = Resources.FindObjectsOfTypeAll<AudioClip>().FirstOrDefault(c => c.name == "Sigma/EditorView/Sounds/DoorMove");
                doorSound.loop = true;
                doorStop = Resources.FindObjectsOfTypeAll<AudioClip>().FirstOrDefault(c => c.name == "Sigma/EditorView/Sounds/DoorStop");
            }

            void Update()
            {
                if (direction > 0 && position < 1)
                {
                    if (!doorSound.isPlaying)
                        doorSound.Play();

                    position += 0.2f * Time.deltaTime;
                    if (position >= 1)
                    {
                        position = 1;
                        doorSound.Stop();
                        doorSound.PlayOneShot(doorStop);
                    }
                    transform.position = Vector3.Lerp(open, close, position);
                }
                else if (direction < 0 && position > 0)
                {
                    if (!doorSound.isPlaying)
                        doorSound.Play();

                    position -= 0.2f * Time.deltaTime;
                    if (position <= 0)
                    {
                        position = 0;
                        doorSound.Stop();
                        doorSound.PlayOneShot(doorStop);
                    }
                    transform.position = Vector3.Lerp(open, close, position);
                }
            }
        }
    }
}
