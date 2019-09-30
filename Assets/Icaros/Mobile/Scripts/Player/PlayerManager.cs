using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Icaros.Mobile.Input;
using Icaros.Mobile.UI;

namespace Icaros.Mobile.Player {
    public class PlayerManager : MonoBehaviour {
        
        public static PlayerManager Instance = null;

        void Awake() {
            if (Instance != null) {
                Destroy(this.gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(this.gameObject);

            disc = GetComponentInChildren<IcarosDiscovery>();
            net = GetComponentInChildren<IcarosNetwork>();
        }
        
        public enum State { Player, Mirror }

        public State state = State.Player;
        
        public GameObject PlayerTemplate;
        public GameObject VirtualRealityCameraRig;
        public GameObject SpectatorCameraRig;

        public int GameKey = 1111;
        public int GameVersion = 0;
        public int GameSubVersion = 1;
        
        private LocalPlayer localPlayer = null;

        private IcarosDiscovery disc;
        private IcarosNetwork net;

        private List<GameObject> temporaryAddons = new List<GameObject>();

        public void addTemporaryObject(GameObject obj) {
            temporaryAddons.Add(obj);
            obj.transform.SetParent(localPlayer.Head.transform, false);
        }

        public void registerPlayer(LocalPlayer player) {
            this.localPlayer = player;
            GameObject model = Instantiate(PlayerTemplate, localPlayer.Vehicle.transform);
            
            GameObject myCam;
            if (player.isLocalPlayer) {
                myCam = Instantiate(VirtualRealityCameraRig, localPlayer.Camera.transform);
            } else {
                myCam = Instantiate(SpectatorCameraRig, localPlayer.Head.transform);
            }
            myCam.transform.localPosition = Vector3.zero;

            UISystem.Instance.CameraPositionCalibrated += player.recenterCamera;

            player.transform.SetParent(transform, true);
        }

        public void LinkController(IInputDevice device) {
            localPlayer.linkController(device);
        }

        public LocalPlayer GetLocalPlayer() {
            return localPlayer;
        }

        public void ChangeScene(string name) {
            ClearAddons();
            net.ServerChangeScene(name);
           
        }

        public void ClearAddons() {
            foreach (GameObject obj in temporaryAddons) {
                Destroy(obj);
            }
            temporaryAddons.Clear();
        }

        private void Start() {
            initializeNetwork();
        }

        internal void connectToServer(string address) {
            net.networkAddress = address;
            net.StartClient();
        }

        internal void reconnect() {
            UISystem.Instance.monitorLoadingScreen.SetActive(true);
            net.StopClient();
            net.enabled = false;
            Debug.Log("reconnect!");
            StartCoroutine(timedReconnect());
        }

        IEnumerator timedReconnect() {
            yield return new WaitForSecondsRealtime(1.0f);
            //UnityEngine.SceneManagement.SceneManager.LoadScene("menu");
            net.enabled = true;
            disc.enabled = true;
            disc.Initialize();
            disc.StartAsClient();
        }

        private void initializeNetwork() {
            if (disc == null || net == null)
                return;

            disc.broadcastKey = GameKey;
            disc.broadcastVersion = GameVersion;
            disc.broadcastSubVersion = GameSubVersion;

            if (state == State.Mirror) {
                disc.Initialize();
                disc.StartAsClient();
            } else {
                disc.Initialize();
                disc.StartAsServer();
                net.StartHost();
            }
        }
    }
}