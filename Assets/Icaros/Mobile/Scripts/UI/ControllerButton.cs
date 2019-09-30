using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Icaros.Mobile.Input;

namespace Icaros.Mobile.UI {
    public class ControllerButton : MonoBehaviour {

        public Text myText;
        public UIControllerSelect select;

        public IInputDevice device = null;
        
        public void OnClick() {
            select.OnClicked(device);
        }

        public void OnScroll() {
            select.OnScroll();
        }

        public void Refit(IInputDevice device) {
            this.device = device;
            myText.text = device.GetDeviceName();
        }
    }
}