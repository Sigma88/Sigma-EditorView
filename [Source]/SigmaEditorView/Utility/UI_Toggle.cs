using UnityEngine;


namespace SigmaEditorViewPlugin
{
    [KSPAddon(KSPAddon.Startup.EditorAny, false)]
    internal class Toggle_UI : MonoBehaviour
    {
        GameObject MainCanvas;

        GameObject AppCanvas;

        bool state = true;

        double wait = 2.0;

        void Start()
        {
            MainCanvas = GameObject.Find("_UIMaster").GetChild("MainCanvas");
            AppCanvas = GameObject.Find("_UIMaster").GetChild("AppCanvas");
        }

        void Update()
        {
            if (wait > 0.0)
            {
                wait -= Time.deltaTime;
            }

            else

            if (GameSettings.TOGGLE_UI.GetKeyDown(false) && HighLogic.LoadedSceneIsEditor && !EditorDriver.fetch.restartingEditor)
            {
                state = !state;
                MainCanvas.SetActive(state);
                AppCanvas.SetActive(state);
            }
        }
    }
}
