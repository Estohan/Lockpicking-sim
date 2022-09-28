using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lockpicking {
    public class VisualTest_Tumblers : MonoBehaviour {

        [SerializeField]
        private List<VisualTest_Tumbler> tumblers;

        //private List<PinStates> tumblersPinStates;
        private int tumblersCount;
        private int currentPin;
        private bool canBindAnotherPin;

        /*
         TODO:
            Use tumbler.getstate() instead of tumblerPinState
            
         */

        private void Start() {
            // Initialize tumblers states
            tumblersCount = tumblers.Count;
            currentPin = 0;
            canBindAnotherPin = true;

            //tumblersPinStates = new();
            foreach (VisualTest_Tumbler tumbler in tumblers) {
                tumbler.SetActive(false);
                tumbler.ReleasePin();
                //tumblersPinStates.Add(PinStates.Free);
            }

            tumblers[currentPin].SetActive(true);
        }

        public void PinPushStart() {
            /* do nothing*/
        }

        public void PinPushStop() {
            tumblers[currentPin].ReleasePin();
        }

        public void PinPositionValueChange(float delta) {
            // If this pin is set, it cannot be moved anymore
            /*if(tumblersPinStates[currentPin] == PinStates.Set) {
                return;
            }*/
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
                    BindRandomPin();
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
                //tumblersPinStates[i] = PinStates.Free;
                tumblers[i].ResetPin();
            }
            // Choose bound pin
            BindRandomPin();
        }

        public void TorqueWrenchRelease() {
            // Reset all tumblers
            foreach (VisualTest_Tumbler tumbler in tumblers) {
                tumbler.ResetPin();
            }

            /*for (int i = 0; i < tumblersCount; i ++) {
                tumblersPinStates[i] = PinStates.Free;
            }*/

            canBindAnotherPin = true;
        }

        private bool AreAllPinsSet() {
            bool allPinsAreSet = true;
            /*foreach (PinStates pinState in tumblersPinStates) {
                if (pinState != PinStates.Set) {
                    allPinsAreSet = false;
                    break;
                }
            }*/
            foreach (VisualTest_Tumbler tumbler in tumblers) {
                if (tumbler.GetState() != PinStates.Set) {
                    allPinsAreSet = false;
                    break;
                }
            }
            return allPinsAreSet;
        }

        private void BindRandomPin() {
            int chosenBoundPinIndex;
            List<int> freePinsIndexes = new();
            // Check if there are no pins already bound
            if (!canBindAnotherPin) {
                return;
            }

            // Find all free pins
            /*for (int i = 0; i < tumblersCount; i++) {
                if (tumblersPinStates[i] == PinStates.Free) {
                    freePinsIndexes.Add(i);
                }
            }*/
            for (int i = 0; i < tumblersCount; i++) {
                if (tumblers[i].GetState() == PinStates.Free) {
                    freePinsIndexes.Add(i);
                }
            }
            // If there are no more free pins
            if (freePinsIndexes.Count == 0) {
                return;
            }
            // Otherwise randomly choose one to bind
            chosenBoundPinIndex = freePinsIndexes[UnityEngine.Random.Range(0, freePinsIndexes.Count)];
            //tumblersPinStates[chosenBoundPinIndex] = PinStates.Bound;
            tumblers[chosenBoundPinIndex].BindPin();
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
    }

    public enum PinStates {
        Free,
        Bound,
        Set
    }
}