using UnityEngine;


namespace SigmaEditorViewPlugin
{
    internal class FlareCamera : MonoBehaviour
    {
        LensFlare flare;
        float maxBrightness;
        bool hidden = false;
        static int layerMask = 1 << 0 | 1 << 1 | 1 << 2 | 1 << 3 | 1 << 4 | 1 << 5 | 1 << 6 | 1 << 7 | 1 << 8 | 1 << 9 |
                               1 << 10 | 1 << 11 | 1 << 12 | 1 << 13 | 1 << 14 | 1 << 15 | 1 << 16 | 1 << 17 | 1 << 18 | 1 << 19 |
                               1 << 20 | 1 << 21 | 1 << 22 | 1 << 23 | 1 << 24 | 1 << 25 | 1 << 26 | 1 << 27 | 1 << 28 | 1 << 29 |
                               1 << 30 | 1 << 31;

        void Start()
        {
            flare = GetComponent<LensFlare>();
            maxBrightness = flare.brightness;
        }

        void Update()
        {
            CheckHidden();

            if (hidden)
            {
                Hide();
            }
            else
            {
                Show();
            }

            Track();
        }

        void CheckHidden()
        {
            hidden = Physics.Raycast(EditorCamera.Instance.transform.position, flare.transform.forward * -5000, Mathf.Infinity, layerMask);//, out RaycastHit hit, Mathf.Infinity, layerMask); // DateTime.Now.Second % 20 < 10;
        }

        void Show()
        {
            if (flare.brightness < maxBrightness)
                flare.brightness += flare.fadeSpeed * Time.deltaTime;

            if (flare.brightness > maxBrightness)
                flare.brightness = maxBrightness;
        }

        void Hide()
        {
            if (flare.brightness > 0)
                flare.brightness -= flare.fadeSpeed * Time.deltaTime;

            if (flare.brightness < 0)
                flare.brightness = 0;
        }

        void Track()
        {
            if (Debug.debug)
            {
                Color color = hidden ? Color.red : Color.green;
                GameObject myLine = new GameObject();
                myLine.transform.position = EditorCamera.Instance.transform.position;
                myLine.AddComponent<LineRenderer>();
                LineRenderer lr = myLine.GetComponent<LineRenderer>();
                lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
                lr.startColor = color;
                lr.endColor = color;
                lr.startWidth = 0.1f;
                lr.endWidth = 0.1f;
                lr.SetPosition(0, EditorCamera.Instance.transform.position);
                lr.SetPosition(1, flare.transform.forward * -5000);
                Destroy(myLine, 0.1f);
            }
        }
    }
}
