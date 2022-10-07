using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lockpicking {
    public class LockpickControls : MonoBehaviour {

        [SerializeField]
        private static InputManager inputManager;

        [SerializeField]
        private LP_Lock _lock;

        private bool pinPushing;

        void Start() {
            inputManager = new InputManager();
            pinPushing = false;

            inputManager.Lockpicking.Enable();

            // Apply force on torque wrench
            inputManager.Lockpicking.RotateWrench.started += _ => RotateWrenchRotate();
            inputManager.Lockpicking.RotateWrench.canceled += _ => RotateWrenchStop();

            // Start pin pushing
            inputManager.Lockpicking.StartPinPush.started += _ => PinPushStarted();
            inputManager.Lockpicking.StartPinPush.canceled += _ => PinPushStopped();

            // Move pin
            inputManager.Lockpicking.PushPin.started += context => PinPushValue(context.ReadValue<float>());

            // Change tumbler
            inputManager.Lockpicking.ChangeTumbler.started += context => ChangeTumbler(context.ReadValue<float>());
        }

        private void RotateWrenchRotate() {
            Debug.Log("[Rotating wrench]");
            _lock.TorqueWrenchPressure();
        }

        private void RotateWrenchStop() {
            Debug.Log("[Released wrench]");
            _lock.TorqueWrenchRelease();
        }

        private void PinPushStarted() {
            pinPushing = true;
            Debug.Log("[Started pushing pin]");
        }

        private void PinPushStopped() {
            pinPushing = false;
            Debug.Log("[Stopped pushing pin]");
            _lock.PinPushStop();
        }

        private void PinPushValue(float delta) {
            if (pinPushing) {
                Debug.Log("[Pushed pin] : " + delta);
                _lock.PinPositionValueChange(delta);
            }
        }

        private void ChangeTumbler(float delta) {
            if (delta > 0) {
                Debug.Log("[Moved to next tumbler]");
            } else {
                Debug.Log("[Moved to previous tumbler]");
            }
            _lock.ChangeTumbler(delta);
        }

        private void OnDisable() {
            inputManager.Lockpicking.Disable();
        }
    }
}