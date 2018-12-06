using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;


namespace SigmaEditorViewPlugin
{
    static class EditorSky
    {
        static RenderTexture skybox;
        static RenderTexture scaled;

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

            // ScaledSpace
            GameObject Atmosphere = FlightGlobals.GetHomeBody()?.scaledBody?.GetChild("Atmosphere");
            bool? atmoBackUp = Atmosphere?.activeSelf;
            if (atmoBackUp == false) Atmosphere.SetActive(true);
            scaled = scaled ?? new RenderTexture(Settings.size, Settings.size, 0) { name = "EditorSky.scaled", dimension = TextureDimension.Cube };
            camera.cullingMask = 1 << 9 | 1 << 10;
            camera.RenderToCubemap(scaled);
            if (atmoBackUp == false) Atmosphere.SetActive(false);

            // CleanUp
            Object.DestroyImmediate(camera);
            Object.DestroyImmediate(marker);
        }

        internal static void Apply(EditorFacility editor)
        {
            RenderSettings.skybox.shader = ShaderLoader.shader;

            RenderSettings.skybox.SetTexture("_SkyBox", skybox);
            RenderSettings.skybox.SetTexture("_Scaled", scaled);

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

        static void Export(Cubemap cube, string name)
        {
            for (int i = 0; i < 6; i++)
            {
                Texture2D texture = new Texture2D(Settings.size, Settings.size);
                texture.SetPixels(cube.GetPixels((CubemapFace)i));
                byte[] png = texture.EncodeToPNG();
                System.IO.File.WriteAllBytes("GameData/Sigma/cube" + name + "_" + (CubemapFace)i + ".png", png);
            }
        }
    }
}
