using UnityEngine;


namespace SigmaEditorViewPlugin
{
    internal class LightTracker : MonoBehaviour
    {
        EditorFacility? editor;

        void LateUpdate()
        {
            editor = editor ?? EditorDriver.editorFacility;

            if (Debug.debug && HighLogic.LoadedScene == GameScenes.EDITOR && editor == EditorDriver.editorFacility)
            {
                Track();
            }
            else
            {
                DestroyImmediate(this);
            }
        }

        LineRenderer line;

        void Track()
        {
            if (line == null)
            {
                GameObject myLine = new GameObject("lineRenderer");
                line = myLine.AddOrGetComponent<LineRenderer>();
                line.transform.position = Camera.main.transform.position + Camera.main.transform.forward.normalized;
                line.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
                line.startColor = line.endColor = Color.yellow.A(0.5f);
                line.startWidth = 0.04f;
                line.endWidth = 50f;
            }

            if (Camera.main?.transform != null)
                line.SetPosition(0, Camera.main.transform.position + Camera.main.transform.forward.normalized);

            if (transform != null)
                line.SetPosition(1, transform.forward.normalized * -5000);
        }

        void OnDestroy()
        {
            if (line != null)
            {
                DestroyImmediate(line);
            }
        }
    }
}
