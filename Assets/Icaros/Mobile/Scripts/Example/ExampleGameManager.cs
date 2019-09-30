using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Icaros.Mobile.Input;
using Icaros.Mobile.UI;

namespace Icaros.Mobile.Player {
    public class ExampleGameManager : MonoBehaviour {

        private IInputDevice device;

        void Start() {
            //Don't do anything if this is a monitor app
            if (PlayerManager.Instance.state == PlayerManager.State.Mirror) {
                return;
            }

            //We want to be informed when the player has chosen their input device.
            DeviceManager.Instance.DeviceUsed += ControllerChosen;
            //To pause the game on controller reconnect attempts, we also listen to this event:
            DeviceManager.Instance.DeviceReconnecting += ControllerReconnecting;
            //And we want to know when the player wants to start the game.
            UISystem.RegisterOnPlayFunction(PlayPressed);
        }

        public void ControllerChosen(IInputDevice device) {
            //This is the chosen device. We don't link it yet because we don't want the player to randomly fly around while in the menu.
            this.device = device;
        }

        public void PlayPressed() {
            //Link the device to the default player.
            PlayerManager.Instance.LinkController(device);
            //We don't need the main menu anymore, so we're going to close it.
            UISystem.CloseUI();
            //In case we paused the game when we lost a previous controller
            Time.timeScale = 1;
        }

        public void ControllerReconnecting(DeviceManager.reconnectingDeviceState state) {
            switch (state) {
                case DeviceManager.reconnectingDeviceState.ReconnectStarted:
                    //Pause ingame time while waiting for the controller to reconnect.
                    Time.timeScale = 0;
                    break;
                case DeviceManager.reconnectingDeviceState.ReconnectFailed:
                    //Returning timeScale to normal will be handled by 'PlayPressed' after the player selects a new controller. This ensures that the game won't continue while returned to the menu screens.
                    break;
                case DeviceManager.reconnectingDeviceState.ReconnectSucceeded:
                    //Reconnect successfull -> time can continue to flow.
                    Time.timeScale = 1;
                    break;
                default:
                    break;
            }
        }
    }
}