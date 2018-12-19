using UnityEngine;


namespace SigmaEditorViewPlugin
{
    internal static class EditorLight
    {
        internal static void Apply(EditorFacility editor)
        {
            Debug.Log("EditorLight.Apply", "editor = " + editor);

            // Fix Light
            GameObject lightObject = GameObject.Find("SpotlightSun") ?? GameObject.Find("ExteriorSun") ?? GameObject.Find("Realtime_ExteriorSun");
            Debug.Log("EditorLight.Apply", "lightObject = " + lightObject);

            if (lightObject != null)
            {
                Light light = lightObject.GetComponent<Light>();
                Debug.Log("EditorLight.Apply", "light = " + light);

                if (light != null)
                {
                    light.range = 3500;
                    light.intensity = 5;
                    light.spotAngle = 89;
                    light.cullingMask = 1 << 0 | 1 << 15;

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
        }
    }
}
