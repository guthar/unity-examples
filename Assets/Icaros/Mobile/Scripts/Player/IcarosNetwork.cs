using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Icaros.Mobile.Player {

    public class IcarosNetwork : NetworkManager {

        private bool spawned = false;

        public override void OnClientDisconnect(NetworkConnection conn) {
            base.OnClientDisconnect(conn);
            PlayerManager.Instance.reconnect();
        }

        public override void OnServerDisconnect(NetworkConnection conn) {
            base.OnServerDisconnect(conn);
        }

        public override void OnClientError(NetworkConnection conn, int errorCode) {
            base.OnClientError(conn, errorCode);
            PlayerManager.Instance.reconnect();
        }

        public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId) {
            if (spawned)
                return;

            base.OnServerAddPlayer(conn, playerControllerId);
            spawned = true;
        }
    }
}