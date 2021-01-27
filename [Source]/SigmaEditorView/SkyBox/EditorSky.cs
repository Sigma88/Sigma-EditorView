using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;


namespace SigmaEditorViewPlugin
{
    class AtmosphereAndSkyBoxRenderer : MonoBehaviour
    {
        Camera skyBoxCamera;
        GameObject skyBoxCameraGO;

        Camera scaledCamera;
        GameObject scaledCameraGO;

        Camera groundCamera;

        public AtmosphereAndSkyBoxRenderer()
        {
        }

        public void Awake()
        {
            // Create a camera that will render skyBox, and another scaledSpace+atmo
            skyBoxCameraGO = new GameObject();
            skyBoxCamera = skyBoxCameraGO.AddComponent<Camera>();
            skyBoxCamera.enabled = false;

            scaledCameraGO = new GameObject();
            scaledCamera = scaledCameraGO.AddComponent<Camera>();
            scaledCamera.enabled = false;

            groundCamera = GetComponent<Camera>();
        }

        public void OnPreCull()
        {
            skyBoxCamera.CopyFrom(groundCamera);
            skyBoxCamera.enabled = false;
            skyBoxCamera.cullingMask = 1 << 18;
            skyBoxCamera.clearFlags = CameraClearFlags.SolidColor;
            skyBoxCamera.backgroundColor = Color.clear;

            skyBoxCamera.targetTexture = groundCamera.targetTexture;
            skyBoxCamera.Render();

            scaledCamera.CopyFrom(groundCamera);
            scaledCamera.enabled = false;
            scaledCamera.cullingMask = 1 << 9 | 1 << 10;
            scaledCamera.clearFlags = CameraClearFlags.Depth;

            scaledCamera.targetTexture = groundCamera.targetTexture;
            scaledCamera.Render();
        }

        public void OnDestroy()
        {
            skyBoxCamera.enabled = false;
            Component.Destroy(skyBoxCamera);
            UnityEngine.Object.Destroy(skyBoxCameraGO);

            scaledCamera.enabled = false;
            Component.Destroy(scaledCamera);
            UnityEngine.Object.Destroy(scaledCameraGO);
        }

    }

    static class EditorSky
    {
        static RenderTexture combined;
        static bool scatterer = AssemblyLoader.loadedAssemblies.FirstOrDefault(a => a.name == "scatterer") != null;

        internal static void Update()
        {
            Debug.Log("EditorSky.Update");

            // Create GameObject and Camera
            GameObject marker = new GameObject("SigmaEditorView Camera");
            Camera camera = marker.AddOrGetComponent<Camera>();
            camera.enabled = false;

            // Setup Camera
            camera.farClipPlane = maxdistance;

            // Ground
            camera.nearClipPlane = 2000;
            camera.farClipPlane = 200000;
            combined = combined ?? new RenderTexture(Settings.size, Settings.size, 0) { name = "EditorSky.ground", dimension = TextureDimension.Cube };
            if (!combined.IsCreated()) combined.Create();
            camera.transform.position = SpaceCenter.Instance.SpaceCenterTransform.position - SpaceCenter.Instance.SpaceCenterTransform.up.normalized * 22;
            camera.cullingMask = 1 << 15;
            camera.clearFlags = CameraClearFlags.Depth;

            camera.gameObject.AddComponent<AtmosphereAndSkyBoxRenderer>();

            camera.RenderToCubemap(combined);

            // CleanUp
            Object.DestroyImmediate(camera);
            Object.DestroyImmediate(marker);
        }

        internal static void Apply(EditorFacility editor)
        {
            Debug.Log("EditorSky.Apply", "editor = " + editor);

            RenderSettings.skybox.shader = ShaderLoader.shader;

            RenderSettings.skybox.SetTexture("_SkyBox", skybox);
            RenderSettings.skybox.SetTexture("_Scaled", scaled);
            RenderSettings.skybox.SetTexture("_Ground", ground);

            if (scatterer)
            {
                RenderSettings.skybox.SetFloat("_Scatterer", 1);
            }

            RenderSettings.skybox.SetMatrix("_Rotation", EditorView.GetMatrix(editor));
        }

        static float? _maxdistance;

        static float maxdistance
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
