using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Icaros.Mobile.Input;
using System;
using UnityEngine.Networking;

namespace Icaros.Mobile.Player {
    public class LocalPlayer : NetworkBehaviour {
        private const string MoneyObjectTag = "MoneyObject";
        private const string WaterTag = "Water";

        public GameObject Camera;
        public GameObject Head;
        public GameObject Vehicle;

        //Current Movementspeed
        public float MoveSpeed = 70f;

        //Multiplies the calculation result for rolls (z-axis rotation)
        public float RollRotationFactor = 2f;
        //Multiplies the calculation result for pitch (x-axis rotation) 
        public float PitchRotationFactor = 1.5f;

        private IInputDevice myController = null;
        private Rigidbody rb;
        private float currentZ = 0f;

        private Transform camToFollow = null;

        private bool ready = false;

        public void linkController(IInputDevice device) {
            if (!isLocalPlayer)
                return;

            myController = device;
            //device.SecondButtonPressed += speedUp;
            //device.ThirdButtonPressed += slowDown;
            device.xAxisRotated += rotateX;
            device.zAxisRotated += rotateZ;

            ready = true;
        }

        public void recalibrate() {
            if (myController == null)
                return;

            if (myController.GetDeviceTypeID() == DeviceManager.ICAROS_CONTROLLER_DEVICE_ID) {
                IcarosController con = myController as IcarosController;
                con.recalibrateFor(IcarosController.Orientation.Icaros);
            }
        }

        public void recenterCamera() {
            UnityEngine.XR.InputTracking.Recenter();
        }

        void Start() {
            rb = GetComponent<Rigidbody>();
            PlayerManager.Instance.registerPlayer(this);
        }

        void rotateX(float x) {
            Vector3 euler = transform.localEulerAngles;
            euler.x = Mathf.Min(Mathf.Max(x * PitchRotationFactor, -65.0f), 65.0f);
            transform.localEulerAngles = euler;
        }

        void rotateZ(float z) {
            Vehicle.transform.localEulerAngles = new Vector3(0, 0, z);
            currentZ = z;
        }

        void Update() {
            if (!isLocalPlayer)
                return;


            Vector3 euler = transform.localEulerAngles;

            if (MoveSpeed < 0) {
                euler.y += -currentZ * -RollRotationFactor * Time.deltaTime;
            }
            else {
                euler.y += currentZ * -RollRotationFactor * Time.deltaTime;
            }

            transform.localEulerAngles = euler;

            updateBody();
        }

        private void FixedUpdate() {
            if (ready) {
                // Unter Wasser
                // TODO: "underWater" verwenden
                if (transform.position.y < 20f) {
                    rb.useGravity = false;
                    rb.drag = 0.7f;

                    rb.AddForce(transform.forward * MoveSpeed * 80 * Time.deltaTime);
                }
                // Über Wasser
                else {
                    rb.useGravity = true;
                    rb.drag = 0f;
                }
            }
        }

        void updateBody() {
            if (camToFollow == null) {
                Camera[] cams = GetComponentsInChildren<Camera>();
                foreach (Camera c in cams) {
                    if (c.CompareTag("MainCamera")) {
                        camToFollow = c.transform;
                    }
                }

                return;
            }

            try {
                Head.transform.localRotation = camToFollow.localRotation;
                Head.transform.localPosition = camToFollow.localPosition;
                Vehicle.transform.position = camToFollow.transform.position;
            }
            catch (Exception e) {
                camToFollow = null;
            }

        }


        // ****************************************
        // HechtManager
        // ****************************************

        /// <summary>
        /// Instanz des <see cref="AtlentosManager"/>-Spielmanagerobjekts.
        /// </summary>
        [Tooltip("Instanz des Atlentos-Spielmanagerobjekts.")]
        public AtlentosManager atlentosManager;

        /// <summary>
        /// Audio Clip, der das aus-dem-Wasser-springen darstellt.
        /// </summary>
        [Tooltip("Ton, der abgespielt wird, wenn der Hecht aus dem Wasser springt.")]
        public AudioClip jumpAudioClip;

        /// <summary>
        /// Audio Clip, der das in-das-Wasser-eintauchen darstellt.
        /// </summary>
        [Tooltip("Ton, der abgespielt wird, wenn der Hecht in das Wasser eintaucht.")]
        public AudioClip diveAudioClip;

        /// <summary>
        /// Aktueller Stand der gesammelten Punkte.
        /// </summary>
        private float currentScore = 0f;

        /// <summary>
        /// Aktuelles Gewicht des gesammelten Geldes.
        /// </summary>
        private float currentWeight = 0f;

        /// <summary>
        /// Kennzeichen, ob der Spieler unter oder über Wasser ist.
        /// </summary>
        private bool underWater = true;

        /// <summary>
        /// Spielt den Ton zum aus-dem-Wasser-springen ab.
        /// </summary>
        private void jump()
        {
            if (jumpAudioClip != null)
            {
                AudioSource.PlayClipAtPoint(jumpAudioClip, gameObject.transform.position);
            }
        }

        /// <summary>
        /// Spielt den Ton zum in-das-Wasser-eintauchen ab.
        /// </summary>
        private void dive()
        {
            if (diveAudioClip != null)
            {
                AudioSource.PlayClipAtPoint(diveAudioClip, gameObject.transform.position);
            }
        }

        /// <summary>
        /// Verarbeitet die Kollision mit anderen Trigger-Objekten
        /// </summary>
        /// <param name="other">[in] Das Objekt, das den Trigger ausgelöst hat.</param>
        private void OnTriggerEnter(Collider other)
        {
            // bestimmen, welches Objekt den Trigger ausgelöst hat
            if (isWater(other.gameObject))
            {
                underWater = true;
                rb.useGravity = false;
                rb.drag = 0.7f;

                dive();
            }
            else if (isMoneyObject(other.gameObject))
            {
                // Geld einsammeln
                collectMoneyObject(other.gameObject);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            // bestimmen, welches Objekt den Trigger ausgelöst hat
            if (isWater(other.gameObject))
            {
                underWater = false;
                rb.useGravity = true;
                rb.drag = 0f;

                jump();
            }
        }

        /// <summary>
        /// Bestimmt, ob es sich um das Wasserobjekt handelt.
        /// </summary>
        /// <param name="other">[in] Objekt, das bestimmt werden soll.</param>
        /// <returns>
        /// <i>true</i> - Es handelt sich um das Wasserobjekt.<br/>
        /// <i>false</i> - Es handelt sich um ein anderes Objekt.
        /// </returns>
        private bool isWater(GameObject other)
        {
            // Objekttyp bestimmen
            bool result = other.CompareTag(WaterTag);
            return result;
        }

        /// <summary>
        /// Bestimmt, ob es sich um ein Geldobjekt handelt.
        /// </summary>
        /// <param name="other">[in] Objekt, das bestimmt werden soll.</param>
        /// <returns>
        /// <i>true</i> - Es handelt sich um ein Geldobjekt.<br/>
        /// <i>false</i> - Es handelt sich um ein anderes Objekt.
        /// </returns>
        private bool isMoneyObject(GameObject other)
        {
            // Objekttyp bestimmen
            bool result = other.CompareTag(MoneyObjectTag);
            return result;
        }

        /// <summary>
        /// Verarbeitet das Einsammeln eines Geldobjekts.
        /// </summary>
        /// <param name="moneyObject">[in] eingesammeltes Geldobjekt</param>
        private void collectMoneyObject(GameObject moneyObject)
        {
            // Wert und Gewicht auslesen
            MoneyObjectProperties moneyObjectProperties = moneyObject.GetComponent<MoneyObjectProperties>();
            if (moneyObjectProperties != null)
            {
                // Ton abspielen
                if (moneyObjectProperties.collectAudioClip)
                {
                    AudioSource.PlayClipAtPoint(moneyObjectProperties.collectAudioClip, gameObject.transform.position);
                }

                // Werte übernehmen
                currentScore += moneyObjectProperties.ScoreValue;
                currentWeight += moneyObjectProperties.weight;

                // Information an Spielmanagerobjekt übergeben
                atlentosManager.CollectMoneyObject(moneyObjectProperties);
            }
        }
    }
}