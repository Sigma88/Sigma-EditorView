using System.Linq;
using System.Reflection;
using UnityEngine;


namespace SigmaEditorViewPlugin
{
    internal static class EditorFlaresScatterer
    {
        static MeshRenderer[] meshRenderers;
        static RenderTexture[] renderTextures;
        static Vector3[] positions;

        internal static void Update()
        {
            Debug.Log("EditorFlaresScatterer.Update");

            DestroyAll();
            Duplicate(Resources.FindObjectsOfTypeAll<MonoBehaviour>().Where(mb => mb?.GetType()?.FullName == "scatterer.SunFlare")?.ToArray());
        }

        internal static void OnPreRender(EditorFacility editor)
        {
            //UnityEngine.Debug.Log("EditorFlaresScatterer.OnPreRender", "editor = " + editor);

            int? n = meshRenderers?.Length;

            //UnityEngine.Debug.Log("EditorFlaresScatterer.OnPreRender", "MeshRenderers found = " + n);
            //UnityEngine.Debug.Log("EditorFlaresScatterer.OnPreRender", "Positions found = " + positions?.Length);

            for (int i = 0; i < n; i++)
            {
                //UnityEngine.Debug.Log("EditorFlaresScatterer.OnPreRender", "meshRenderers[" + i + "] = " + meshRenderers[i]);
                //UnityEngine.Debug.Log("EditorFlaresScatterer.OnPreRender", "positions[" + i + "] = " + positions[i]);

                Material material = meshRenderers[i]?.material;
                //UnityEngine.Debug.Log("EditorFlaresScatterer.OnPreRender", "material = " + material);

                if (material)
                {
                    // Get the position Relative to the VAB
                    Vector3 position = positions[i];

                    // Correct the position if we are in the SPH
                    if (editor == EditorFacility.SPH)
                        position -= EditorBuildings.SPHposition;

                    //UnityEngine.Debug.Log("EditorFlaresScatterer.OnPreRender", "position = " + position);

                    // Calculate sunViewPortPos
                    Vector3 sunViewPortPos = EditorCamera.Instance.cam.WorldToViewportPoint(EditorView.Rotation * position);

                    // Set the correct sunViewPortPos
                    material.SetVector("sunViewPortPos", sunViewPortPos);

                    // Only Display the flare if it is visible
                    if (sunViewPortPos.z > 0)
                    {
                        // Set Other Properties
                        material.SetFloat(Shader.PropertyToID("renderSunFlare"), 1.0f);
                        material.SetFloat(Shader.PropertyToID("renderOnCurrentCamera"), 1.0f);
                        material.SetFloat(Shader.PropertyToID("useDbufferOnCamera"), 0.0f);
                    }
                }
            }
        }

        internal static void OnPostRender()
        {
            //UnityEngine.Debug.Log("EditorFlaresScatterer.OnPostRender");

            int? n = meshRenderers?.Length;

            //UnityEngine.Debug.Log("EditorFlaresScatterer.OnPostRender", "MeshRenderers found = " + n);
            //UnityEngine.Debug.Log("EditorFlaresScatterer.OnPostRender", "Positions found = " + positions?.Length);

            for (int i = 0; i < n; i++)
            {
                //UnityEngine.Debug.Log("EditorFlaresScatterer.OnPostRender", "meshRenderers[" + i + "] = " + meshRenderers[i]);
                //UnityEngine.Debug.Log("EditorFlaresScatterer.OnPostRender", "positions[" + i + "] = " + positions[i]);

                Material material = meshRenderers[i].material;
                //UnityEngine.Debug.Log("EditorFlaresScatterer.OnPostRender", "material = " + material);

                if (material)
                {
                    // Set Other Properties
                    material.SetFloat(Shader.PropertyToID("renderSunFlare"), 0.0f);
                    material.SetFloat(Shader.PropertyToID("renderOnCurrentCamera"), 0.0f);
                    material.SetFloat(Shader.PropertyToID("useDbufferOnCamera"), 0.0f);
                }
            }
        }

        static void DestroyAll()
        {
            int? n = meshRenderers?.Length;

            Debug.Log("EditorFlaresScatterer.DestroyAll", "sunFlares found = " + n);

            for (int i = 0; i < n; i++)
            {
                try { Object.DestroyImmediate(meshRenderers[i].gameObject); } catch { }
                try { Object.DestroyImmediate(renderTextures[i]); } catch { }
            }
        }

        static void Duplicate(MonoBehaviour[] sunFlares)
        {
            int? n = sunFlares?.Length;
            Debug.Log("EditorFlaresScatterer.Duplicate", "sunFlares found = " + (n ?? 0));
            meshRenderers = new MeshRenderer[n ?? 0];
            renderTextures = new RenderTexture[n ?? 0];
            positions = new Vector3[n ?? 0];

            for (int i = 0; i < n; i++)
            {
                // Clone SunFlare GameObject
                GameObject clone = Object.Instantiate(sunFlares[i].GetSunFlareGO());
                clone.layer = 16;
                Object.DontDestroyOnLoad(clone);

                // Store MeshRenderer
                meshRenderers[i] = clone.GetComponent<MeshRenderer>();

                // Clone SunFlare Extinction Texture
                renderTextures[i] = sunFlares[i].GetSunFlareRT();
                Object.DontDestroyOnLoad(renderTextures[i]);

                if (meshRenderers[i])
                {
                    // Set new Extinction Texture
                    meshRenderers[i].material.SetTexture("extinctionTexture", renderTextures[i]);

                    // Get SunFlare Source
                    CelestialBody source = sunFlares[i].GetSunFlareCB();

                    if (source)
                    {
                        // Set Position of Source relative to VAB
                        positions[i] = source.transform.position - EditorBuildings.VABposition;
                    }
                }
            }
        }

        static GameObject GetSunFlareGO(this MonoBehaviour sunFlare)
        {
            try
            {
                GameObject GO = sunFlare?.GetType()?.GetField("sunflareGameObject", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(sunFlare) as GameObject;

                Debug.Log("EditorFlaresScatterer.GetSunFlareGO", "Found GameObject = '" + GO + "' for sunFlare: " + sunFlare);

                return GO;
            }
            catch
            {
                Debug.Log("EditorFlaresScatterer.GetSunFlareGO", "Failed to find GameObject for sunFlare: " + sunFlare);
                return null;
            }
        }

        static RenderTexture GetSunFlareRT(this MonoBehaviour sunFlare)
        {
            try
            {
                RenderTexture oldRT = sunFlare?.GetType()?.GetField("extinctionTexture", BindingFlags.Public | BindingFlags.Instance)?.GetValue(sunFlare) as RenderTexture;

                Debug.Log("EditorFlaresScatterer.GetSunFlareRT", "Found RenderTexture = '" + oldRT + "' for sunFlare: " + sunFlare);

                RenderTexture newRT = new RenderTexture(oldRT);
                Graphics.Blit(oldRT, newRT);

                Debug.Log("EditorFlaresScatterer.GetSunFlareRT", "Created RenderTexture = '" + newRT + "' for sunFlare: " + sunFlare);

                return newRT;
            }
            catch
            {
                Debug.Log("EditorFlaresScatterer.GetSunFlareRT", "Failed to find RenderTexture for sunFlare: " + sunFlare);
                return null;
            }
        }

        static CelestialBody GetSunFlareCB(this MonoBehaviour sunFlare)
        {
            try
            {
                CelestialBody CB = sunFlare?.GetType()?.GetField("source", BindingFlags.Public | BindingFlags.Instance)?.GetValue(sunFlare) as CelestialBody;

                Debug.Log("EditorFlaresScatterer.GetSunFlareCB", "Found CelestialBody = '" + CB + "' for sunFlare: " + sunFlare);

                return CB;
            }
            catch
            {
                Debug.Log("EditorFlaresScatterer.GetSunFlareCB", "Failed to find CelestialBody for sunFlare: " + sunFlare);
                return null;
            }
        }
    }


    internal static class TextureUtility
    {
        internal static void EncodeToPNG(this RenderTexture RT, string path)
        {
            RenderTexture oldActive = RenderTexture.active;
            RenderTexture.active = RT;

            // Load new texture
            Texture2D finalTexture = new Texture2D(RT.width, RT.height);
            finalTexture.ReadPixels(new Rect(0, 0, finalTexture.width, finalTexture.height), 0, 0);

            int counter = 0;

            while (System.IO.File.Exists(path + counter + ".png"))
            {
                counter++;
            }
            System.IO.File.WriteAllBytes(path + counter + ".png", finalTexture.EncodeToPNG());
            RenderTexture.active = oldActive;
        }
    }
}
