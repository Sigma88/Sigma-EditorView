using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;


namespace SigmaEditorViewPlugin
{
    static class EditorSky
    {
        static RenderTexture skybox;
        static RenderTexture scaled;
        static RenderTexture ground;
        static bool scatterer = AssemblyLoader.loadedAssemblies.FirstOrDefault(a => a.name == "scatterer") != null;

        internal static void Update()
        {
            Debug.Log("EditorSky.Update");

            // Create GameObject and Camera
            GameObject marker = new GameObject("SigmaEditorView Camera");
            Camera camera = marker.AddOrGetComponent<Camera>();

            // Setup Camera
            camera.farClipPlane = maxdistance;
            camera.backgroundColor = Color.clear;

            // SkyBox
            skybox = skybox ?? new RenderTexture(Settings.size, Settings.size, 0) { name = "EditorSky.skybox", dimension = TextureDimension.Cube };
            camera.cullingMask = 1 << 18;
            camera.RenderToCubemap(skybox);

            // ScaledSpace and Atmosphere
            camera.backgroundColor = Color.clear;
            scaled = scaled ?? new RenderTexture(Settings.size, Settings.size, 0) { name = "EditorSky.scaled", dimension = TextureDimension.Cube }; //*/ new Cubemap(Settings.size, TextureFormat.RGBA32, false);
            camera.cullingMask = 1 << 9 | 1 << 10;
            camera.RenderToCubemap(scaled);

            // Ground
            camera.backgroundColor = Color.black;
            camera.nearClipPlane = 2000;
            camera.farClipPlane = 200000;
            camera.transform.position = SpaceCenter.Instance.SpaceCenterTransform.position - SpaceCenter.Instance.SpaceCenterTransform.up.normalized * 22;
            ground = ground ?? new RenderTexture(Settings.size, Settings.size, 0) { name = "EditorSky.ground", dimension = TextureDimension.Cube };
            camera.cullingMask = 1 << 15;
            camera.RenderToCubemap(ground);

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
