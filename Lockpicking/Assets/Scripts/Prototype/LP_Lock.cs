using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lockpicking {
    public class LP_Lock : MonoBehaviour {
        private LP_Animation toolsAnimation;

        [SerializeField]
        private int tumblersCount;

        public List<LP_Tumbler> tumblers;
        // Lock pin sequence
        public int[] pinSequence; // [ DEBUG ] change to private
        public int currPosInSequence; // [ DEBUG ] change to private
        public  bool canBindAnotherPin; // [ DEBUG ] change to private

        public int currentPin; // change to private

        private void Awake() {
            toolsAnimation = this.gameObject.GetComponent<LP_Animation>();
        }

        private void Start() {
            tumblers = new();
            for (int i = 0; i < tumblersCount; i ++) {
                tumblers.Add(new LP_Tumbler());
            }

            // Initialize tumblers states
            currentPin = 0;
            canBindAnotherPin = true;
            currPosInSequence = -1;

            foreach (LP_Tumbler tumbler in tumblers) {
                tumbler.ReleasePin();
            }

            // Generate pin sequence
            CreatePinSequence();

            // Start minigame
            StartLockpicking();
        }

        public void StartLockpicking() {
            // AudioManager.instance.PlayIntroAudio();
            toolsAnimation.LockpickingIntro();
        }

        public void PinPushStart() {
            /* do nothing*/
        }

        public void PinPushStop() {
            tumblers[currentPin].ReleasePin();
            toolsAnimation.PushPinStop(tumblers[currentPin].GetPinValue());
        }

        public void PinPositionValueChange(float delta) {
            // If this pin is set, it cannot be moved anymore
            if (tumblers[currentPin].GetState() == PinStates.Set) {
                return;
            }
            // Move pin
            tumblers[currentPin].MovePin(delta);
            toolsAnimation.PushPin(tumblers[currentPin].GetPinValue());
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
                AudioManager.instance.PlayTumblerChangeAudio();
            } else {
                PreviousTumbler();
                AudioManager.instance.PlayTumblerChangeAudio();
            }
            toolsAnimation.ChangeTumbler(direction);
        }

        public void TorqueWrenchPressure() {
            // Reset pin states
            for (int i = 0; i < tumblersCount; i++) {
                tumblers[i].ResetPin();
            }
            // Bind first pin
            BindNextPinInSequence();
            // Wrench animation
            toolsAnimation.WrenchPressure();
        }

        public void TorqueWrenchRelease() {
            // Reset all tumblers
            foreach (LP_Tumbler tumbler in tumblers) {
                tumbler.ResetPin();
            }

            // Reset pin binding and pin sequence counter
            canBindAnotherPin = true;
            currPosInSequence = -1;
            // Wrench animation
            toolsAnimation.WrenchRelease();
        }

        public int GetTumblersCount() {
            return tumblersCount;
        }

        private bool AreAllPinsSet() {
            bool allPinsAreSet = true;
            foreach (LP_Tumbler tumbler in tumblers) {
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
            if (currPosInSequence >= tumblersCount) {
                return;
            }

            // Bind next pin in sequence
            tumblers[pinSequence[currPosInSequence]].BindPin();
            canBindAnotherPin = false;
        }

        private void NextTumbler() {
            if (currentPin + 1 < tumblersCount) {
                currentPin++;
            }
        }

        private void PreviousTumbler() {
            if (currentPin - 1 > -1) {
                currentPin--;
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

            pinSequence = new int[tumblersCount];

            for (int i = 0; i < tumblersCount; i++) {
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