﻿using UnityEngine;


namespace SigmaEditorViewPlugin
{
    internal static class EditorView
    {
        static Quaternion _VABrot;
        static Quaternion _SPHrot;

        internal static Quaternion VABrot
        {
            get { return _VABrot; }
            set { _VABrot = Quaternion.LookRotation(Vector3.right, Vector3.up) * Quaternion.Inverse(value); }
        }
        internal static Quaternion SPHrot
        {
            get { return _SPHrot; }
            set { _SPHrot = Quaternion.LookRotation(Vector3.forward, Vector3.up) * Quaternion.Inverse(value); }
        }
        internal static Quaternion Rotation
        {
            get
            {
                if (EditorDriver.editorFacility == EditorFacility.SPH)
                {
                    return SPHrot;
                }
                else
                {
                    return VABrot;
                }
            }
        }

        internal static void Update()
        {
            Debug.Log("EditorView.Update");

            Transform ksc = SpaceCenter.Instance.SpaceCenterTransform;
            VABrot = Quaternion.LookRotation(ksc.right, ksc.up);
            SPHrot = Quaternion.LookRotation(Quaternion.AngleAxis(-30, ksc.up) * ksc.right, ksc.up);

            // SkyBox
            EditorSky.Update();
            EditorFlares.Update();

            // Terrain
            EditorBuildings.Update();
            EditorShadows.Update();
        }

        internal static void Apply(EditorFacility editor)
        {
            Debug.Log("EditorView.Apply", "editor = " + editor);

            // SkyBox
            EditorSky.Apply(editor);
            EditorFlares.Apply(editor);
            EditorColliders.Apply(editor);

            // UI
            EditorButtons.Apply(editor);

            // Terrain
            AmbientSettings.Apply(editor);
            EditorTerrain.Apply(editor);
            EditorLight.Apply(editor);
            EditorDoors.Apply(editor);
            EditorShadows.Apply(editor);
            EditorBuildings.Apply(editor);

            // Scatterer Compatibility
            EditorCamera.Instance.cam.gameObject.AddOrGetComponent<EditorCameraTracker>();
        }

        internal static Matrix4x4 GetMatrix(EditorFacility editor)
        {
            if (editor == EditorFacility.SPH)
                return Matrix4x4.TRS(Vector3.zero, SPHrot, Vector3.one);
            else
                return Matrix4x4.TRS(Vector3.zero, VABrot, Vector3.one);
        }

        internal static void OnDestroy()
        {
            Debug.Log("EditorView.OnDestroy");
            EditorFlares.DisableAll();
            EditorBuildings.DisableAll();
        }

        class EditorCameraTracker : MonoBehaviour
        {
            void OnPreRender()
            {
                EditorFlaresScatterer.OnPreRender(EditorDriver.editorFacility);
            }

            void OnPostRender()
            {
                EditorFlaresScatterer.OnPostRender();
            }
        }
    }
}
