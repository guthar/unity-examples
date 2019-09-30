using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Icaros.Mobile.Player;

namespace Icaros.Mobile.UI {
    public class MonitorUI : NetworkBehaviour {
        public Camera monitorView;
        public GameObject loadingScreen;
        public MonitorUIScreen[] MonitorUIScreens;

        [SyncVar]
        private int synchronisedScreenId = -1;
        private int currentScreenId = -1;

        [SyncVar]
        private int synchronizedButtonId = -1;
        private int currentButtonId = -1;

        private SyncListString buttonTitles = new SyncListString();

        public void setScreenId(int id) {
            synchronisedScreenId = id;
        }

        public void setScreenIdConditional(int id, int previousScreenId) {
            if (synchronisedScreenId == previousScreenId)
                synchronisedScreenId = id;
        }

        public void OnSelected(int buttonId) {
            synchronizedButtonId = buttonId;
        }

        public void OnDeselected() {
            synchronizedButtonId = -1;
        }

        public void setButtonSyncList(List<string> toSync) {
            buttonTitles.Clear();
            for (int i = 0; i < toSync.Count; i++) {
                buttonTitles.Add(toSync[i]);
            }
        }

        private void Update() {
            if (UISystem.Instance.state == UISystem.State.Mirror) {
                updateMonitorScreen();
            }
        }

        private void Start() {
            monitorView.backgroundColor = UISystem.Instance.MenuCameraBackgroundColor;
            loadingScreen.SetActive(true);
            foreach (MonitorUIScreen screen in MonitorUIScreens)
                screen.gameObject.SetActive(false);
        }

        private void updateMonitorScreen() {
            if (synchronisedScreenId != currentScreenId) {
                currentScreenId = synchronisedScreenId;

                loadingScreen.SetActive(false);
                foreach (MonitorUIScreen screen in MonitorUIScreens)
                    screen.gameObject.SetActive(false);

                if (currentScreenId < MonitorUIScreens.Length) {
                    monitorView.enabled = true;
                    MonitorUIScreens[currentScreenId].gameObject.SetActive(true);
                    MonitorUIScreens[currentScreenId].titleList = buttonTitles;
                } else {
                    monitorView.enabled = false;
                }
            }

            if (synchronizedButtonId != currentButtonId) {
                currentButtonId = synchronizedButtonId;
                if (currentScreenId < MonitorUIScreens.Length) {
                    MonitorUIScreens[currentScreenId].updateActiveButton(currentButtonId);
                }
            }
        }
    }
}