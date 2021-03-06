﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;


namespace SigmaEditorViewPlugin
{
    internal class BackgroundCamera : MonoBehaviour
    {
        internal Camera backgroundCamera;
        Camera foregroundCamera;

        internal void Awake()
        {
            foregroundCamera = GetComponent<Camera>();
        }

        internal void OnPreCull()
        {
            backgroundCamera.CopyFrom(foregroundCamera);
            backgroundCamera.enabled = false;
            backgroundCamera.worldToCameraMatrix = Matrix4x4.TRS(Vector3.zero, foregroundCamera.worldToCameraMatrix.rotation, Vector3.one);
            backgroundCamera.backgroundColor = Color.black;
            backgroundCamera.clearFlags = CameraClearFlags.SolidColor;
            backgroundCamera.targetTexture = foregroundCamera.targetTexture;
            backgroundCamera.cullingMask = 1 << 18;
            backgroundCamera.nearClipPlane = 0.3f;
            backgroundCamera.farClipPlane = EditorSky.maxdistance;
            backgroundCamera.transform.position = Vector3.zero;
            backgroundCamera.Render();
            backgroundCamera.clearFlags = CameraClearFlags.Depth;
            backgroundCamera.cullingMask = 1 << 9 | 1 << 10;
            backgroundCamera.Render();
        }
    }

    internal static class EditorSky
    {
        static RenderTexture cubemap;
        static Vector3 vabOffset = new Vector3(110, 50, -50);
        static Vector3 vabRealOffset;

        internal static void Update()
        {
            Debug.Log("EditorSky.Update");

            Transform KSC = SpaceCenter.Instance.SpaceCenterTransform;
            vabRealOffset = Vector3.Scale(vabOffset, KSC.localScale);
            vabRealOffset = KSC.right.normalized * vabRealOffset.x + KSC.up.normalized * vabRealOffset.y + KSC.forward.normalized * vabRealOffset.z;

            GameObject foreground = new GameObject("SigmaEditorView Foreground Camera");
            Camera foregroundCamera = foreground.AddOrGetComponent<Camera>();

            GameObject background = new GameObject("SigmaEditorView Background Camera");
            Camera backgroundCamera = background.AddOrGetComponent<Camera>();

            cubemap = cubemap ?? new RenderTexture(Settings.size, Settings.size, 0) { name = "EditorSky.ground", dimension = TextureDimension.Cube };

            foregroundCamera.enabled = false;
            foregroundCamera.nearClipPlane = 2000;
            foregroundCamera.farClipPlane = 200000;
            foregroundCamera.cullingMask = 1 << 15;
            foregroundCamera.clearFlags = CameraClearFlags.Depth;
            foregroundCamera.gameObject.AddOrGetComponent<BackgroundCamera>().backgroundCamera = backgroundCamera;
            foregroundCamera.transform.position = KSC.position + vabRealOffset;
            foregroundCamera.RenderToCubemap(cubemap);

            // CleanUp
            Object.DestroyImmediate(foreground);
            Object.DestroyImmediate(background);
        }

        internal static void Apply(EditorFacility editor)
        {
            Debug.Log("EditorSky.Apply", "editor = " + editor);

            RenderSettings.skybox.shader = ShaderLoader.shader;

            RenderSettings.skybox.SetTexture("_CubeMap", cubemap);

            RenderSettings.skybox.SetMatrix("_Rotation", EditorView.GetMatrix(editor));
        }

        static float? _maxdistance;

        internal static float maxdistance
        {
            get
            {
                if (_maxdistance == null)
                {
                    float max = 0;
                    GameObject kerbin = FlightGlobals.GetHomeBody().scaledBody;

                    List<CelestialBody> list = FlightGlobals.Bodies;
                    int? n = list?.Count;

                    for (int i = 0; i < n; i++)
                    {
                        try { max = Mathf.Max(max, (list[i].scaledBody.transform.position - kerbin.transform.position).magnitude); }
                        catch { }
                    }

                    _maxdistance = max * 2;
                }

                return _maxdistance ?? 0;
            }
        }
    }
}
