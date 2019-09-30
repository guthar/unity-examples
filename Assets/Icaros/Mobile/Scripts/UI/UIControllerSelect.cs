using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Icaros.Mobile.Input;

namespace Icaros.Mobile.UI {
    public class UIControllerSelect : MonoBehaviour {

        void Start() {
            DeviceManager.Instance.NewDeviceRegistered += OnDeviceFound;
            DeviceManager.Instance.DeviceLost += OnDeviceLost;
        }

        public ControllerButton[] buttons;
        public Canvas canvas;
        public bool onlyShowIcarosControllers = true;

        private List<IInputDevice> devices = new List<IInputDevice>();
        private int page = 0;

        private bool markedDirty = false;
        private MonitorUI monitorController;

        public void Hide() {
            canvas.gameObject.SetActive(false);
        }

        public void Show() {
            canvas.gameObject.SetActive(true);
            refresh();
        }

        public void clear() {
            devices.Clear();
        }

        internal void OnDeviceFound(IInputDevice device) {
            if (onlyShowIcarosControllers && !(device.GetDeviceTypeID() == DeviceManager.ICAROS_CONTROLLER_DEVICE_ID))
                return;

            devices.Add(device);
            markedDirty = true;
        }

        private void Update() {
            if (markedDirty && canvas.gameObject.activeSelf)
                refresh();
        }

        internal void OnDeviceLost(IInputDevice device) {
            devices.Remove(device);
            if (canvas.gameObject.activeSelf)
                refresh();
        }

        private void refresh() {
            if (page * buttons.Length >= devices.Count) {
                page = 0;
            }

            foreach (ControllerButton b in buttons) {
                b.gameObject.SetActive(false);
            }

            List<string> titlesToSync = new List<string>();
            titlesToSync.Add("scrollButtonDummy");
            for (int i = 0; i < buttons.Length; i++) {
                int pos = i + page * buttons.Length;
                if (pos < devices.Count) {
                    buttons[i].gameObject.SetActive(true);
                    buttons[i].Refit(devices[pos]);
                    titlesToSync.Add(devices[pos].GetDeviceName());
                }
            }
            UISystem.Instance.resyncButtons(titlesToSync);

            markedDirty = false;
        }

        public void OnClicked(IInputDevice device) {
            UISystem.Instance.OnControllerSelected(device);
            canvas.gameObject.SetActive(false);
        }

        public void OnScroll() {
            page++;
            Show();
        }
    }
}