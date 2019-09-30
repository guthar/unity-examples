using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Icaros.Mobile.UI {
    public class UIExternalCameraScreen : MonoBehaviour {

        public new Camera camera;
        public Canvas canvas;
        public RealWorldTexture rwt;

        void Start() {
            camera.backgroundColor = UISystem.Instance.MenuCameraBackgroundColor;
        }

        public void Hide() {
            rwt.pause();
            canvas.gameObject.SetActive(false);
            camera.gameObject.SetActive(false);
        }

        public void Show() {
            canvas.gameObject.SetActive(true);
            camera.gameObject.SetActive(true);
            rwt.unpause();
        }
    }
}
