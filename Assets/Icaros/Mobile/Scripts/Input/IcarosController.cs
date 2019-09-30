using UnityEngine;
using System.Collections;
using System;

namespace Icaros.Mobile.Input {
    public class IcarosController : IInputDevice {

        #region Orientation
        public enum Orientation { Icaros, IcarosR }

        public delegate Quaternion AxesDefinition(float qX, float qY, float qZ, float qW);
        private AxesDefinition transformAxes;

        public IcarosController() {
            transformAxes = axesTransformIcaros;
        }
        #endregion

        #region IInputDeviceImplementation
        public event Action FirstButtonPressed = delegate { };
        public event Action SecondButtonPressed = delegate { };
        public event Action ThirdButtonPressed = delegate { };
        public event Action FourthButtonPressed = delegate { };

        public event Action FirstButtonReleased = delegate { };
        public event Action SecondButtonReleased = delegate { };
        public event Action ThirdButtonReleased = delegate { };
        public event Action FourthButtonReleased = delegate { };

        public event Action<float> xAxisRotated = delegate { };
        public event Action<float> zAxisRotated = delegate { };
        public event Action<float> yAxisRotated = delegate { };
        public event Action<Quaternion> RotationChanged = delegate { };

        public bool FirstButtonDown() {
            return FirstButtonIsDown;
        }

        public bool SecondButtonDown() {
            return SecondButtonIsDown;
        }

        public bool ThirdButtonDown() {
            return ThirdButtonIsDown;
        }

        public bool FourthButtonDown() {
            return FourthButtonIsDown;
        }

        public string GetDeviceTypeID() {
            return deviceType;
        }

        public string GetDeviceName() {
            return deviceName;
        }

        public bool IsInUse() {
            return used;
        }

        public void Update() {
        }

        public void Initialize() {
        }
        #endregion

        private bool FirstButtonIsDown = false;
        private bool SecondButtonIsDown = false;
        private bool ThirdButtonIsDown = false;
        private bool FourthButtonIsDown = false;

        public string deviceType = DeviceManager.ICAROS_CONTROLLER_DEVICE_ID;
        public string deviceName;
        public bool used = false;

        //this is the inverse to the range of difference in angles to be registered
        //as of 09.11.16 the controller doesn't seem to support anything lower then 100 or so
        public float sensitivity = 250.0f;

        public Quaternion rotation = Quaternion.identity;
        public Vector3 eulerAngles = Vector3.zero;
        public float Voltage = 0;

        public void HandleNewBytes(byte[] bytes) {
            int ButtonMask = (int)bytes[1];

            bool newFirstButtonIsDown = (ButtonMask & 1) != 0;

            if (!FirstButtonIsDown && newFirstButtonIsDown)
                FirstButtonPressed();

            if (FirstButtonIsDown && !newFirstButtonIsDown)
                FirstButtonReleased();

            FirstButtonIsDown = newFirstButtonIsDown;


            bool newSecondButtonIsDown = (ButtonMask & 2) != 0;

            if (!SecondButtonIsDown && newSecondButtonIsDown)
                SecondButtonPressed();

            if (SecondButtonIsDown && !newSecondButtonIsDown)
                SecondButtonReleased();

            SecondButtonIsDown = newSecondButtonIsDown;


            bool newThirdButtonIsDown = (ButtonMask & 4) != 0;

            if (!ThirdButtonIsDown && newThirdButtonIsDown)
                ThirdButtonPressed();

            if (ThirdButtonIsDown && !newThirdButtonIsDown)
                ThirdButtonReleased();

            ThirdButtonIsDown = newThirdButtonIsDown;


            bool newFourthButtonIsDown = (ButtonMask & 8) != 0;

            if (!FourthButtonIsDown && newFourthButtonIsDown)
                FourthButtonPressed();

            if (FourthButtonIsDown && !newFourthButtonIsDown)
                FourthButtonReleased();

            FourthButtonIsDown = newFourthButtonIsDown;

            float qX = BitConverter.ToSingle(bytes, 2);
            float qY = BitConverter.ToSingle(bytes, 6);
            float qZ = BitConverter.ToSingle(bytes, 10);
            float qW = BitConverter.ToSingle(bytes, 14);

            Voltage = ((int)bytes[18]) / 10f;

            //use potentially redefined coordinate system or rotations
            rotation = transformAxes(qX, qY, qZ, qW);

            Vector3 newEuler = rotation.eulerAngles;

            if (Mathf.Abs(newEuler.x - eulerAngles.x) > 1 / sensitivity || Mathf.Abs(newEuler.y - eulerAngles.y) > 1 / sensitivity || Mathf.Abs(newEuler.z - eulerAngles.z) > 1 / sensitivity) {
                eulerAngles = newEuler;
                RotationChanged(rotation);

                //transform coordinates to simple +/- 180° differences
                if (eulerAngles.x > 180f)
                    eulerAngles.x -= 360f;
                if (eulerAngles.y > 180f)
                    eulerAngles.y -= 360f;
                if (eulerAngles.z > 180f)
                    eulerAngles.z -= 360f;

                xAxisRotated(eulerAngles.x);
                yAxisRotated(eulerAngles.y);
                zAxisRotated(eulerAngles.z);
            }

        }

        private void calibrate(AxesDefinition definition) {
            transformAxes = definition;
        }

        internal Quaternion axesTransformIcaros(float qX, float qY, float qZ, float qW) {
            return new Quaternion(-0.5f, -0.5f, 0.5f, -0.5f) * new Quaternion(qX, qY, qZ, qW);
        }

        internal Quaternion axesTransformIcarosR(float qX, float qY, float qZ, float qW) {
            return new Quaternion(0.5f, -0.5f, -0.5f, -0.5f) * new Quaternion(-qY, -qX, qZ, -qW);
        }

        public void recalibrateFor(AxesDefinition definition) {
            calibrate(definition);
        }
        public void recalibrateFor(Orientation or) {
            switch (or) {
                case Orientation.Icaros:
                    recalibrateFor(axesTransformIcaros);
                    break;
                case Orientation.IcarosR:
                    recalibrateFor(axesTransformIcarosR);
                    break;
                default:
                    break;
            }
        }

    }
}
