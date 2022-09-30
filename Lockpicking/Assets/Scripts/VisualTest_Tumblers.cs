using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lockpicking {
    public class VisualTest_Tumblers : MonoBehaviour {

        [SerializeField]
        private List<VisualTest_Tumbler> tumblers;

        // Lock pin sequence
        private int[] pinSequence;
        private int currPosInSequence;
        private bool canBindAnotherPin;

        private int tumblersCount;
        private int currentPin;

        private void Start() {
            // Initialize tumblers states
            tumblersCount = tumblers.Count;
            currentPin = 0;
            canBindAnotherPin = true;
            currPosInSequence = -1;

            foreach (VisualTest_Tumbler tumbler in tumblers) {
                tumbler.SetActive(false);
                tumbler.ReleasePin();
            }

            tumblers[currentPin].SetActive(true);

            // Generate pin sequence
            CreatePinSequence();
        }

        public void PinPushStart() {
            /* do nothing*/
        }

        public void PinPushStop() {
            tumblers[currentPin].ReleasePin();
        }

        public void PinPositionValueChange(float delta) {
            // If this pin is set, it cannot be moved anymore
            if (tumblers[currentPin].GetState() == PinStates.Set) {
                return;
            }
            // Move pin
            tumblers[currentPin].MovePin(delta);
            // If pin is set then bind another pin or rotate lock core
            if (tumblers[currentPin].isSet()) {
                if (AreAllPinsSet()) {
                    Debug.Log("!!! LOCK PICKED !!!");
                } else {
                    canBindAnotherPin = true;
                    BindNextPinInSequence();
                }
            }
        }

        public void ChangeTumbler(float direction) {
            if (direction > 0) {
                NextTumbler();
            } else {
                PreviousTumbler();
            }
        }

        public void TorqueWrenchPressure() {
            // Reset pin states
            for (int i = 0; i < tumblersCount; i++) {
                tumblers[i].ResetPin();
            }
            // Bind first pin
            BindNextPinInSequence();
        }

        public void TorqueWrenchRelease() {
            // Reset all tumblers
            foreach (VisualTest_Tumbler tumbler in tumblers) {
                tumbler.ResetPin();
            }

            // Reset pin binding and pin sequence counter
            canBindAnotherPin = true;
            currPosInSequence = -1;
        }

        private bool AreAllPinsSet() {
            bool allPinsAreSet = true;
            foreach (VisualTest_Tumbler tumbler in tumblers) {
                if (tumbler.GetState() != PinStates.Set) {
                    allPinsAreSet = false;
                    break;
                }
            }
            return allPinsAreSet;
        }

        private void BindNextPinInSequence() {
            // Check if there are no pins already bound
            if (!canBindAnotherPin) {
                return;
            }

            // Check if there are any free pins left
            currPosInSequence++;
            if (currPosInSequence >= tumblers.Count) {
                return;
            }

            // Bind next pin in sequence
            tumblers[pinSequence[currPosInSequence]].BindPin();
            canBindAnotherPin = false;
        }

        private void NextTumbler() {
            if (currentPin + 1 < tumblersCount) {
                tumblers[currentPin].SetActive(false);
                currentPin++;
                tumblers[currentPin].SetActive(true);
            }
        }

        private void PreviousTumbler() {
            if (currentPin - 1 > -1) {
                tumblers[currentPin].SetActive(false);
                currentPin--;
                tumblers[currentPin].SetActive(true);
            }
        }

        /*
         * Create a random sequence of binding the pins and store it 
         * in pinSequence.
         * Ex: 
         *      tumblers.Count = 7     ->     seq = [6, 0, 5, 4, 2, 1, 3]
         */
        private void CreatePinSequence() {
            List<int> auxPins = new();
            int randIndex;
            int j = 0;

            pinSequence = new int[tumblers.Count];

            for (int i = 0; i < tumblers.Count; i ++) {
                auxPins.Add(i);
            }

            while (auxPins.Count > 0) {
                randIndex = UnityEngine.Random.Range(0, auxPins.Count);
                pinSequence[j] = auxPins[randIndex];
                auxPins.RemoveAt(randIndex);
                j++;
            }
        }
    }

    public enum PinStates {
        Free,
        Bound,
        Set
    }
}