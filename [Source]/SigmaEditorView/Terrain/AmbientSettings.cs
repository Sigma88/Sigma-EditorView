using UnityEngine;


namespace SigmaEditorViewPlugin
{
    internal static class AmbientSettings
    {
        internal static void Apply(EditorFacility editor)
        {
            Debug.Log("AmbientSettings.Apply", "editor = " + editor);

            // Fix the sceneryCam
            Camera sceneryCam = EditorCamera.Instance?.gameObject?.GetChild("sceneryCam")?.GetComponent<Camera>();
            Debug.Log("AmbientSettings.Apply", "sceneryCam = " + sceneryCam);

            if (sceneryCam != null)
            {
                sceneryCam.nearClipPlane = 0.45f;
                sceneryCam.farClipPlane = 2000;
            }

            // Fix the fog
            RenderSettings.fog = false;
        }
    }
}
