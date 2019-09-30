using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Icaros.Mobile.Localization {
    public class LocalizedText : MonoBehaviour {
        public string tokenID = "";
        private bool init = false;

        private void Update() {
            if (init)
                return;

            Text t = GetComponent<Text>();

            if (t == null)
                return;

            t.text = LocalizationManager.Get(tokenID);
            init = true;
        }
    }
}