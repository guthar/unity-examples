using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Icaros.Mobile.UI {
    public class MonitorUICompanyLogo : MonoBehaviour {

        void Update() {
            if (transform.childCount < 1) {
                GameObject logo = Instantiate(UISystem.Instance.CompanyLogoTemplate);
                logo.transform.SetParent(transform, false);
                logo.transform.localScale = new Vector3(1f, 1.35f, 1f);
                logo.layer = this.gameObject.layer;
            }
        }
    }
}