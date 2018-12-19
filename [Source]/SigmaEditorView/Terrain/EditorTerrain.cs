using UnityEngine;


namespace SigmaEditorViewPlugin
{
    internal static class EditorTerrain
    {
        internal static void Apply(EditorFacility editor)
        {
            Debug.Log("EditorTerrain.Apply", "editor = " + editor);

            // Fix Light
            GameObject lightObject = GameObject.Find("SpotlightSun") ?? GameObject.Find("ExteriorSun") ?? GameObject.Find("Realtime_ExteriorSun");
            Debug.Log("EditorTerrain.Apply", "lightObject = " + lightObject);

            if (lightObject != null)
            {
                Light light = lightObject.GetComponent<Light>();
                Debug.Log("EditorTerrain.Apply", "light = " + light);

                if (light != null)
                {
                    light.range = 3500;
                    light.intensity = 5;
                    light.spotAngle = 89;
                    if (editor == EditorFacility.SPH)
                    {
                        light.transform.position = new Vector3(0, 1800, 400);
                        light.transform.eulerAngles = new Vector3(61, 0, 0);
                    }
                    else
                    {
                        light.transform.position = new Vector3(400, 1800, 0);
                        light.transform.eulerAngles = new Vector3(61, 90, 0);
                    }
                }
            }

            // Fix Ground
            GameObject groundObject = GameObject.Find("ksc_terrain");
            Debug.Log("EditorTerrain.Apply", "groundObject = " + groundObject);

            if (groundObject != null)
            {
                groundObject.layer = 0;
                if (editor == EditorFacility.SPH)
                    groundObject.transform.position += Vector3.down * 2.5f;

                MeshCollider groundCollider = groundObject.AddOrGetComponent<MeshCollider>();
                Debug.Log("EditorTerrain.Apply", "groundCollider = " + groundCollider);
            }
        }
    }
}
