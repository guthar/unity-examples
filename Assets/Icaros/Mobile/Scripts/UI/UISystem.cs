using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Icaros.Mobile.Input;
using Icaros.Mobile.Localization;
using Icaros.Mobile.Player;

namespace Icaros.Mobile.UI {
    public class UISystem : MonoBehaviour {
        public enum State { Player, Mirror }

        public static UISystem Instance = null;

        void Awake() {
            if (Instance != null) {
                Destroy(this.gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
            
            if (UISystem.Instance.state == UISystem.State.Mirror) {
                playerState.SetActive(false);
                monitorState.SetActive(true);
            } else {
                playerState.SetActive(true);
                monitorState.SetActive(false);
            }
        }

        public event System.Action<string> MenuItemSelected = delegate { };
        public event System.Action PlayPressed = delegate { };
        public event System.Action CameraPositionCalibrated = delegate { };

        public State state = State.Player;
        public GameObject CompanyLogoTemplate;
        public float CompanyLogoDisplayTime = 3.0f;

        public Camera UsedCamera;
        public Color MenuCameraBackgroundColor;
        public LayerMask MenuCameraCullingMask;
        public float viewDistance = 10f;
        public GameObject GazeEvents;
        public GameObject ButtonEvents;
        public GameObject CrosshairTemplate;

        public GameObject playerState;
        public GameObject monitorState;
        public GameObject monitorLoadingScreen;
        public Sprite DefaultButton;
        public Sprite HoverButton;

        internal bool DeviceSubmitButtonPressed = false;
        internal bool DeviceSubmitButtonReleased = false;

        private CameraClearFlags outsideOfMenuFlags;
        private Color outsideOfMenuColor;
        private int outsideOfMenuCullingMask;

        private GameObject Crosshair;
        private UIWarningScreen WarningScreen;
        private UIControllerSelect ControllerSelect;
        private UICompanyLogo CompanyLogoScreen;
        private UIExternalCameraScreen ExternalCameraScreen;
        private UIWaitForController WaitForController;
        private UICalibrationScreen CalibrationScreen;
        private UIReconnectController ReconnectControllerScreen;
        private UINoBluetooth NoBluetoothScreen;
        private UIControllerLost ControllerLostScreen;

        private MonitorUI monitorController;

        private bool cameraTargetingActive = false;
        private bool firstStart = true;
        private bool userSeated = false;
        private bool userMoving = true;
        private bool submitAsAny = true;

        void Start() {
            initializeCamera();
            initializeComponents();

            if (UISystem.Instance.state == UISystem.State.Mirror) 
                return;

            initializeMenuStructure();
            DeviceManager.Instance.DeviceUsed += DeviceRunning;
            DeviceManager.Instance.DeviceReconnecting += ControllerReconnecting;
            DeviceManager.Instance.DeviceLost += ControllerLost;
            ButtonEvents.GetComponent<UnityEngine.EventSystems.EventSystem>().sendNavigationEvents = false;
            
            if (UsedCamera) {
                firstStart = false;
                restart();
            }
        }

        void Update() {
            if (cameraTargetingActive)
                positionSystemInFrontOfCamera();

            if (!UsedCamera)
                initializeCamera();

            if (firstStart && UsedCamera) {
                restart();
                firstStart = false;
            }
        }

        #region easy access
        public static void RegisterOnPlayFunction(System.Action OnPlay) {
            Instance.PlayPressed += OnPlay;
        }

        public static void RegisterOnMenuItemSelectedFunction(System.Action<string> OnMenuItemSelected) {
            Instance.MenuItemSelected += OnMenuItemSelected;
        }

        public static void Restart() {
            Instance.restart();
        }

        public static void BackToMainMenu() {
            Instance.restart();
            Instance.backToMainMenu();
        }

        public static void BackToExternalCameraView() {
            Instance.restart();
            Instance.backToExternalCameraView();
        }

        public static void AddLanguageToOptions(string languageID) {
            UIManager.Instance.RegisterMenuItem("ica_lang_" + languageID, "LANG_" + languageID, "ica_languages", true);
        }

        public static void RegisterMenuItem(string Id, string Title, string Parent) {
            UIManager.Instance.RegisterMenuItem(Id, Title, Parent);
        }
        public static void RegisterPlayMenuItem(string Id, string Title) {
            UIManager.Instance.RegisterMenuItem(Id, Title, "ica_play");
        }
        public static void RegisterOptionsMenuItem(string Id, string Title) {
            UIManager.Instance.RegisterMenuItem(Id, Title, "ica_options", true);
        }
        public static void RegisterUnlistedMenuItem(string Id, string Title) {
            UIManager.Instance.RegisterUnlistedMenuItem(Id, Title);
        }

        public static void OpenMenu(string Id) {
            UIManager.Instance.OpenMenu(Id);
        }
        public static void OpenMainMenu() {
            UIManager.Instance.OpenMenu();
        }
        public static void OpenOptionsMenu() {
            UIManager.Instance.OpenMenu("ica_options");
        }

        public static void CloseUI() {
            Instance.closeUI();
        }
        #endregion

        internal void ControllerLost(IInputDevice device) {
            showControllerLostScreen();
        }

        internal void ControllerReconnecting(DeviceManager.reconnectingDeviceState state){
            if (state == DeviceManager.reconnectingDeviceState.ReconnectStarted) ReconnectControllerScreen.Show();
            else ReconnectControllerScreen.Hide();
        }

        internal void OnControllerLostScreenClosed() {
            ControllerSelect.Show();
            if (monitorController != null)
                monitorController.setScreenId(2);
        }

        internal void OnWarningScreenClosed() {
            ControllerSelect.clear();
            DeviceManager.Instance.Rescan();
            CompanyLogoScreen.Show();
            if (monitorController != null)
                monitorController.setScreenId(1);
        }

        internal void OnCompanyLogoDisplayFinished() {
            ControllerSelect.Show();
            if (monitorController != null)
                monitorController.setScreenId(2);
        }

        internal void OnControllerSelected(IInputDevice device) {
            DeviceManager.Instance.UseDevice(device);
            WaitForController.Show();
            if (monitorController != null)
                monitorController.setScreenIdConditional(3, 2);
        }

        internal void OnUserSeated() {
            if (userSeated)
                return;
            
            submitAsAny = false;

            GazeEvents.SetActive(false);
            ButtonEvents.SetActive(true);
            UIManager.Instance.eventSystem = ButtonEvents.GetComponent<UnityEngine.EventSystems.EventSystem>();

            CalibrationScreen.Hide();
            if (monitorController != null)
                monitorController.setScreenId(6);
            OpenMainMenu();
            userSeated = true;

            CameraPositionCalibrated();
        }

        internal void DeviceRunning(IInputDevice device) {
            device.FirstButtonPressed += OnUISubmitButtonPressed;
            device.FirstButtonReleased += OnUISubmitButtonReleased;

            device.SecondButtonPressed += OnUIPreviousButtonPressed;
            device.ThirdButtonPressed += OnUINextButtonPressed;

            device.FirstButtonPressed += OnUIAnyButtonPressed;
            device.SecondButtonPressed += OnUIAnyButtonPressed;
            device.ThirdButtonPressed += OnUIAnyButtonPressed;
            device.FourthButtonPressed += OnUIAnyButtonPressed;

            WaitForController.Hide();
            showExternalCameraScreen();
        }

        internal void OnUIAnyButtonPressed() {
            if (!submitAsAny)
                return;

            if (userMoving) {
                CalibrationScreen.Show(1);
                if (monitorController != null)
                    monitorController.setScreenId(5);
                userMoving = false;
                return;
            }

            OnUserSeated();
        }

        internal void OnUIPreviousButtonPressed() {
            UIManager.Instance.selectPreviousButton();
        }
        
        internal void OnUINextButtonPressed() {
            UIManager.Instance.selectNextButton();
        }

        internal void OnUISubmitButtonPressed() {
            if (submitAsAny)
                return;

            DeviceSubmitButtonPressed = true;
            DeviceSubmitButtonReleased = false;

            UIManager.Instance.submitButtonPressed();
        }

        internal void OnUISubmitButtonReleased() {
            if (submitAsAny)
                return;

            DeviceSubmitButtonReleased = true;
            DeviceSubmitButtonPressed = false;
        }

        internal void OnUIMenuButtonClicked(string id) {
            MenuItemSelected(id);

            switch (id) {
                case "backToRoot":
                    OpenMainMenu();
                    break;
                case "backToOptions":
                    OpenOptionsMenu();
                    break;
                case "ica_play":
                    PlayPressed();
                    break;
                default:
                    if (id.StartsWith("ica_lang_")) {
                        string lang = id.Substring(9, 2);
                        LocalizationManager.SetLanguage(lang);
                        OpenOptionsMenu();
                    }
                    break;
            }
        }

        internal void resyncButtons(List<string> toSync) {
            if (monitorController != null)
                monitorController.setButtonSyncList(toSync);
        }

        private void closeUI() {
            UIManager.Instance.CloseMenu();
            ExternalCameraScreen.Hide();
            if (monitorController != null)
                monitorController.setScreenId(9);

            ButtonEvents.SetActive(false);
            GazeEvents.SetActive(true);

            if (cameraTargetingActive)
                revertCamera();
            Instance.cameraTargetingActive = false;
        }

        private void showExternalCameraScreen() {
            setCameraToRWT();
            Instance.Crosshair.SetActive(false);
            submitAsAny = true;
            userSeated = false;
            userMoving = true;
            ExternalCameraScreen.Show();
            CalibrationScreen.Show(0);
            if (monitorController != null)
                monitorController.setScreenId(4);
        }

        private void enableCameraTargetting() {
            if (!cameraTargetingActive)
                tintCamera();
            cameraTargetingActive = true;
        }

        private void restart() {
            enableCameraTargetting();

            if (monitorController != null)
                monitorController.setScreenId(0);

            WarningScreen.Show();
            CompanyLogoScreen.Hide();
            ControllerSelect.Hide();
            ExternalCameraScreen.Hide();
            CalibrationScreen.Hide();
            WaitForController.Hide(true);
            ReconnectControllerScreen.Hide();
            ControllerLostScreen.Hide();
            NoBluetoothScreen.Hide();
            UIManager.Instance.CloseMenu();
            
            Crosshair.SetActive(true);

            ButtonEvents.SetActive(false);
            GazeEvents.SetActive(true);
        }

        private void showControllerLostScreen() {
            restart();
            WarningScreen.Hide();
            ControllerSelect.clear();
            DeviceManager.Instance.Rescan();
            ControllerLostScreen.Show();

            if (monitorController != null)
                monitorController.setScreenId(7);
        }

        internal void showNoBluetoothScreen() {
            restart();
            WarningScreen.Hide();
            NoBluetoothScreen.Show();
            if (monitorController != null)
                monitorController.setScreenId(8);
        }

        private void backToExternalCameraView() {
            WarningScreen.Hide();
            showExternalCameraScreen();
        }

        private void backToMainMenu() {
            backToExternalCameraView();
            OnUserSeated();
        }

        private void positionSystemInFrontOfCamera() {
            float camY = UsedCamera.transform.rotation.eulerAngles.y;

            if (camY < 270 && camY > 90) {
                transform.position = UsedCamera.transform.position + new Vector3(0f, 0f, -viewDistance);
                transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            } else {
                transform.position = UsedCamera.transform.position + new Vector3(0f, 0f, viewDistance);
                transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            }
        }

        private void tintCamera() {
            outsideOfMenuFlags = UsedCamera.clearFlags;
            outsideOfMenuColor = UsedCamera.backgroundColor;
            outsideOfMenuCullingMask = UsedCamera.cullingMask;
            UsedCamera.clearFlags = CameraClearFlags.SolidColor;
            UsedCamera.backgroundColor = MenuCameraBackgroundColor;
            UsedCamera.cullingMask = MenuCameraCullingMask;
        }

        private void setCameraToRWT() {
            UsedCamera.clearFlags = CameraClearFlags.Depth;
        }
        private void undoRWT() {
            UsedCamera.clearFlags = CameraClearFlags.SolidColor;
        }

        private void revertCamera() {
            UsedCamera.clearFlags = outsideOfMenuFlags;
            UsedCamera.backgroundColor = outsideOfMenuColor;
            UsedCamera.cullingMask = outsideOfMenuCullingMask;
        }

        private void initializeComponents() {
            WarningScreen = GetComponentInChildren<UIWarningScreen>();
            ControllerSelect = GetComponentInChildren<UIControllerSelect>();
            CompanyLogoScreen = GetComponentInChildren<UICompanyLogo>();
            ExternalCameraScreen = GetComponentInChildren<UIExternalCameraScreen>();
            WaitForController = GetComponentInChildren<UIWaitForController>();
            CalibrationScreen = GetComponentInChildren<UICalibrationScreen>();
            ReconnectControllerScreen = GetComponentInChildren<UIReconnectController>();
            ControllerLostScreen = GetComponentInChildren<UIControllerLost>();
            NoBluetoothScreen = GetComponentInChildren<UINoBluetooth>();
            monitorController = GetComponentInChildren<MonitorUI>();
        }

        private void initializeMenuStructure() {
            UIManager.Instance.RegisterMenuItem("ica_play", "PLAY");

            UIManager.Instance.RegisterMenuItem("ica_options", "OPTIONS");
            UIManager.Instance.RegisterMenuItem("ica_languages", "LANGUAGES", "ica_options");
            UIManager.Instance.RegisterMenuItem("backToRoot", "BACK", "ica_options");
            UIManager.Instance.RegisterMenuItem("backToOptions", "BACK", "ica_languages");

            AddLanguageToOptions("EN");
        }

        private void initializeCamera() {
            if (!UsedCamera)
                UsedCamera = Camera.main;

            if (!UsedCamera)
                return;

            if (CrosshairTemplate != null) {
                Crosshair = Instantiate(CrosshairTemplate);
                Crosshair.transform.SetParent(UsedCamera.transform, false);
                Crosshair.SetActive(false);
            }
        }

        
    }
}
