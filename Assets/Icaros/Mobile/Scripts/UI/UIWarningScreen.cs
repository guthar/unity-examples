using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Icaros.Mobile.UI {
    public class UIWarningScreen : MonoBehaviour {

        public Canvas canvas;

        public void Show() {
            canvas.gameObject.SetActive(true);
        }

        public void Hide() {
            canvas.gameObject.SetActive(false);
        }

        public void OnButtonClicked() {
            UISystem.Instance.OnWarningScreenClosed();
            canvas.gameObject.SetActive(false);
        }
    }
}