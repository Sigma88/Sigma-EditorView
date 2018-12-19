using UnityEngine;


namespace SigmaEditorViewPlugin
{
    internal class FlareRemover : MonoBehaviour
    {
        LensFlare flare;
        bool skip = true;

        void Awake()
        {
            flare = GetComponent<LensFlare>();
            Debug.Log("FlareRemover.Awake", "flare = " + flare);

            if (flare?.enabled != true)
            {
                DestroyImmediate(this);
            }
        }

        void Start()
        {
            TimingManager.LateUpdateAdd(TimingManager.TimingStage.BetterLateThanNever, LaterUpdate);
        }

        void LaterUpdate()
        {
            if (HighLogic.LoadedScene == GameScenes.EDITOR)
            {
                flare.enabled = skip = false;

                Track();
            }

            else

            if (!skip)
            {
                flare.enabled = skip = true;
                DestroyImmediate(this);
            }
        }

        LineRenderer line;

        void Track()
        {
            if (Debug.debug)
            {
                if (line == null)
                {
                    GameObject myLine = new GameObject("lineRenderer");
                    line = myLine.AddOrGetComponent<LineRenderer>();
                    line.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
                    line.startColor = line.endColor = Color.black;
                    line.startWidth = 0.02f;
                    line.endWidth = 25f;
                }

                if (Camera.main?.transform != null)
                    line.SetPosition(0, Camera.main.transform.position + Camera.main.transform.forward.normalized);

                if (transform != null)
                    line.SetPosition(1, transform.forward * -5000);
            }
        }

        void OnDestroy()
        {
            TimingManager.LateUpdateRemove(TimingManager.TimingStage.BetterLateThanNever, LaterUpdate);
        }
    }
}
