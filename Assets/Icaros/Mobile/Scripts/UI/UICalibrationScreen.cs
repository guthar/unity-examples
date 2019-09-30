using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Icaros.Mobile.UI {
    public class UICalibrationScreen : MonoBehaviour {
        
        public Canvas canvas;
        public GameObject getOnText;
        public GameObject lookStraightText;
        
        public void Show(int textNr) {
            canvas.gameObject.SetActive(true);
            if (textNr == 0) {
                getOnText.gameObject.SetActive(true);
                lookStraightText.gameObject.SetActive(false);
            } else {
                getOnText.gameObject.SetActive(false);
                lookStraightText.gameObject.SetActive(true);
            }
        }

        public void Hide() {
            getOnText.SetActive(false);
            lookStraightText.SetActive(false);
            canvas.gameObject.SetActive(false);
        }
    }
}