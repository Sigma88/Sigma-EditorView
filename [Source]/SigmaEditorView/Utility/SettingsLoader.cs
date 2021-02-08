using System.Linq;
using UnityEngine;


namespace SigmaEditorViewPlugin
{
    internal static class Settings
    {
        internal static int size = 1024;
        internal static bool editorSunFlares = true;
        internal static bool toggleLights = true;
        internal static bool toggleDoors = true;
        internal static bool closeDoors = false;
        internal static Texture doorsTexture;
        internal static Texture doorsBump;
        internal static Vector2 doorsTexScale = new Vector2(2, 3);
        internal static float doorsGloss = 0.4f;
        internal static AudioClip doorsMoveSound;
        internal static AudioClip doorsStopSound;
        internal static Texture2D cargoBtnTexture;
    }

    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    internal class SettingsLoader : MonoBehaviour
    {
        void Start()
        {
            Debug.Log("SettingsLoader.Start");

            // User Settings
            ConfigNode settings = UserSettings.ConfigNode;

            if (settings != null)
            {
                if (!int.TryParse(settings.GetValue("skyboxSize"), out Settings.size))
                {
                    Settings.size = 1024;
                }

                if (!bool.TryParse(settings.GetValue("editorSunFlares"), out Settings.editorSunFlares))
                {
                    Settings.editorSunFlares = true;
                }

                if (!bool.TryParse(settings.GetValue("toggleLights"), out Settings.toggleLights))
                {
                    Settings.toggleLights = true;
                }

                if (!bool.TryParse(settings.GetValue("toggleDoors"), out Settings.toggleDoors))
                {
                    Settings.toggleDoors = true;
                }

                bool.TryParse(settings.GetValue("closeDoors"), out Settings.closeDoors);

                Settings.doorsTexture = Resources.FindObjectsOfTypeAll<Texture>().FirstOrDefault(t => t.name == settings.GetValue("doorTex")) ?? Resources.FindObjectsOfTypeAll<Texture>().FirstOrDefault(t => t.name == "NissenHut_d");

                Settings.doorsBump = Resources.FindObjectsOfTypeAll<Texture>().FirstOrDefault(t => t.name == settings.GetValue("doorBump")) ?? Resources.FindObjectsOfTypeAll<Texture>().FirstOrDefault(t => t.name == "NissenHut_n");

                if (!settings.GetValue("doorTexScale").TryParse(out Settings.doorsTexScale))
                {
                    Settings.doorsTexScale = new Vector2(2, 3);
                }

                if (!float.TryParse(settings.GetValue("doorGloss"), out Settings.doorsGloss))
                {
                    Settings.doorsGloss = 0.4f;
                }

                Settings.doorsMoveSound = Resources.FindObjectsOfTypeAll<AudioClip>().FirstOrDefault(c => c.name == settings.GetValue("doorsMoveSound"));

                Settings.doorsStopSound = Resources.FindObjectsOfTypeAll<AudioClip>().FirstOrDefault(c => c.name == settings.GetValue("doorsStopSound"));

                Settings.cargoBtnTexture = Resources.FindObjectsOfTypeAll<Texture2D>().FirstOrDefault(t => t.name == settings.GetValue("cargoBtnTexture"));
            }
        }
    }
}
