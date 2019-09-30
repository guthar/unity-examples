using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;

namespace Icaros.Mobile.Input {
    public class JavaBridgeManager : MonoBehaviour, IControllerManager{
        private const int reconnectTimeout = 5000; //in ms

        internal static JavaBridgeManager Instance = null;
        
        public Dictionary<string, string> devices;

        #region Controller Manager Interface
        public event Action<byte[]> OnNewBytes = delegate { };
        public event Action<string> OnControllerFound = delegate { };
        public event Action OnControllerConnected = delegate { };
        public event Action OnControllerReconnecting = delegate { };
        public event Action OnControllerDisconnected = delegate { };
        public event Action<string> OnDebugMessage = delegate { };
        public event Action<DeviceManager.errorCode> OnFailedWithError = delegate { };

        private AndroidJavaObject javaBridge;
        private bool initialized = false;

        void Awake() {
            if (Instance != null) {
                Destroy(this.gameObject);
                return;
            }

            Instance = this;
        }
        
        public void Disconnect() {
            javaBridge.Call("disconnectDevice");
        }

        public void ConnectToController(string name) {
            if (devices.ContainsKey(name)) {
                javaBridge.Call("connectToDevice", devices[name]);
            } else {
                Debug.LogError("Device name not found: " + name);
            }
        }

        public void StartScan() {
            devices.Clear();
            javaBridge.Call("startScan");
        }

        public void StopScan() {
            javaBridge.Call("stopScan");
        }
        
        public void Initialize() {
            if (initialized) return;

            javaBridge = new AndroidJavaObject("com.icaros.JavaBridge", reconnectTimeout);
            devices = new Dictionary<string, string>();
            initialized = true;
        }

        void Update() {
            if (!initialized) return;

            byte[] bytes = javaBridge.Get<byte[]>("lastBytes");
            if (bytes.Length >= 20) {
                OnNewBytes(bytes);
            }
        }

        public void CleanUp() {
            StopScan();
            Disconnect();
        }
        #endregion

        #region Java Bridge Implementation
        public void OnDeviceFound(String peripheralNameAndAddress) {
            string[] parts = peripheralNameAndAddress.Split('|');
            string name = parts[0];
            string address = parts[1];
            devices.Add(name, address);

            OnControllerFound(name);
        }

        public void OnDeviceConnected() {
            OnControllerConnected();
        }

        public void OnDeviceDisconnected() {
            OnControllerDisconnected();
        }

        public void WaitForDevice() {
            OnControllerReconnecting();
        }

        public void FailedWithError(string error) {
            switch (error) {
                case "NoBluetooth":
                    OnFailedWithError(DeviceManager.errorCode.NoBluetooth);
                    break;
                case "CouldNotConnect":
                    OnFailedWithError(DeviceManager.errorCode.CouldNotConnect);
                    break;
                default:
                    break;
            }
        }

        public void DebugMessage(string msg) {
            OnDebugMessage(msg);
        }
        #endregion
    }
}