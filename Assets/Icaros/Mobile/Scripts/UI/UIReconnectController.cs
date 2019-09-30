using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Icaros.Mobile.UI {
    public class UIReconnectController : MonoBehaviour {

        public Camera messageView;

        private void Awake() {
            Hide();
        }

        public void Show() {
            messageView.gameObject.SetActive(true);
        }

        public void Hide() {
            messageView.gameObject.SetActive(false);
        }
    }
}