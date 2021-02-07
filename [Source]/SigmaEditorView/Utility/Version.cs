﻿using UnityEngine;


namespace SigmaEditorViewPlugin
{
    [KSPAddon(KSPAddon.Startup.Instantly, true)]
    internal class Version : MonoBehaviour
    {
        public static readonly System.Version number = new System.Version("0.5.0");

        void Awake()
        {
            UnityEngine.Debug.Log("[SigmaLog] Version Check:   Sigma EditorView v" + number);
        }
    }
}
