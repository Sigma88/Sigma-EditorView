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

            groundMaterial = ksc?.GetChild("VAB_lev2_groundPlane")?.GetComponentInChildren<Renderer>(true)?.material;
            if (groundMaterial != null)
            {
                Debug.Log("EditorTerrain.Update", "VABlvl2 - groundMaterial = " + groundMaterial);
            }
            else
            {
                groundMaterial = ksc?.GetChild("vab_exterior_groundPlane")?.GetComponentInChildren<Renderer>(true)?.materials?[1];
                if (groundMaterial != null)
                {
                    Debug.Log("EditorTerrain.Update", "VABlvl3 - groundMaterial = " + groundMaterial);
                }
                else
                {
                    groundMaterial = ksc?.GetChild("model_vab_exterior_ground_v46n")?.GetComponentInChildren<Renderer>(true)?.materials?[2];
                    Debug.Log("EditorTerrain.Update", "VABmodern - groundMaterial = " + groundMaterial);
                }
            }
        }

        internal static void Apply(EditorFacility editor)
        {
            Debug.Log("EditorTerrain.Apply", "editor = " + editor);

            GameObject groundObject = GameObject.Find("ksc_terrain");
            Debug.Log("EditorTerrain.Apply", "groundObject = " + groundObject);

            if (groundObject != null)
            {
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
