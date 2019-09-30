using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Icaros.Mobile.UI {
    public class UIControllerLost : MonoBehaviour {

        public Camera messageView;
        public float DisplayTime = 2.0f;

        private void Awake() {
            Hide();
        }

        public void Show() {
            messageView.gameObject.SetActive(true);
            StartCoroutine(WaitForMessage());
        }

        public void Hide() {
            messageView.gameObject.SetActive(false);
        }

        private IEnumerator WaitForMessage() {
            yield return new WaitForSecondsRealtime(DisplayTime);
            Hide();
            UISystem.Instance.OnControllerLostScreenClosed();
        }
    }
}