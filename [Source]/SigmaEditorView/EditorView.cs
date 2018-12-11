using UnityEngine;


namespace SigmaEditorViewPlugin
{
    static class EditorView
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
            EditorTerrain.Update();
        }

        internal static void Apply(EditorFacility editor)
        {
            Debug.Log("EditorView.Apply", "editor = " + editor);

            // SkyBox
            EditorSky.Apply(editor);
            EditorFlares.Apply(editor);
            EditorColliders.Apply(editor);

            // Terrain
            AmbientSettings.Apply(editor);
            EditorTerrain.Apply(editor);
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
    }
}
