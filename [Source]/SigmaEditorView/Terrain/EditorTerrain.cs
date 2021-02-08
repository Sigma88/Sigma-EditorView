using UnityEngine;


namespace SigmaEditorViewPlugin
{
    internal static class EditorTerrain
    {
        internal static void Apply(EditorFacility editor)
        {
            Debug.Log("EditorTerrain.Apply", "editor = " + editor);

            // Fix Ground
            GameObject groundObject = GameObject.Find("ksc_terrain");
            Debug.Log("EditorTerrain.Apply", "groundObject = " + groundObject);

            if (groundObject != null)
            {
                //groundObject.layer = 0;

                if (editor == EditorFacility.SPH)
                {
                    groundObject.transform.position += Vector3.down * 2.5f;
                }

                MeshCollider groundCollider = groundObject.AddOrGetComponent<MeshCollider>();
                Debug.Log("EditorTerrain.Apply", "groundCollider = " + groundCollider);
            }
        }
    }
}
