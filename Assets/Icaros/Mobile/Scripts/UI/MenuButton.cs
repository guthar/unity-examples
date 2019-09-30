using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Icaros.Mobile.UI {
    public class MenuButton : MonoBehaviour {

        public Text myText;
        private string id;
        
        public void OnClick() {
            UIManager.Instance.buttonClicked(id);
        }
        
        public void Refit(string id, string title) {
            this.id = id;
            myText.text = title;
        }
    }
}