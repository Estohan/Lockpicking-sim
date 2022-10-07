using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lockpicking {
    public class VisualTest_LockpickinControls : MonoBehaviour {

        [SerializeField]
        private static InputManager inputManager;

        [SerializeField]
        private VisualTest_Tumblers VT_Tumblers;

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
            VT_Tumblers.TorqueWrenchPressure();
        }

        private void RotateWrenchStop() {
            Debug.Log("[Released wrench]");
            VT_Tumblers.TorqueWrenchRelease();
        }

        private void PinPushStarted() {
            pinPushing = true;
            Debug.Log("[Started pushing pin]");
        }

        private void PinPushStopped() {
            pinPushing = false;
            Debug.Log("[Stopped pushing pin]");
            VT_Tumblers.PinPushStop();
        }

        private void PinPushValue(float delta) {
            if (pinPushing) {
                Debug.Log("[Pushed pin] : " + delta);
                VT_Tumblers.PinPositionValueChange(delta);
            }
        }

        private void ChangeTumbler(float delta) {
            if (delta > 0) {
                Debug.Log("[Moved to next tumbler]");
            } else {
                Debug.Log("[Moved to previous tumbler]");
            }
            VT_Tumblers.ChangeTumbler(delta);
        }

        private void OnDisable() {
            inputManager.Lockpicking.Disable();
        }
    }
}