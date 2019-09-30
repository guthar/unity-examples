using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Icaros.Mobile.Player
{
    public class IcarosDiscovery : UnityEngine.Networking.NetworkDiscovery
    {
        private bool end = false;

        public override void OnReceivedBroadcast(string fromAddress, string data)
        {
            PlayerManager.Instance.connectToServer(fromAddress);
            end = true;
        }

        private void LateUpdate()
        {
            if (end)
            {
                StopBroadcast();
                end = false;
                enabled = false;
            }
        }
    }
}