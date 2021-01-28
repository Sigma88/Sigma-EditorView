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
            UnityEngine.Debug.Log("SigmaLog: BEFORE COPY skyBoxCamera.transform.eulerAngles = " + (Vector3d)skyBoxCamera.transform.eulerAngles + ", skyBoxCamera.nearClipPlane" + skyBoxCamera.nearClipPlane + ", skyBoxCamera.farClipPlane" + skyBoxCamera.farClipPlane);
            skyBoxCamera.CopyFrom(groundCamera);
           UnityEngine.Debug.Log("SigmaLog: AFTER COPY skyBoxCamera.transform.eulerAngles = " + (Vector3d)skyBoxCamera.transform.eulerAngles + ", skyBoxCamera.nearClipPlane" + skyBoxCamera.nearClipPlane + ", skyBoxCamera.farClipPlane" + skyBoxCamera.farClipPlane);
            skyBoxCamera.enabled = false;
            skyBoxCamera.cullingMask = 1 << 18;
            skyBoxCamera.clearFlags = CameraClearFlags.SolidColor;
            skyBoxCamera.backgroundColor = Color.clear;

            skyBoxCamera.targetTexture = groundCamera.targetTexture;
            skyBoxCamera.nearClipPlane = EditorSky.backupnear;
            skyBoxCamera.farClipPlane = EditorSky.backupfar;
            skyBoxCamera.transform.position = EditorSky.backupcamera;
            skyBoxCamera.Render();
            UnityEngine.Debug.Log("SigmaLog: AFTER RENDER skyBoxCamera.transform.eulerAngles = " + (Vector3d)skyBoxCamera.transform.eulerAngles + ", skyBoxCamera.nearClipPlane" + skyBoxCamera.nearClipPlane + ", skyBoxCamera.farClipPlane" + skyBoxCamera.farClipPlane);


            UnityEngine.Debug.Log("SigmaLog: BEFORE COPY scaledCamera.transform.eulerAngles = " + (Vector3d)scaledCamera.transform.eulerAngles + ", scaledCamera.nearClipPlane" + scaledCamera.nearClipPlane + ", scaledCamera.farClipPlane" + scaledCamera.farClipPlane);

               scaledCamera.CopyFrom(groundCamera);  
            UnityEngine.Debug.Log("SigmaLog: AFTER COPY scaledCamera.transform.eulerAngles = " + (Vector3d)scaledCamera.transform.eulerAngles + ", scaledCamera.nearClipPlane" + scaledCamera.nearClipPlane + ", scaledCamera.farClipPlane" + scaledCamera.farClipPlane);

              scaledCamera.enabled = false;
            scaledCamera.cullingMask = 1 << 9 | 1 << 10;
            scaledCamera.clearFlags = CameraClearFlags.Depth;

            scaledCamera.targetTexture = groundCamera.targetTexture;
            scaledCamera.nearClipPlane = EditorSky.backupnear;
            scaledCamera.farClipPlane = EditorSky.backupfar;
            scaledCamera.transform.position = EditorSky.backupcamera;
            scaledCamera.Render();
            UnityEngine.Debug.Log("SigmaLog: AFTER RENDER scaledCamera.transform.eulerAngles = " + (Vector3d)scaledCamera.transform.eulerAngles + ", scaledCamera.nearClipPlane" + scaledCamera.nearClipPlane + ", scaledCamera.farClipPlane" + scaledCamera.farClipPlane);

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

  internal  static class EditorSky
    {
       static Cubemap combined;
        internal static Vector3 backupcamera;
        internal static float backupnear;
        internal static float backupfar;
        static bool scatterer = AssemblyLoader.loadedAssemblies.FirstOrDefault(a => a.name == "scatterer") != null;

        internal static void Update()
        {
            Debug.Log("EditorSky.Update");

            // Create GameObject and Camera
            GameObject marker = new GameObject("SigmaEditorView Camera");
            Camera camera = marker.AddOrGetComponent<Camera>();
            camera.enabled = false;
            backupcamera = camera.transform.position;
            // Setup Camera
            camera.farClipPlane = maxdistance;

            // Ground
            backupnear = camera.nearClipPlane;
            backupfar = camera.farClipPlane;

            camera.nearClipPlane = 2000;
            camera.farClipPlane = 200000;
            combined = combined ?? new Cubemap(Settings.size, TextureFormat.ARGB32, false);//new RenderTexture(Settings.size, Settings.size, 0) { name = "EditorSky.ground", dimension = TextureDimension.Cube };
            //if (!combined.IsCreated()) combined.Create();

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
