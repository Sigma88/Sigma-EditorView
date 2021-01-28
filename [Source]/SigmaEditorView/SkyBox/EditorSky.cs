using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;


namespace SigmaEditorViewPlugin
{
    class AtmosphereAndSkyBoxRenderer : MonoBehaviour
    {
        Camera skyBoxCamera;
        GameObject skyBoxCameraGO;

        Camera scaledCamera;
        GameObject scaledCameraGO;

        Camera groundCamera;

        float maxdistance;

        public AtmosphereAndSkyBoxRenderer()
        {
        }

        public void Awake()
        {
            maxdistance = EditorSky.maxdistance;

            groundCamera = GetComponent<Camera>();

            skyBoxCameraGO = new GameObject();
            skyBoxCamera = skyBoxCameraGO.AddComponent<Camera>();
            skyBoxCamera.enabled = false;

            scaledCameraGO = new GameObject();
            scaledCamera = scaledCameraGO.AddComponent<Camera>();
            scaledCamera.enabled = false;
        }

        public void OnPreCull()
        {
            skyBoxCamera.CopyFrom(groundCamera);
            skyBoxCamera.enabled = false;
            skyBoxCamera.backgroundColor = Color.clear;
            skyBoxCamera.clearFlags = CameraClearFlags.SolidColor;
            skyBoxCamera.targetTexture = groundCamera.targetTexture;
            skyBoxCamera.cullingMask = 1 << 18;
            skyBoxCamera.nearClipPlane = 0.3f;
            skyBoxCamera.farClipPlane = maxdistance;
            skyBoxCamera.transform.position = Vector3.zero;
            skyBoxCamera.Render();

            scaledCamera.CopyFrom(groundCamera);
            scaledCamera.enabled = false;
            scaledCamera.targetTexture = groundCamera.targetTexture;
            scaledCamera.cullingMask = 1 << 9 | 1 << 10;
            scaledCamera.nearClipPlane = 0.3f;
            scaledCamera.farClipPlane = maxdistance;
            scaledCamera.transform.position = Vector3.zero;
            scaledCamera.Render();
        }

        public void OnDestroy()
        {
            skyBoxCamera.enabled = false;
            DestroyImmediate(skyBoxCamera);
            DestroyImmediate(skyBoxCameraGO);

            scaledCamera.enabled = false;
            DestroyImmediate(scaledCamera);
            DestroyImmediate(scaledCameraGO);
        }
    }

    internal static class EditorSky
    {
        static Cubemap combined;
        static bool scatterer = AssemblyLoader.loadedAssemblies.FirstOrDefault(a => a.name == "scatterer") != null;

        internal static void Update()
        {
            Debug.Log("EditorSky.Update");

            // Create GameObject and Camera
            GameObject marker = new GameObject("SigmaEditorView Camera");
            Camera camera = marker.AddOrGetComponent<Camera>();
            camera.enabled = false;
            camera.nearClipPlane = 2000;
            camera.farClipPlane = 200000;
            combined = combined ?? new Cubemap(Settings.size, TextureFormat.ARGB32, false);//new RenderTexture(Settings.size, Settings.size, 0) { name = "EditorSky.ground", dimension = TextureDimension.Cube };

            camera.transform.position = SpaceCenter.Instance.SpaceCenterTransform.position - SpaceCenter.Instance.SpaceCenterTransform.up.normalized * 22;
            camera.cullingMask = 1 << 15;
            camera.clearFlags = CameraClearFlags.Depth;

            camera.gameObject.AddComponent<AtmosphereAndSkyBoxRenderer>();
            camera.RenderToCubemap(combined);
            PrintCubemap(combined, "combined");

            // CleanUp
            Object.DestroyImmediate(camera);
            Object.DestroyImmediate(marker);
        }

        internal static void PrintCubemap(Cubemap cubemap, string name)
        {
            for (int i = 0; i < 6; i++)
            {
                Texture2D tex = new Texture2D(cubemap.width, cubemap.width);
                tex.SetPixels(cubemap.GetPixels((CubemapFace)i));
                Directory.CreateDirectory("GameData/PluginData/Cubemaps/");
                File.WriteAllBytes("GameData/PluginData/Cubemaps/" + name + "_" + i + ".png", tex.EncodeToPNG());
            }
        }

        internal static void Apply(EditorFacility editor)
        {
            Debug.Log("EditorSky.Apply", "editor = " + editor);

            RenderSettings.skybox.shader = ShaderLoader.shader;

            RenderSettings.skybox.SetTexture("_SkyBox", combined);
            //    RenderSettings.skybox.SetTexture("_Scaled", scaled);
            //   RenderSettings.skybox.SetTexture("_Ground", ground);

            if (scatterer)
            {
                RenderSettings.skybox.SetFloat("_Scatterer", 1);
            }

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
