using System.Linq;
using UnityEngine;


namespace SigmaEditorViewPlugin
{
    internal static class Settings
    {
        internal static int size = 1024;
        internal static bool closeDoors = false;
        internal static bool toggleDoors = false; // change to 'true' when the feature is completely fixed
        internal static bool editorSunFlares = true;
        internal static Texture doorTex;
        internal static Texture doorBump;
        internal static Vector2 doorTexScale = new Vector2(2, 3);
        internal static float doorGloss = 0.4f;
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

                bool.TryParse(settings.GetValue("closeDoors"), out Settings.closeDoors);

                if (!bool.TryParse(settings.GetValue("toggleDoors"), out Settings.toggleDoors))
                {
                    Settings.toggleDoors = false; // change to 'true' when the feature is completely fixed
                }

                if (!bool.TryParse(settings.GetValue("doorsSound"), out Settings.doorsSound))
                {
                    Settings.doorsSound = true;
                }

                Settings.doorTex = Resources.FindObjectsOfTypeAll<Texture>().FirstOrDefault(t => t.name == settings.GetValue("doorTex")) ?? Resources.FindObjectsOfTypeAll<Texture>().FirstOrDefault(t => t.name == "NissenHut_d");

                Settings.doorBump = Resources.FindObjectsOfTypeAll<Texture>().FirstOrDefault(t => t.name == settings.GetValue("doorBump")) ?? Resources.FindObjectsOfTypeAll<Texture>().FirstOrDefault(t => t.name == "NissenHut_n");

                if (!settings.GetValue("doorTexScale").TryParse(out Settings.doorTexScale))
                {
                    Settings.doorTexScale = new Vector2(2, 3);
                }

                if (!float.TryParse(settings.GetValue("doorGloss"), out Settings.doorGloss))
                {
                    Settings.doorGloss = 0.4f;
                }
            }
        }
    }
}
