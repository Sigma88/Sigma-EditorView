using UnityEngine;


namespace SigmaEditorViewPlugin
{
    [KSPAddon(KSPAddon.Startup.SpaceCentre, false)]
    internal class KSCTriggers : MonoBehaviour
    {
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log("KSCTriggers.Update");
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
            Debug.Log("EditorTriggers.Start");

            // Enable FlareLayer
            EditorCamera.Instance.cam.GetComponent<FlareLayer>().enabled = true;

            // Set Editor Facility
            editor = EditorDriver.editorFacility;

            Debug.Log("EditorTriggers.Start", "editor = " + editor);

            // Apply
            EditorView.Apply(editor);
        }

        void Update()
        {
            if (EditorDriver.editorFacility != editor)
            {
                // Change Editor Facility
                editor = EditorDriver.editorFacility;

                Debug.Log("EditorTriggers.Update", "Editor Facility Changed To = " + editor);

                // Apply
                EditorView.Apply(editor);
            }

            if (Debug.debug && Input.GetKeyDown("space"))
            {
                EditorColliders.Apply(EditorDriver.editorFacility);
            }
        }

        void OnDestroy()
        {
            Debug.Log("EditorTriggers.OnDestroy");
            EditorView.OnDestroy();
        }
    }
}
