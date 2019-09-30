using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Icaros.Mobile.UI {
    public class MonitorUIButton : MonoBehaviour {

        public Sprite defaultSpriteOverride;
        public Sprite hoverSpriteOverride;

        private Image myImage;
        private Text myText;

        private void Awake() {
            myImage = GetComponent<Image>();
            myText = GetComponentInChildren<Text>();
        }

        public void setText(string t) {
            if (myText != null)
                myText.text = t;
        }

        public void setActive(bool active) {
            if (!defaultSpriteOverride)
                defaultSpriteOverride = UISystem.Instance.DefaultButton;
            if (!hoverSpriteOverride)
                hoverSpriteOverride = UISystem.Instance.HoverButton;

            if (active)
                myImage.sprite = hoverSpriteOverride;
            else
                myImage.sprite = defaultSpriteOverride;
        }
    }
}