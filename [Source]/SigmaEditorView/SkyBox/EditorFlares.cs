using UnityEngine;
using Object = UnityEngine.Object;


namespace SigmaEditorViewPlugin
{
    static class EditorFlares
    {
        static LensFlare[] lensFlares;
        static Quaternion[] rotations;

        internal static void Update()
        {
            DestroyAll();
            Duplicate(Resources.FindObjectsOfTypeAll<SunFlare>());
        }

        internal static void Apply(EditorFacility editor)
        {
            int? n = lensFlares?.Length;

            for (int i = 0; i < n; i++)
            {
                if (lensFlares[i]?.gameObject != null)
                {
                    if (editor == EditorFacility.SPH)
                        lensFlares[i].transform.rotation = EditorView.SPHrot * rotations[i];
                    else
                        lensFlares[i].transform.rotation = EditorView.VABrot * rotations[i];

                    lensFlares[i].gameObject.SetActive(true);
                }
                Debug.Log("EditorFlares.Apply", "lensFlares[" + i + "] = " + lensFlares[i]);
                Debug.Log("EditorFlares.Apply", "rotations[" + i + "] = " + rotations[i]);
            }
        }

        internal static void DestroyAll()
        {
            int? n = lensFlares?.Length;

            for (int i = 0; i < n; i++)
            {
                try { Object.DestroyImmediate(lensFlares[i].gameObject); } catch { }
            }
        }

        static void Duplicate(SunFlare[] sunFlares)
        {
            int? n = sunFlares?.Length;
            Debug.Log("EditorFlares.Duplicate", "sunFlares found = " + (n ?? 0));
            lensFlares = new LensFlare[n ?? 0];
            rotations = new Quaternion[n ?? 0];

            for (int i = 0; i < n; i++)
            {
                if (sunFlares[i]?.enabled == true)
                {
                    // Disable the original SunFlare component
                    sunFlares[i].enabled = false;

                    // Instantiate LensFlare
                    int oldDelegates = Camera.onPreCull.GetInvocationList().Length;               // KopernicusSunFlare compatibility

                    lensFlares[i] = Object.Instantiate(sunFlares[i].sunFlare);

                    System.Delegate[] newDelegates = Camera.onPreCull.GetInvocationList();        // KopernicusSunFlare compatibility
                    if (newDelegates.Length == oldDelegates + 1)                                  // KopernicusSunFlare compatibility
                        Camera.onPreCull -= (Camera.CameraCallback)newDelegates[oldDelegates];    // KopernicusSunFlare compatibility

                    // Disable the clone
                    lensFlares[i].gameObject.SetActive(false);

                    // Remove SunFlare component from the clone
                    Object.DestroyImmediate(lensFlares[i].GetComponent<SunFlare>());

                    // Reposition the GameObject
                    lensFlares[i].transform.SetParent(sunFlares[i].transform.parent);
                    lensFlares[i].transform.position = sunFlares[i].transform.position;

                    // Backup LensFlare rotation
                    rotations[i] = sunFlares[i].transform.rotation;

                    // Add FlareCamera component
                    lensFlares[i].gameObject.AddOrGetComponent<FlareCamera>();

                    // Re-enable the original SunFlare component
                    sunFlares[i].enabled = true;
                }
            }
        }
    }
}
