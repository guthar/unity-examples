using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Icaros.Mobile.Input {
    public interface IControllerManager {
        event System.Action<byte[]> OnNewBytes;
        event System.Action<string> OnControllerFound;
        event System.Action OnControllerConnected;
        event System.Action OnControllerReconnecting;
        event System.Action OnControllerDisconnected;
        event System.Action<string> OnDebugMessage;
        event System.Action<DeviceManager.errorCode> OnFailedWithError;

        void Initialize();
        void StartScan();
        void StopScan();
        void ConnectToController(string name);
        void Disconnect();
        void CleanUp();
    }
}
