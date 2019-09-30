using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Icaros.Mobile.Input {
    public class DeviceManager : MonoBehaviour {
        #region enums
        public enum errorCode { NoBluetooth, CouldNotConnect }
        public enum reconnectingDeviceState {
            ReconnectStarted, ReconnectFailed, ReconnectSucceeded }
        #endregion

        #region device types
        public const string ICAROS_CONTROLLER_DEVICE_ID = "ICAROS_CONTROLLER";
        public const string KEYBOARD_DEVICE_ID = "KEYBOARD";
        public const string JOYSTICK_DEVICE_ID = "JOYSTICK";
        #endregion

        #region UnityInputManager mapping
        public const string INPUT_FIRST_BUTTON = "ICAROS_Button1";
        public const string INPUT_SECOND_BUTTON = "ICAROS_Button2";
        public const string INPUT_THIRD_BUTTON = "ICAROS_Button3";
        public const string INPUT_FOURTH_BUTTON = "ICAROS_Button4";

        public const string INPUT_X_AXIS = "ICAROS_xAxis";
        public const string INPUT_Y_AXIS = "ICAROS_yAxis";
        public const string INPUT_Z_AXIS = "ICAROS_zAxis";
        #endregion

        public static DeviceManager Instance = null;

        public event System.Action<IInputDevice> NewDeviceRegistered = delegate { };
        public event System.Action<IInputDevice> DeviceLost = delegate { };
        public event System.Action<reconnectingDeviceState> DeviceReconnecting = delegate { };
        public event System.Action<IInputDevice> DeviceUsed = delegate { };
        public event System.Action<string> OnDebugMessage = delegate { };

        List<IInputDevice> registeredInputDevices = new List<IInputDevice>();
        bool initialized = false;

        private IControllerManager ControllerManager = null;
        private IcarosController icarosControllerOnHold = null;
        private IcarosController icarosControllerInUse = null;

        private bool receivedControllerPairedEvent = false;
        private bool receivedControllerLostEvent = false;
        private bool receivedControllerReconnectingEvent = false;
        private bool noBluetoothMessage = false;

        void Awake() {
            if (Instance != null) {
                Destroy(this.gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        
        void init() {
            try {
#if !UNITY_ANDROID || UNITY_EDITOR
                ControllerManager = SPPManager.Instance;
#endif
#if UNITY_ANDROID && !UNITY_EDITOR
                ControllerManager = JavaBridgeManager.Instance;
#endif
                if (ControllerManager != null) {
                    ControllerManager.Initialize();
                    ControllerManager.OnControllerFound += IcarosControllerFound;
                    ControllerManager.OnControllerConnected += IcarosControllerPaired;
                    ControllerManager.OnControllerReconnecting += IcarosControllerReconnecting;
                    ControllerManager.OnControllerDisconnected += IcarosControllerDisconnected;
                    ControllerManager.OnDebugMessage += ControllerManagerDebugMessage;
                    ControllerManager.OnFailedWithError += ControllerManagerFailedWithError;
                }

            } catch (System.Exception e) {
                Debug.Log(e.Message);
            }
        }

        void Update() {
            if (!initialized) {
                init();
                initialized = true;
            }
            
            if (noBluetoothMessage) {
                UI.UISystem.Instance.showNoBluetoothScreen();
                noBluetoothMessage = false;
                return;
            }

            if (receivedControllerPairedEvent) {
                handleControllerPaired();
                receivedControllerPairedEvent = false;
            } else if (receivedControllerLostEvent) {
                handleControllerLost();
                receivedControllerLostEvent = false;
            } else if (receivedControllerReconnectingEvent) {
                handleControllerReconnecting();
                receivedControllerReconnectingEvent = false;
            }

            foreach (IInputDevice device in registeredInputDevices) {
                if (device.IsInUse())
                    device.Update();
            }
        }

        internal void Rescan() {
            registeredInputDevices.Clear();
#if UNITY_EDITOR
            registerDevice(new Keyboard());
#endif
#if UNITY_ANDROID && !UNITY_EDITOR
            registerDevice(new Keyboard() { deviceName = "Gear VR Touchpad" });
#endif

            foreach (string jname in UnityEngine.Input.GetJoystickNames()) {
                GenericJoystick joystick = new GenericJoystick();
                joystick.deviceName = jname;
                registerDevice(joystick);
            }

            if (icarosControllerInUse != null && ControllerManager != null)
                ControllerManager.Disconnect();

            if (ControllerManager != null)
                ControllerManager.StartScan();
        }

        public void UseDevice(IInputDevice device) {
            device.Initialize();

            if (device.GetType() == typeof(IcarosController)) {
                IcarosController con = device as IcarosController;
                icarosControllerOnHold = con;
                ControllerManager.ConnectToController(con.GetDeviceName());
            }
            if (device.GetType() == typeof(Keyboard)) {
                Keyboard key = device as Keyboard;
                key.used = true;
                DeviceUsed(device);
            }
            if (device.GetType() == typeof(GenericJoystick)) {
                GenericJoystick joy = device as GenericJoystick;
                joy.used = true;
                DeviceUsed(device);
            }

        }

        public IInputDevice[] GetRegisteredDevices() {
            IInputDevice[] devices = new IInputDevice[registeredInputDevices.Count];
            registeredInputDevices.CopyTo(devices);
            return devices;
        }

        public List<IInputDevice> GetUsedDevices() {
            List<IInputDevice> devices = new List<IInputDevice>();
            foreach (IInputDevice device in registeredInputDevices) {
                if (device.IsInUse())
                    devices.Add(device);
            }
            return devices;
        }

        void registerDevice(IInputDevice device) {
            registeredInputDevices.Add(device);
            NewDeviceRegistered(device);
        }

        void handleControllerPaired() {
            if (icarosControllerInUse != null) {
                DeviceReconnecting(reconnectingDeviceState.ReconnectSucceeded);
                return;
            }

            if (icarosControllerOnHold == null) {
                OnDebugMessage("Controller Connection failed");
                return;
            }
            ControllerManager.OnNewBytes += icarosControllerOnHold.HandleNewBytes;
            icarosControllerOnHold.used = true;
            DeviceUsed(icarosControllerOnHold);
            icarosControllerInUse = icarosControllerOnHold;
            icarosControllerOnHold = null;
        }

        void handleControllerLost() {
            if (icarosControllerInUse != null) {
                icarosControllerInUse.used = false;
                registeredInputDevices.Remove(icarosControllerInUse);
                DeviceLost(icarosControllerInUse);
                DeviceReconnecting(reconnectingDeviceState.ReconnectFailed);
                icarosControllerInUse = null;
            }

            if (icarosControllerOnHold != null) {
                registeredInputDevices.Remove(icarosControllerOnHold);
                DeviceLost(icarosControllerOnHold);
                icarosControllerOnHold = null;
            }
        }

        void handleControllerReconnecting() {
            if (icarosControllerInUse == null) return;

            DeviceReconnecting(reconnectingDeviceState.ReconnectStarted);
        }

        private void OnDestroy() {
            if (ControllerManager != null) {
                OnDebugMessage("Cleanup OD");
                ControllerManager.CleanUp();
                ControllerManager = null;
                OnDebugMessage("Cleanup done");
            }
        }

        private void OnApplicationQuit() {
            if (ControllerManager != null) {
                OnDebugMessage("Cleanup APPX");
                ControllerManager.CleanUp();
                ControllerManager = null;
                OnDebugMessage("Cleanup done");
            }
        }

        #region Icaros Controller
        private void IcarosControllerFound(string name) {
            IcarosController con = new IcarosController();
            con.deviceName = name;
            registerDevice(con);
        }

        private void IcarosControllerPaired() {
            receivedControllerPairedEvent = true;
        }

        private void IcarosControllerReconnecting() {
            receivedControllerReconnectingEvent = true;
        }

        private void IcarosControllerDisconnected() {
            receivedControllerLostEvent = true;
        }
         
        private void ControllerManagerDebugMessage(string msg) {
            OnDebugMessage(msg);
        }   

        private void ControllerManagerFailedWithError(errorCode code) {
            switch (code) {
                case errorCode.NoBluetooth:
                    noBluetoothMessage = true;
                    break;
                case errorCode.CouldNotConnect:
                    IcarosControllerDisconnected();
                    break;
                default:
                    break;
            }
            OnDebugMessage("Failed with error: " + code);
        }
        #endregion

    }
}