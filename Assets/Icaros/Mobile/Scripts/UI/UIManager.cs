using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Icaros.Mobile.Localization;
using System.Linq;
using System;

namespace Icaros.Mobile.UI {
    public class UIManager : MonoBehaviour {
        public static UIManager Instance = null;

        void Awake() {
            if (Instance == null) {
                Instance = this;
            }
        }

        public UnityEngine.EventSystems.EventSystem eventSystem;

        public Canvas canvas;
        public MenuButton[] buttons;
        public GameObject[] scrollButtons;

        private Dictionary<string, MenuItem> menuItems = new Dictionary<string, MenuItem>();
        private MenuItem root = new MenuItem();
        private MenuItem unlisted = new MenuItem();

        private List<MenuItem> currentDisplay;
        private int lowerPageLimit = 0;
        private int upperPageLimit = 3;

        public void RegisterMenuItem(string id, string title, string parent, bool FirstInList = false) {
            if (!menuItems.ContainsKey(parent)) {
                Debug.LogError("Parent with id: " + parent + " does not exist!");
                return;
            }
            registerMenuItem(id, title, menuItems[parent], FirstInList);
        }
        //to register to the root menu
        public void RegisterMenuItem(string id, string title) {
            registerMenuItem(id, title, root, false);
        }
        //to generate a new menu anchor object
        public void RegisterUnlistedMenuItem(string id, string title) {
            registerMenuItem(id, title, unlisted, false);
        }

        public void OpenMenu(string id) {
            if (!menuItems.ContainsKey(id)) {
                Debug.LogError("Menu Item with id: " + id + " does not exist!");
                return;
            }
            openMenu(menuItems[id].children);
        }
        //to open the root menu
        public void OpenMenu() {
            openMenu(root.children);
        }

        public void CloseMenu() {
            disableAllButtons();
            canvas.gameObject.SetActive(false);
        }

        internal void buttonClicked(string id) {
            if (menuItems.ContainsKey(id) && menuItems[id].children.Count > 0)
                OpenMenu(id);

            UISystem.Instance.OnUIMenuButtonClicked(id);
        }

        internal void submitButtonPressed() {
            MenuButton active = getCurrentlySelectedButton();
            if (!active) return;

            if (buttons[0].gameObject.activeSelf)
                active.OnClick();
        }

        internal void selectNextButton() {
            if (buttons.Length < 1 || !buttons[0].gameObject.activeSelf)
                return;
            if (currentDisplay.Count < 2)
                return;

            MenuButton active = getCurrentlySelectedButton();
            if (!active) return;

            int currentButton = Array.FindIndex(buttons, b => b == active) + lowerPageLimit;
            currentButton++;

            if (currentButton > upperPageLimit) {
                if (currentButton == currentDisplay.Count) {
                    currentButton = 0;
                    lowerPageLimit = 0;
                    upperPageLimit = Mathf.Min(buttons.Length - 1, currentDisplay.Count - 1);
                } else {
                    int shift = Mathf.Min(buttons.Length,
                                          currentDisplay.Count - currentButton);

                    lowerPageLimit += shift;
                    upperPageLimit += shift;
                }

                showShift();
            }
            
            eventSystem.SetSelectedGameObject(buttons[currentButton - lowerPageLimit].gameObject);
        }

        internal void selectPreviousButton() {
            if (buttons.Length < 1 || !buttons[0].gameObject.activeSelf)
                return;
            if (currentDisplay.Count < 2)
                return;

            MenuButton active = getCurrentlySelectedButton();
            if (!active) return;

            int currentButton = Array.FindIndex(buttons, b => b == active) + lowerPageLimit;
            currentButton--;

            if (currentButton < lowerPageLimit) {
                if (currentButton < 0) {
                    currentButton = currentDisplay.Count - 1;
                    upperPageLimit = currentDisplay.Count - 1;
                    lowerPageLimit = Mathf.Max(0, upperPageLimit - (buttons.Length - 1));
                } else {
                    int shift = Mathf.Min(buttons.Length, currentButton + 1);

                    lowerPageLimit -= shift;
                    upperPageLimit -= shift;
                }

                showShift();
            }
            
            eventSystem.SetSelectedGameObject(buttons[currentButton - lowerPageLimit].gameObject);
        }

        private MenuButton getCurrentlySelectedButton() {
            GameObject currentObject = eventSystem.currentSelectedGameObject;
            if (!currentObject) return buttons[0];
            return currentObject.GetComponent<MenuButton>();
        }
        
        private void showShift() {
            List<string> titlesToSync = new List<string>();

            for (int i = 0; i < currentDisplay.Count && i < buttons.Length; i++) {
                string text = LocalizationManager.Get(currentDisplay[i + lowerPageLimit].title);
                buttons[i].Refit(currentDisplay[i + lowerPageLimit].id, text);
                titlesToSync.Add(text);
            }

            if (titlesToSync.Count == 4) {
                titlesToSync.Add("placeholder ScrollButton");
                titlesToSync.Add("placeholder ScrollButton");
            }

            UISystem.Instance.resyncButtons(titlesToSync);
        }

        private void registerMenuItem(string id, string title, MenuItem parentItem, bool first) {
            //this allows reusing of the same button in different menus
            //backToMainMenu for example
            MenuItem registered;
            if (menuItems.ContainsKey(id)) {
                registered = menuItems[id];
            } else {
                registered = new MenuItem() { id = id, title = title };
                menuItems.Add(id, registered);
            }

            if (first)
                parentItem.children.Insert(0, registered);
            else
                parentItem.children.Add(registered);
        }

        private void openMenu(List<MenuItem> toDisplay) {
            currentDisplay = toDisplay;
            lowerPageLimit = 0;
            upperPageLimit = Mathf.Min(toDisplay.Count - 1, buttons.Length - 1);
            canvas.gameObject.SetActive(true);

            disableAllButtons();
            populateButtonPool(toDisplay);
            showShift();
            eventSystem.SetSelectedGameObject(buttons[0].gameObject);
        }

        private void populateButtonPool(List<MenuItem> toDisplay) {
            for (int i = 0; i < toDisplay.Count && i < buttons.Length; i++) {
                buttons[i].gameObject.SetActive(true);
            }
            if (toDisplay.Count > buttons.Length) {
                foreach (GameObject g in scrollButtons)
                    g.SetActive(true);
            }
        }
        
        private void disableAllButtons() {
            if (eventSystem != null)
                eventSystem.SetSelectedGameObject(this.gameObject);
            foreach (MenuButton b in buttons)
                b.gameObject.SetActive(false);
            foreach (GameObject g in scrollButtons)
                g.SetActive(false);
        }

        private class MenuItem {
            public string id;
            public string title;
            public List<MenuItem> children = new List<MenuItem>();
        }
    }
}