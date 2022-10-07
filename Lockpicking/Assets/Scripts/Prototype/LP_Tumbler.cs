using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Lockpicking {
    [System.Serializable] // [ DEBUG ]
    public class LP_Tumbler {

        // Internal data
        private float positionValue;
        private float currentValue;
        private PinStates pinState;

        // Other settings
        public float pinAlignmentOffset;

        private void Start() {
            // Set values
            currentValue = 100f;
            positionValue = UnityEngine.Random.Range(0f, 100f);
            pinState = PinStates.Free;
        }

        private void CheckPinPosition() {
            if (pinState != PinStates.Bound) {
                return;
            }

            if (currentValue >= positionValue - pinAlignmentOffset &&
                currentValue <= positionValue + pinAlignmentOffset) {
                SetPin();
            }
        }
        private void SetPin() {
            pinState = PinStates.Set;
        }

        public void MovePin(float delta) {
            // Set pins cannot be moved anymore
            if (pinState == PinStates.Set) {
                return;
            }

            if (currentValue + delta > 100f) {
                currentValue = 100f;
                return;
            }

            if (currentValue + delta < 0f) {
                currentValue = 0f;
                return;
            }

            currentValue += delta;
            CheckPinPosition();
        }

        public void BindPin() {
            pinState = PinStates.Bound;
        }

        public void ResetPin() {
            pinState = PinStates.Free;
            currentValue = 100f;
        }

        public void ReleasePin() {
            // Bound or set pins do not move back
            if (pinState == PinStates.Free) {
                currentValue = 100f;
            }
        }

        public bool isSet() {
            return pinState == PinStates.Set;
        }

        public PinStates GetState() {
            return pinState;
        }
    }
}