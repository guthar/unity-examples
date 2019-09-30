using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Icaros.Mobile.UI {
    public class UIWaitForController : MonoBehaviour {

        public Canvas canvas;

        private bool supposedToHide = false;

        public void Show() {
            canvas.gameObject.SetActive(true);
        }

        public void Hide(bool forced = false) {
            if (forced) {
                canvas.gameObject.SetActive(false);
                return;
            }

            supposedToHide = true;
        }

        private void Update() {
            if (canvas.gameObject.activeSelf && supposedToHide) {
                supposedToHide = false;
                canvas.gameObject.SetActive(false);
            }
        }
    }
}