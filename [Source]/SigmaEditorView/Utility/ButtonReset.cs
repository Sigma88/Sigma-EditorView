using UnityEngine;
using UnityEngine.UI;


namespace SigmaEditorViewPlugin
{
    internal class ButtonReset : MonoBehaviour
    {
        Button btn;

        void Start()
        {
            btn = GetComponent<Button>();
        }

        void Update()
        {
            if (Input.GetMouseButtonUp(0))
            {
                btn.enabled = false;
                btn.enabled = true;
            }
        }
    }
}
