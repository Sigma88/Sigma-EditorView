using UnityEngine;


namespace SigmaEditorViewPlugin
{
    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    internal class SettingsLoader : MonoBehaviour
    {
        void Start()
        {
            Debug.Log("SettingsLoader.Start");

            // User Settings
            ConfigNode settings = UserSettings.ConfigNode;
        }
    }

    internal static class Settings
    {
        internal static int size = 1024;
    }
}
