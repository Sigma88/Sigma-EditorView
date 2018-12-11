using UnityEngine;


namespace SigmaEditorViewPlugin
{
    internal static class AmbientSettings
    {
        static bool fog = false;

        internal static void Update()
        {
            Debug.Log("AmbientSettings.Update", "RenderSettings.sun = " + RenderSettings.sun);

            fog = RenderSettings.sun.name == "Scaledspace SunLight";

            Debug.Log("AmbientSettings.Update", "fog = " + fog);
        }

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
            RenderSettings.fog = fog;
            RenderSettings.fogColor = Color.black;
            Debug.Log("AmbientSettings.Apply", "RenderSettings.fogColor = " + RenderSettings.fogColor);
        }
    }
}
