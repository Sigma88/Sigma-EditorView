using UnityEngine;


namespace SigmaEditorViewPlugin
{
    internal static class EditorTerrain
    {
        static Material groundMaterial;

        internal static void Update()
        {
            GameObject ksc = SpaceCenter.Instance?.SpaceCenterTransform?.parent?.gameObject;
            Debug.Log("EditorTerrain.Update", "ksc = " + ksc);

            Material material = ksc?.GetChild("VAB_lev2_groundPlane")?.GetComponentInChildren<Renderer>(true)?.material;
            if (material != null)
            {
                Debug.Log("EditorTerrain.Update", "VABlvl2 - Ground material = " + material);
            }
            else
            {
                material = ksc?.GetChild("vab_exterior_groundPlane")?.GetComponentInChildren<Renderer>(true)?.materials?[1];
                if (material != null)
                {
                    Debug.Log("EditorTerrain.Update", "VABlvl3 - Ground material = " + material);
                }
                else
                {
                    material = ksc?.GetChild("model_vab_exterior_ground_v46n")?.GetComponentInChildren<Renderer>(true)?.materials?[2];
                    if (material != null)
                    {
                        Debug.Log("EditorTerrain.Update", "VABmodern - Ground material = " + material);
                    }
                    else
                    {
                        Debug.Log("EditorTerrain.Update", "Unable to find Ground material");
                        return;
                    }
                }
            }

            // If the material is valid
            groundMaterial = material;
        }

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
                Debug.Log("EditorTerrain.Apply", "Set groundObject.layer = " + groundObject.layer);

                MeshCollider groundCollider = groundObject.AddOrGetComponent<MeshCollider>();
                Debug.Log("EditorTerrain.Apply", "groundCollider = " + groundCollider);

                Renderer groundRenderer = groundObject.GetComponentInChildren<Renderer>(true);
                Debug.Log("EditorTerrain.Apply", "groundRenderer = " + groundRenderer);

                if (groundRenderer != null)
                {
                    Debug.Log("EditorTerrain.Apply", "groundMaterial = " + groundMaterial);
                    if (groundMaterial != null)
                    {
                        groundRenderer.material = groundMaterial;
                        Debug.Log("EditorTerrain.Apply", "groundRenderer.material = " + groundRenderer.material);
                    }
                }
            }
        }
    }
}
