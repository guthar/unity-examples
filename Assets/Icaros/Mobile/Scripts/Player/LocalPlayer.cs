﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Icaros.Mobile.Input;
using System;
using UnityEngine.Networking;

namespace Icaros.Mobile.Player {
    public class LocalPlayer : NetworkBehaviour {
        public GameObject Camera;
        public GameObject Head;
        public GameObject Vehicle;

        //Current Movementspeed
        public float MoveSpeed = 0f;
        //Acceleration per Tick
        public float Acceleration = 5f;

        //Multiplies the calculation result for rolls (z-axis rotation)
        public float RollRotationFactor = 2f;
        //Multiplies the calculation result for pitch (x-axis rotation) 
        public float PitchRotationFactor = 1.5f;
        
        private IInputDevice myController = null;
        private CharacterController cc;
        private float currentZ = 0f;

        private Transform camToFollow = null;
        
        public void linkController(IInputDevice device) {
            if (!isLocalPlayer)
                return;

            myController = device;
            device.SecondButtonPressed += speedUp;
            device.ThirdButtonPressed += slowDown;
            device.xAxisRotated += rotateX;
            device.zAxisRotated += rotateZ;
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
            cc = GetComponent<CharacterController>();
            PlayerManager.Instance.registerPlayer(this);
        }

        void speedUp() {
            MoveSpeed += Acceleration;
        }

        void slowDown() {
            MoveSpeed -= Acceleration;
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
            
            if (MoveSpeed < 0)
                euler.y += -currentZ * -RollRotationFactor * Time.deltaTime;
            else
                euler.y += currentZ * -RollRotationFactor * Time.deltaTime;

            transform.localEulerAngles = euler;
            cc.Move(transform.forward * MoveSpeed * Time.deltaTime);

            updateBody();
        }

        void updateBody() {
            if (camToFollow == null) {
                Camera[] cams = GetComponentsInChildren<Camera>();
                foreach(Camera c in cams) {
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
            }catch (Exception e) {
                camToFollow = null;
            }

        }
    }
}