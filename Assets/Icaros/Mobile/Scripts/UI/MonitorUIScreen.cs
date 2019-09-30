using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Icaros.Mobile.UI {
    public class MonitorUIScreen : NetworkBehaviour {
        
        public MonitorUIButton[] buttons;

        public bool syncButtonTitleAndState = false;

        [HideInInspector]
        public SyncListString titleList;

        private MonitorUIButton activeButton = null;

        public void updateActiveButton(int active) {
            if (activeButton != null)
                activeButton.setActive(false);

            if (active < 0) {
                activeButton = null;
                return;
            }

            if (active > buttons.Length)
                return;

            activeButton = buttons[active];
            activeButton.setActive(true);
        }

        private void Update() {
            if (!syncButtonTitleAndState)
                return;
            if (titleList == null)
                return;

            int index = 0;
            while (index < titleList.Count) {
                buttons[index].gameObject.SetActive(true);
                buttons[index].setText(titleList[index]);
                index++;
            }

            while (index < buttons.Length) {
                buttons[index].gameObject.SetActive(false);
                index++;
            }
        }
    }
}