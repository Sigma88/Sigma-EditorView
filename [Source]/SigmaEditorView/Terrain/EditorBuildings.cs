using System.Collections.Generic;
using UnityEngine;


namespace SigmaEditorViewPlugin
{
    internal static class EditorBuildings
    {
        static Vector3 SPHposition;

        static GameObject[] buildings;
        static Vector3[] positions;
        static Quaternion[] rotations;

        internal static void Update()
        {
            Debug.Log("EditorBuildings.Update");

            // Destroy old clones
            DestroyAll();

            // Find the homeworld
            CelestialBody homeBody = FlightGlobals.GetHomeBody();
            Debug.Log("EditorBuildings.Update", "homeBody = " + homeBody);

            // Get the pqsController
            PQS pqs = homeBody?.pqsController;
            Debug.Log("EditorBuildings.Update", "pqsController = " + pqs);

            // Filter and Duplicate PQSMods
            Duplicate(pqs?.GetComponentsInChildren<PQSMod>());
        }

        internal static void Apply(EditorFacility editor)
        {
            Debug.Log("EditorBuildings.Apply", "editor = " + editor);

            int? n = buildings?.Length;

            Debug.Log("EditorBuildings.Apply", "buildings found = " + n);
            Debug.Log("EditorBuildings.Apply", "positions found = " + positions?.Length);
            Debug.Log("EditorBuildings.Apply", "rotations found = " + rotations?.Length);

            Quaternion editorRotation = EditorView.Rotation;

            for (int i = 0; i < n; i++)
            {
                GameObject building = buildings[i];
                Debug.Log("EditorBuildings.Apply", "buildings[" + i + "] = " + building);

                if (building != null)
                {
                    Vector3 position = positions[i];
                    if (editor == EditorFacility.SPH)
                        position -= SPHposition;
                    Debug.Log("EditorBuildings.Apply", "positions[" + i + "] = " + position);

                    Quaternion rotation = rotations[i];
                    Debug.Log("EditorBuildings.Apply", "rotations[" + i + "] = " + rotation);

                    building.transform.position = editorRotation * position;
                    building.transform.rotation = editorRotation * rotation;
                    building.SetActive(true);
                }
            }
        }

        internal static void DisableAll()
        {
            int? n = buildings?.Length;

            Debug.Log("EditorBuildings.DisableAll", "buildings found = " + n);

            for (int i = 0; i < n; i++)
            {
                try { buildings[i].SetActive(false); } catch { }
            }
        }

        static void DestroyAll()
        {
            int? n = buildings?.Length;

            Debug.Log("EditorBuildings.DestroyAll", "buildings found = " + n);

            for (int i = 0; i < n; i++)
            {
                try { Object.DestroyImmediate(buildings[i]); } catch { }
            }
        }

        static void Duplicate(PQSMod[] pqsMods)
        {
            // Count pqsMods
            int? n = pqsMods?.Length;
            Debug.Log("EditorBuildings.Duplicate", "pqsMods found = " + (n ?? 0));

            if (!(n > 0)) return;

            // Find KSC
            PQSCity ksc = SpaceCenter.Instance?.SpaceCenterTransform?.parent?.GetComponentInChildren<PQSCity>();
            if (ksc == null) return;

            // VAB position
            Vector3 VABposition = ksc.transform.position +
                                  ksc.transform.right.normalized * -186.3f +
                                  ksc.transform.up.normalized * +24.8f;
            // SPH position
            SPHposition = ksc.transform.right.normalized * -157.6f +
                          ksc.transform.up.normalized * -0.1f +
                          ksc.transform.forward.normalized * 263.5f;

            // Find Radius
            float radius = (float)FlightGlobals.GetHomeBody().Radius;

            // Initialize Lists
            List<GameObject> objects = new List<GameObject>();
            List<Vector3> vectors = new List<Vector3>();
            List<Quaternion> quaternions = new List<Quaternion>();

            for (int i = 0; i < n; i++)
            {
                PQSMod pqsMod = pqsMods[i];

                if (pqsMod.GetType() == typeof(PQSCity) || pqsMod.GetType() == typeof(PQSCity2))
                {
                    // Calculate position relative to KSC
                    Vector3 relativePosition = pqsMod.transform.position - VABposition;

                    // Check distance from KSC
                    double distance = relativePosition.magnitude;

                    // If the PQSMod is within the chosen range
                    if (pqsMod != ksc && distance < 2000)
                    {
                        Debug.Log("EditorBuildings.Duplicate", "pqsMod = " + pqsMod + ", distance = " + distance);

                        // Backup the original enabled state
                        bool backup = pqsMod.enabled;

                        // Disable the original PQSMod component
                        pqsMod.enabled = false;

                        // Instantiate GameObject
                        GameObject clone = Object.Instantiate(pqsMod.gameObject);
                        Debug.Log("EditorBuildings.Duplicate", "clone = " + clone);

                        // Disable the clone
                        clone.SetActive(false);

                        // Change to Layer 0
                        clone.SetLayerRecursive(0);

                        // Remove PQSMod component from the clone
                        Object.DestroyImmediate(clone.GetComponent<PQSMod>());

                        // Don't destroy object
                        Object.DontDestroyOnLoad(clone);

                        // Add clone to list
                        objects.Add(clone);
                        // Add clone position to list
                        vectors.Add(relativePosition);
                        // Add clone rotation to list
                        quaternions.Add(pqsMod.transform.rotation);

                        // Restore the original PQSMod enabled state
                        pqsMod.enabled = backup;
                    }
                }
            }

            // Store lists to arrays
            buildings = objects.ToArray();
            Debug.Log("EditorBuildings.Duplicate", "buildings count = " + buildings?.Length);

            positions = vectors.ToArray();
            Debug.Log("EditorBuildings.Duplicate", "positions count = " + positions?.Length);

            rotations = quaternions.ToArray();
            Debug.Log("EditorBuildings.Duplicate", "rotations count = " + rotations?.Length);

        }
    }
}
