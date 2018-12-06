using UnityEngine;


namespace SigmaEditorViewPlugin
{
    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    internal class SettingsLoader : MonoBehaviour
    {
        void Start()
        {
            // User Settings
            ConfigNode[] InfoNodes = UserSettings.ConfigNode.GetNodes("EditorView");
        }
    }

    internal static class Settings
    {
        internal static int size = 1024;
    }
}
