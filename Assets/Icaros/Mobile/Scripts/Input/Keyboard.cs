using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Icaros.Mobile.Input {
    public class Keyboard : IInputDevice {

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

        public void Initialize() {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        #endregion

        private bool FirstButtonIsDown = false;
        private bool SecondButtonIsDown = false;
        private bool ThirdButtonIsDown = false;
        private bool FourthButtonIsDown = false;

        public string deviceType = DeviceManager.KEYBOARD_DEVICE_ID;
        public string deviceName = "Keyboard";
        public bool used = false;

        public float xAngle = 0;
        public float yAngle = 0;
        public float zAngle = 0;
        public Quaternion Rotation = Quaternion.identity;

        public float MaxXAngle = 35;
        public float MaxYAngle = 35;
        public float MaxZAngle = 35;

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
            FirstButtonIsDown = false;
            SecondButtonIsDown = false;
            ThirdButtonIsDown = false;
            FourthButtonIsDown = false;
        }

        public void Update() {
            if (UnityEngine.Input.GetButtonDown(DeviceManager.INPUT_FIRST_BUTTON)) {
                FirstButtonPressed();
                FirstButtonIsDown = true;
            }

            if (UnityEngine.Input.GetButtonUp(DeviceManager.INPUT_FIRST_BUTTON)) {
                FirstButtonReleased();
                FirstButtonIsDown = false;
            }

            if (UnityEngine.Input.GetButtonUp(DeviceManager.INPUT_SECOND_BUTTON)) {
                SecondButtonPressed();
                SecondButtonIsDown = false;
            }

            if (UnityEngine.Input.GetButtonDown(DeviceManager.INPUT_SECOND_BUTTON)) {
                SecondButtonReleased();
                SecondButtonIsDown = true;
            }

            if (UnityEngine.Input.GetButtonDown(DeviceManager.INPUT_THIRD_BUTTON)) {
                ThirdButtonPressed();
                ThirdButtonIsDown = true;
            }

            if (UnityEngine.Input.GetButtonUp(DeviceManager.INPUT_THIRD_BUTTON)) {
                ThirdButtonReleased();
                ThirdButtonIsDown = false;
            }

            if (UnityEngine.Input.GetButtonDown(DeviceManager.INPUT_FOURTH_BUTTON)) {
                FourthButtonPressed();
                FourthButtonIsDown = true;
            }

            if (UnityEngine.Input.GetButtonUp(DeviceManager.INPUT_FOURTH_BUTTON)) {
                FourthButtonReleased();
                FourthButtonIsDown = false;
            }

            float oldX = xAngle;
            float oldY = yAngle;
            float oldZ = zAngle;

            xAngle = MaxXAngle * UnityEngine.Input.GetAxis(DeviceManager.INPUT_X_AXIS);
            yAngle = MaxYAngle * UnityEngine.Input.GetAxis(DeviceManager.INPUT_Y_AXIS);
            zAngle = MaxZAngle * UnityEngine.Input.GetAxis(DeviceManager.INPUT_Z_AXIS);
            Rotation = Quaternion.Euler(xAngle, yAngle, zAngle);

            if (Mathf.Abs(oldX - xAngle) > 0) {
                xAxisRotated(xAngle);
            }
            if (Mathf.Abs(oldY - yAngle) > 0) {
                yAxisRotated(yAngle);
            }
            if (Mathf.Abs(oldZ - zAngle) > 0) {
                zAxisRotated(zAngle);
            }
            if (Mathf.Abs(oldZ - zAngle) > 0 || Mathf.Abs(oldX - xAngle) > 0 || Mathf.Abs(oldY - yAngle) > 0) {
                RotationChanged(Rotation);
            }

        }

    }
}