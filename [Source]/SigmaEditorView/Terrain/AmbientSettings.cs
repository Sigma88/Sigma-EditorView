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
            Debug.Log("AmbientSettings.Apply", "Main Camera = " + EditorCamera.Instance.cam.farClipPlane);
            Debug.Log("AmbientSettings.Apply", "sceneryCam = " + sceneryCam);

            EditorCamera.Instance.cam.farClipPlane = sceneryCam.farClipPlane = 100000;
            QualitySettings.shadowDistance = 2000;
            QualitySettings.shadowCascade2Split = 0.01f;
            QualitySettings.shadowCascade4Split = new Vector3(0.005f, 0.01f, 0.05f);

            // Fix the fog
            RenderSettings.fog = false;
        }
    }
}
