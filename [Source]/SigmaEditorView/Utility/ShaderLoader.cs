using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


namespace SigmaEditorViewPlugin
{
    internal class ShaderLoader
    {
        // Shader
        internal static Shader shader
        {
            get
            {
                if (_shader == null)
                {
                    LoadAssetBundle("Sigma/EditorView/Shaders/", "EditorSkyBox");
                    _shader = GetShader("SigmaEditorView/EditorSkyBox");
                }

                return _shader;
            }
        }

        static Shader _shader = null;

        // Asset Loader
        static Dictionary<String, Shader> shaderDictionary = new Dictionary<String, Shader>();

        static Shader GetShader(String shaderName)
        {
            Debug.Log("ShaderLoader", "GetShader " + shaderName);

            if (shaderDictionary.ContainsKey(shaderName))
            {
                return shaderDictionary[shaderName];
            }

            return null;
        }

        static void LoadAssetBundle(String path, String bundleName)
        {
            // GameData
            string fullPath = Path.Combine(KSPUtil.ApplicationRootPath + "GameData/", path);

            // Pick correct bundle for platform
            if (Application.platform == RuntimePlatform.WindowsPlayer && SystemInfo.graphicsDeviceVersion.StartsWith("OpenGL"))
                fullPath = Path.Combine(fullPath, bundleName + "-linux.unity3d");   // fixes OpenGL on windows
            else if (Application.platform == RuntimePlatform.WindowsPlayer)
                fullPath = Path.Combine(fullPath, bundleName + "-windows.unity3d");
            else if (Application.platform == RuntimePlatform.LinuxPlayer)
                fullPath = Path.Combine(fullPath, bundleName + "-linux.unity3d");
            else
                fullPath = Path.Combine(fullPath, bundleName + "-macosx.unity3d");

            Debug.Log("ShaderLoader", "Loading asset bundle at path " + path);

            using (WWW www = new WWW("file://" + fullPath))
            {
                AssetBundle bundle = www.assetBundle;
                Shader[] shaders = bundle.LoadAllAssets<Shader>();

                foreach (Shader shader in shaders)
                {
                    Debug.Log("ShaderLoader", "adding " + shader.name);
                    shaderDictionary.Add(shader.name, shader);
                }

                bundle.Unload(false); // unload the raw asset bundle
            }
        }
    }
}
