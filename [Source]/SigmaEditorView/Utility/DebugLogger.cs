﻿using UnityEngine;


namespace SigmaEditorViewPlugin
{
    internal static class Debug
    {
        internal static bool debug = false;
        internal static string Tag = "[SigmaLog EV]";

        internal static void Log(string message)
        {
            if (debug)
            {
                UnityEngine.Debug.Log(Tag + ": " + message);
            }
        }

        internal static void Log(string Method, string message)
        {
            if (debug)
            {
                UnityEngine.Debug.Log(Tag + " " + Method + ": " + message);
            }
        }
    }

    [KSPAddon(KSPAddon.Startup.Instantly, true)]
    internal class DebugLoader : MonoBehaviour
    {
        void OnDestroy()
        {
            // Debug Spam
            if (bool.TryParse(UserSettings.ConfigNode?.GetValue("debug"), out bool debug) && debug) Debug.debug = true;
        }
    }

    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    internal class DebugWarning : MonoBehaviour
    {
        void Start()
        {
            if (Debug.debug)
            {
                PopupDialog.SpawnPopupDialog
                (
                    new Vector2(0.5f, 0.5f),
                    new Vector2(0.5f, 0.5f),
                    UserSettings.nodeName,
                    UserSettings.nodeName + " Warning",
                    "\n<color=#FF9231><b>Debug Spam is activated.</b>\n\n" +
                    "This feature will greatly affect performance:\n" +
                    "use it only for debugging purposes.</color>",
                    "OK",
                    true,
                    UISkinManager.GetSkin("MainMenuSkin")
                );
            }
        }
    }
}
