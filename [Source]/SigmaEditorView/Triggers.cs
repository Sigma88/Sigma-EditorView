using UnityEngine;
using System;


namespace SigmaEditorViewPlugin
{
    [KSPAddon(KSPAddon.Startup.SpaceCentre, false)]
    internal class KSCTriggers : MonoBehaviour
    {
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log("KSCTriggers.Update", "Detected left mouse button click");
                EditorView.Update();
            }
        }
    }

    [KSPAddon(KSPAddon.Startup.EditorAny, false)]
    internal class EditorTriggers : MonoBehaviour
    {
        EditorFacility editor;

        void Start()
        {
            Debug.Log("EditorTriggers.Start", "Editor = " + EditorDriver.editorFacility);

            // Enable FlareLayer
            EditorCamera.Instance.cam.GetComponent<FlareLayer>().enabled = true;

            // Set Editor Facility
            editor = EditorDriver.editorFacility;
            EditorView.Apply(editor);
        }

        void Update()
        {
            if (EditorDriver.editorFacility != editor)
            {
                Debug.Log("EditorTriggers.Update", "Editor Facility Changed To = " + EditorDriver.editorFacility);

                // Set Editor Facility
                editor = EditorDriver.editorFacility;
                EditorView.Apply(editor);
            }
        }

        void OnDestroy()
        {
            Debug.Log("EditorTriggers.OnDestroy", "Editor = " + EditorDriver.editorFacility);
            EditorFlares.DestroyAll();
        }
    }
}
