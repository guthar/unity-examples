using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Icaros.Mobile.UI {
    public class UICompanyLogo : MonoBehaviour {

        public Canvas canvas;

        public void Show() {
            canvas.gameObject.SetActive(true);

            if (canvas.transform.childCount < 1) {
                GameObject logo = Instantiate(UISystem.Instance.CompanyLogoTemplate);
                logo.transform.SetParent(canvas.transform, false);
            }

            StartCoroutine(WaitForLogo());
        }

        public void Hide() {
            canvas.gameObject.SetActive(false);
        }

        private IEnumerator WaitForLogo() {
            yield return new WaitForSecondsRealtime(UISystem.Instance.CompanyLogoDisplayTime);
            UISystem.Instance.OnCompanyLogoDisplayFinished();
            canvas.gameObject.SetActive(false);
        }
    }
}