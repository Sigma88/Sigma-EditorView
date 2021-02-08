using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace SigmaEditorViewPlugin
{
    internal static class EditorDoors
    {
        internal static GameObject shadeL;
        internal static GameObject doorL;
        internal static GameObject shadeR;
        internal static GameObject doorR;

        static Dictionary<EditorFacility, bool> doorState = null;

        internal static void Apply(EditorFacility editor)
        {
            Debug.Log("EditorDoors.Apply", "editor = " + editor);

            // Renderer
            Debug.Log("EditorDoors.Apply", "doorL = " + doorL + ", shadeL = " + shadeL);
            Debug.Log("EditorDoors.Apply", "doorR = " + doorR + ", shadeR = " + shadeR);

            if (doorState == null)
            {
                doorState = new Dictionary<EditorFacility, bool>() { { EditorFacility.SPH, !Settings.closeDoors }, { EditorFacility.VAB, !Settings.closeDoors } };
            }

            if (Settings.toggleDoors)
            {
                EditorButtons.doorsOnBtn.gameObject.SetActive(doorState[editor]);
                EditorButtons.doorsOffBtn.gameObject.SetActive(!doorState[editor]);
            }
            else
            {
                doorL.SetActive(Settings.closeDoors);
                shadeL.SetActive(Settings.closeDoors);
                doorR.SetActive(Settings.closeDoors);
                shadeR.SetActive(Settings.closeDoors);
            }
        }

        internal class Mover : MonoBehaviour
        {
            internal Vector3 open = Vector3.one;
            internal Vector3 close = Vector3.one;
            internal int direction = 1;  // -1 = open, 1 = close
            internal float position = 0; //  0 = open, 1 = close
            internal AudioSource doorSound;
            internal AudioClip doorStop;

            internal void Reverse()
            {
                direction *= -1;
                Stop(false);
            }

            void Start()
            {
                direction = Settings.closeDoors ? 1 : -1;
                position = Settings.closeDoors ? 1 : 0;
                EditorButtons.doorsOnBtn.onClick.AddListener(Reverse);
                EditorButtons.doorsOffBtn.onClick.AddListener(Reverse);
                close = transform.position;
                transform.position = Settings.closeDoors ? close : open;

                if (Settings.doorsMoveSound)
                {
                    doorSound = gameObject.AddOrGetComponent<AudioSource>();
                    doorSound.clip = Settings.doorsMoveSound;
                    doorSound.loop = true;
                    doorSound.volume = 0.2f;
                }

                doorStop = Settings.doorsStopSound;
            }

            void Update()
            {
                if (direction > 0 && position < 1)
                {
                    Play();
                    position += 0.2f * Time.deltaTime;
                    if (position >= 1)
                    {
                        position = 1;
                        Stop();
                    }
                    transform.position = Vector3.Lerp(open, close, position);
                }
                else if (direction < 0 && position > 0)
                {
                    Play();
                    position -= 0.2f * Time.deltaTime;
                    if (position <= 0)
                    {
                        position = 0;
                        Stop();
                    }
                    transform.position = Vector3.Lerp(open, close, position);
                }
            }

            void Play()
            {
                if (doorSound && !doorSound.isPlaying)
                {
                    doorSound.Play();
                }
            }

            void Stop(bool end = true)
            {
                if (doorSound)
                {
                    doorSound.Stop();

                    if (end && doorStop)
                    {
                        doorSound.PlayOneShot(doorStop);
                    }
                }
            }
        }
    }
}
