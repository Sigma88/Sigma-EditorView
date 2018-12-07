﻿using UnityEngine;


namespace SigmaEditorViewPlugin
{
    internal static class AmbientSettings
    {
        static Color fogColor;

        internal static void Update(Camera camera)
        {
            Debug.Log("AmbientSettings.Update", "camera = " + camera);

            // BackUp
            RenderTexture backupRT = RenderTexture.active;
            RenderTexture backupTex = camera.targetTexture;
            Vector3 backupPos = camera.transform.position;
            Quaternion backupRot = camera.transform.rotation;
            int backupMask = camera.cullingMask;
            float backupFOV = camera.fieldOfView;

            // New RenderTexture
            RenderTexture rt = new RenderTexture(1, 1, 0);
            RenderTexture.active = rt;
            camera.targetTexture = rt;

            // Setup Camera
            Transform ksc = SpaceCenter.Instance.SpaceCenterTransform;
            camera.transform.position = Vector3.zero;
            camera.transform.rotation = Quaternion.LookRotation(ksc.right, ksc.up);
            camera.cullingMask = 1 << 9;
            camera.fieldOfView = 0.05f;

            // Get Color
            camera.Render();
            Texture2D texture = new Texture2D(1, 1);
            texture.ReadPixels(new Rect(0, 0, 1, 1), 0, 0);
            fogColor = texture.GetPixel(0, 0);

            Debug.Log("AmbientSettings.Update", "fogColor = " + fogColor);

            // Restore
            RenderTexture.active = backupRT;
            camera.targetTexture = backupTex;
            camera.transform.position = backupPos;
            camera.transform.rotation = backupRot;
            camera.cullingMask = backupMask;
            camera.fieldOfView = backupFOV;

            // CleanUp
            Object.DestroyImmediate(rt);
            Object.DestroyImmediate(texture);
        }

        internal static void Apply(EditorFacility editor)
        {
            Debug.Log("AmbientSettings.Apply", "editor = " + editor);

            RenderSettings.fogColor = fogColor;
        }
    }
}
