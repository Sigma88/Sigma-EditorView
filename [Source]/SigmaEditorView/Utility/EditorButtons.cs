using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using KSP.UI.TooltipTypes;


namespace SigmaEditorViewPlugin
{
    internal static class EditorButtons
    {
        internal static Button lightsOnBtn;
        internal static Button lightsOffBtn;
        internal static Button doorsOnBtn;
        internal static Button doorsOffBtn;

        internal static void Apply(EditorFacility editor)
        {
            Debug.Log("EditorButtons.Apply", "editor = " + editor);

            // Lights Button
            if (Settings.toggleLights)
            {
                CreateButton("SigmaEditorView/Textures/LightsOn", "SigmaEditorView/Textures/LightsOff", out lightsOnBtn, out lightsOffBtn);

                lightsOnBtn.gameObject.GetComponent<TooltipController_Text>().textString = lightsOffBtn.gameObject.GetComponent<TooltipController_Text>().textString = "Toggle Lights";

                lightsOnBtn.onClick.AddListener(LightsBtnOnClick);
                lightsOffBtn.onClick.AddListener(LightsBtnOnClick);

                lightsOnBtn.gameObject.SetActive(true);
                lightsOffBtn.gameObject.SetActive(false);
            }

            // Doors Button
            if (Settings.toggleDoors)
            {
                CreateButton("SigmaEditorView/Textures/DoorsOn", "SigmaEditorView/Textures/DoorsOff", out doorsOnBtn, out doorsOffBtn, Settings.toggleLights);

                doorsOnBtn.gameObject.GetComponent<TooltipController_Text>().textString = "Close Doors";
                doorsOffBtn.gameObject.GetComponent<TooltipController_Text>().textString = "Open Doors";

                doorsOnBtn.onClick.AddListener(DoorsBtnOnClick);
                doorsOffBtn.onClick.AddListener(DoorsBtnOnClick);

                doorsOnBtn.gameObject.SetActive(true);
                doorsOffBtn.gameObject.SetActive(false);
            }

            // Cargo Button
            if (Settings.cargoBtnTexture)
            {
                Button cargoBtn = EditorLogic.fetch.cargoPanelBtn;
                cargoBtn.spriteState = new SpriteState
                {
                    highlightedSprite = Sprite.Create(Settings.cargoBtnTexture, new Rect(128, 128, 128, 128), Vector2.zero),
                    pressedSprite = Sprite.Create(Settings.cargoBtnTexture, new Rect(0, 0, 128, 128), Vector2.zero),
                    disabledSprite = Sprite.Create(Settings.cargoBtnTexture, new Rect(128, 0, 128, 128), Vector2.zero)
                };
                cargoBtn.image.sprite = Sprite.Create(Settings.cargoBtnTexture, new Rect(0, 128, 128, 128), Vector2.zero);
            }
        }

        static void CreateButton(string textureOnName, string textureOffName, out Button btnOn, out Button btnOff, bool doubleOffset = false)
        {
            Texture2D textureON = Resources.FindObjectsOfTypeAll<Texture2D>().FirstOrDefault(t => t.name == textureOnName);
            Texture2D textureOFF = Resources.FindObjectsOfTypeAll<Texture2D>().FirstOrDefault(t => t.name == textureOffName);

            if (textureON && textureOFF)
            {
                CreateButton(textureON, textureOFF, out btnOn, out btnOff, doubleOffset);
            }
            else
            {
                btnOn = null;
                btnOff = null;
            }
        }

        static void CreateButton(Texture2D textureOn, Texture2D textureOff, out Button btnOn, out Button btnOff, bool doubleOffset = false)
        {
            GameObject buttonCrew = EditorLogic.fetch.crewPanelBtn.gameObject;
            GameObject buttonCargo = EditorLogic.fetch.cargoPanelBtn.gameObject;
            GameObject buttonEditor = EditorLogic.fetch.switchEditorBtn.gameObject;
            GameObject topBar = buttonCrew.transform.parent.gameObject;

            Button oldButton = buttonCrew.GetComponent<Button>();

            GameObject buttonOn = Object.Instantiate(buttonCrew);
            GameObject buttonOff = Object.Instantiate(buttonCrew);
            buttonOn.transform.SetParent(topBar.transform);
            buttonOff.transform.SetParent(topBar.transform);

            buttonOn.transform.position = buttonOff.transform.position = buttonEditor.transform.position + (buttonEditor.transform.position - buttonCargo.transform.position) * (doubleOffset ? 2 : 1);
            buttonOn.transform.localScale = buttonOff.transform.localScale = buttonCargo.transform.localScale;
            buttonOn.transform.rotation = buttonOff.transform.rotation = buttonCargo.transform.rotation;

            Object.DestroyImmediate(buttonOff.GetComponent<Button>());
            btnOn = buttonOff.AddOrGetComponent<Button>();
            btnOn.image = buttonOff.GetComponent<Image>();

            Object.DestroyImmediate(buttonOn.GetComponent<Button>());
            btnOff = buttonOn.AddOrGetComponent<Button>();
            btnOff.image = buttonOn.GetComponent<Image>();

            btnOn.transition = Selectable.Transition.SpriteSwap;
            btnOn.spriteState = new SpriteState
            {
                highlightedSprite = Sprite.Create(textureOn, new Rect(128, 128, 128, 128), Vector2.zero),
                pressedSprite = Sprite.Create(textureOn, new Rect(0, 0, 128, 128), Vector2.zero),
                disabledSprite = Sprite.Create(textureOn, new Rect(128, 0, 128, 128), Vector2.zero)
            };
            btnOn.image.sprite = Sprite.Create(textureOn, new Rect(0, 128, 128, 128), Vector2.zero);

            btnOff.transition = Selectable.Transition.SpriteSwap;
            btnOff.spriteState = new SpriteState
            {
                highlightedSprite = Sprite.Create(textureOff, new Rect(128, 128, 128, 128), Vector2.zero),
                pressedSprite = Sprite.Create(textureOff, new Rect(0, 0, 128, 128), Vector2.zero),
                disabledSprite = Sprite.Create(textureOff, new Rect(128, 0, 128, 128), Vector2.zero)
            };
            btnOff.image.sprite = Sprite.Create(textureOff, new Rect(0, 128, 128, 128), Vector2.zero);

            btnOn.gameObject.AddOrGetComponent<ButtonReset>();
            btnOff.gameObject.AddOrGetComponent<ButtonReset>();
        }

        static void LightsBtnOnClick()
        {
            bool state = !lightsOnBtn.isActiveAndEnabled;

            lightsOnBtn.gameObject.SetActive(state);
            lightsOffBtn.gameObject.SetActive(!state);
        }

        static void DoorsBtnOnClick()
        {
            bool state = !doorsOnBtn.isActiveAndEnabled;

            doorsOnBtn.gameObject.SetActive(state);
            doorsOffBtn.gameObject.SetActive(!state);
        }
    }
}
