using UnityEngine;


namespace SigmaEditorViewPlugin
{
    internal static class EditorShadows
    {
        static Quaternion lightDirection;

        internal static void Update()
        {
            Debug.Log("EditorShadows.Update", "RenderSettings.sun = " + RenderSettings.sun);

            lightDirection = RenderSettings.sun.transform.rotation;

            Debug.Log("EditorShadows.Update", "lightDirection = " + lightDirection);
        }

        internal static void Apply(EditorFacility editor)
        {
            Debug.Log("EditorTerrain.Apply", "editor = " + editor);

            GameObject ksc = GameObject.Find("SpaceCenter");
            Debug.Log("EditorShadows.Apply", "ksc = " + ksc);

            if (ksc != null)
            {
                ksc.SetLayerRecursive(0);
            }

            GameObject shadowsObject = GameObject.Find("Shadow Light") ?? GameObject.Find("Realtime_Shadow Light");
            Debug.Log("EditorShadows.Apply", "shadowsObject = " + shadowsObject);

            if (shadowsObject != null)
            {
                shadowsObject.transform.rotation = EditorView.Rotation * lightDirection;
                shadowsObject.AddOrGetComponent<LightTracker>();

                //shadowsObject.GetComponent<Light>().cullingMask = 1 << 0 | 1 << 15;
            }

            GameObject shadowPlane = GameObject.Find("ShadowPlane");
            Debug.Log("EditorShadows.Apply", "shadowPlane = " + shadowPlane);

            if (shadowPlane != null)
            {
                shadowPlane.transform.localScale *= 100;
            }
        }
    }
}
