using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Icaros.Mobile.UI {
    public class RealWorldTexture : MonoBehaviour {

        //s6/7 1440 x 2560

        public int width = -1;
        public int height = -1;
        public int fps = -1;

        private WebCamTexture tex;
        private Renderer renderTarget;
        private RawImage uiTarget;

        private void Start() {
            renderTarget = GetComponent<Renderer>();
            uiTarget = GetComponent<RawImage>();
        }

        WebCamTexture getWebCam() {
            if (width > 0 && height > 0) {
                if (fps > 0) {
                    return new WebCamTexture(width, height, fps);
                }

                return new WebCamTexture(width, height);
            }

            return new WebCamTexture();
        }

        public void pause() {
            if (renderTarget) {
                renderTarget.material.mainTexture = null;
            }

            if (uiTarget) {
                uiTarget.texture = null;
                uiTarget.material.mainTexture = null;
            }

            if (tex != null) {
                tex.Stop();
                DestroyImmediate(tex);
            }
        }

        public void unpause() {
            tex = getWebCam();
            tex.filterMode = FilterMode.Trilinear;

            if (renderTarget) {
                renderTarget.material.mainTexture = tex;
                tex.Play();
            }

            if (uiTarget) {
                uiTarget.texture = tex;
                uiTarget.material.mainTexture = tex;
                tex.Play();
            }

        }
    }
}
