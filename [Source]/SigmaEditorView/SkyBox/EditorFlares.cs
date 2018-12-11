using UnityEngine;


namespace SigmaEditorViewPlugin
{
    static class EditorFlares
    {
        static LensFlare[] lensFlares;
        static Quaternion[] rotations;

        internal static void Update()
        {
            Debug.Log("EditorFlares.Update");

            DestroyAll();
            Duplicate(Resources.FindObjectsOfTypeAll<SunFlare>());
        }

        internal static void Apply(EditorFacility editor)
        {
            Debug.Log("EditorFlares.Apply", "editor = " + editor);

            int? n = lensFlares?.Length;

            Debug.Log("EditorFlares.Apply", "lensFlares found = " + n);
            Debug.Log("EditorFlares.Apply", "rotations found = " + rotations?.Length);

            for (int i = 0; i < n; i++)
            {
                if (lensFlares[i]?.gameObject != null)
                {
                    lensFlares[i].transform.rotation = EditorView.Rotation * rotations[i];
                    lensFlares[i].gameObject.SetActive(true);
                }
                Debug.Log("EditorFlares.Apply", "lensFlares[" + i + "] = " + lensFlares[i]);
                Debug.Log("EditorFlares.Apply", "rotations[" + i + "] = " + rotations[i]);
            }
        }

        internal static void DisableAll()
        {
            int? n = lensFlares?.Length;

            Debug.Log("EditorFlares.DisableAll", "sunFlares found = " + n);

            for (int i = 0; i < n; i++)
            {
                try { lensFlares[i].gameObject.SetActive(false); } catch { }
            }
        }

        static void DestroyAll()
        {
            int? n = lensFlares?.Length;

            Debug.Log("EditorFlares.DestroyAll", "sunFlares found = " + n);

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

                    // Disable the clone
                    lensFlares[i].gameObject.SetActive(false);

                    // Remove SunFlare component from the clone
                    Object.DestroyImmediate(lensFlares[i].GetComponent<SunFlare>());

                    System.Delegate[] newDelegates = Camera.onPreCull.GetInvocationList();        // KopernicusSunFlare compatibility
                    if (newDelegates.Length == oldDelegates + 1)                                  // KopernicusSunFlare compatibility
                        Camera.onPreCull -= (Camera.CameraCallback)newDelegates[oldDelegates];    // KopernicusSunFlare compatibility

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
