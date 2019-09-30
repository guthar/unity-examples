using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Icaros.Mobile.Input {
    public class SPPManager : MonoBehaviour, IControllerManager {

        internal static SPPManager Instance = null;

        private bool initialized = false;

        #region Controller Manager Interface
        public event Action<byte[]> OnNewBytes = delegate { };
        public event Action<string> OnControllerFound = delegate { };
        public event Action OnControllerConnected = delegate { };
        public event Action OnControllerReconnecting = delegate { };
        public event Action OnControllerDisconnected = delegate { };
        public event Action<string> OnDebugMessage = delegate { };
        public event Action<DeviceManager.errorCode> OnFailedWithError = delegate { };

        void Awake() {
            if (Instance != null) {
                Destroy(this.gameObject);
                return;
            }

            Instance = this;
        }

        public void Initialize() {
#if !UNITY_ANDROID || UNITY_EDITOR
            Bluetooth.Manager.OnDebugMessage += debug;
            Bluetooth.Manager.OnControllerFound += controllerFound;
            Bluetooth.Manager.OnControllerLost += controllerLost;
            Bluetooth.Manager.OnCouldNotConnect += couldNotConnect;
            Bluetooth.Manager.OnPaired += controllerPaired;
            Bluetooth.Manager.Initialize();
#endif
            initialized = true;
        }

        public void ConnectToController(string name) {
#if !UNITY_ANDROID || UNITY_EDITOR
            Bluetooth.Manager.PairController(name);
#endif
        }

        public void StartScan() {
#if !UNITY_ANDROID || UNITY_EDITOR
            Bluetooth.Manager.StartScan();
#endif
        }

        public void StopScan() {
#if !UNITY_ANDROID || UNITY_EDITOR
            Bluetooth.Manager.StopScan();
#endif
        }

        public void Disconnect() {
#if !UNITY_ANDROID || UNITY_EDITOR
            Bluetooth.Manager.CloseConnection();
#endif
        }

        public void CleanUp() {
#if !UNITY_ANDROID || UNITY_EDITOR
            Bluetooth.Manager.CloseConnection();
#endif
        }
        #endregion

        #region Icaros.Bluetooth Implementation
        private void debug(string msg) {
            OnDebugMessage(msg);
        }

        private void controllerFound(string name) {
            OnControllerFound(name);
        }

        private void controllerLost() {
            OnControllerDisconnected();
        }

        private void couldNotConnect() {
            OnFailedWithError(DeviceManager.errorCode.CouldNotConnect);
        }

        private void controllerPaired() {
            OnControllerConnected();
        }

        void Update() {
            if (!initialized) return;

#if !UNITY_ANDROID || UNITY_EDITOR
            byte[] bytes = Bluetooth.Manager.getCurrentPacket();
            if (bytes.Length >= 20) {
                OnNewBytes(bytes);
            }
#endif
        }
        #endregion
    }
}