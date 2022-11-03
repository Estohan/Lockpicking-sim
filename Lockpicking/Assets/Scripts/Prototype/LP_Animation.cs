using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lockpicking {
    public class LP_Animation : MonoBehaviour {
        [SerializeField]
        private GameObject _wrench;
        [SerializeField]
        private GameObject _pick;
        [SerializeField]
        private LP_Lock _lock;

        [Header("Movement settings")]
        [SerializeField]
        private float lockLength;
        [Tooltip("How much can the pick rotate (degrees).")]
        [SerializeField]
        private float pickAngle;
        [SerializeField]
        private float pickSpeed;
        [SerializeField]
        private float resetPickStep;
        [SerializeField]
        private float movementError;

        private Vector3 origPickPos;
        private Vector3 origPickRot;
        private Vector3 currPickPos;
        private Quaternion currPickRot;
        // distance between two tumblers
        private float tumblerChangeStep;
        private bool pickIsMoving;

        private Animator wrenchAnimator;
        private Animator pickAnimator;
        private WaitForEndOfFrame endOfFrame;

        private int StartHash;
        private int UnlockHash;
        private int PressureHash;

        private void Awake() {
            endOfFrame = new WaitForEndOfFrame();

            pickAnimator = _pick.gameObject.GetComponent<Animator>();
            wrenchAnimator = _wrench.gameObject.GetComponent<Animator>();

            StartHash = Animator.StringToHash("Start");
            UnlockHash = Animator.StringToHash("Unlock");
            PressureHash = Animator.StringToHash("Pressure");
        }

        private void Start() {
            origPickPos = _pick.transform.localPosition;
            origPickRot = _pick.transform.localRotation.eulerAngles;
            currPickPos = origPickPos;

            pickIsMoving = false;

            tumblerChangeStep = lockLength / _lock.GetTumblersCount();
        }

        public void WrenchPressure() {
            wrenchAnimator.SetBool(PressureHash, true);
        }

        public void WrenchRelease() {
            wrenchAnimator.SetBool(PressureHash, false);
        }

        public void LockpickingIntro() {
            wrenchAnimator.SetTrigger(StartHash);
            pickAnimator.SetTrigger(StartHash);
        }

        public void Unlock() {
            wrenchAnimator.SetTrigger(UnlockHash);
            pickAnimator.enabled = true;
            pickAnimator.SetTrigger(UnlockHash);
        }

        public void ChangeTumbler(float direction) {
            // disable pick animator so that its transform values could
            // be modified through code
            pickAnimator.enabled = false;
            if (direction < 0 && // direction and threshold check
                currPickPos.z + tumblerChangeStep <= origPickPos.z) {
                currPickPos.z += tumblerChangeStep;
            }
            if (direction > 0 && // direction and threshold check
                currPickPos.z - tumblerChangeStep >= origPickPos.z - lockLength){
                currPickPos.z -= tumblerChangeStep;
            }

            if (!pickIsMoving) {
                StartCoroutine(MovePickCoroutine());
            } 
        }

        // position >= 0f && position <= 100f
        public void PushPin(float position) {
            pickAnimator.enabled = false;
            StopCoroutine(PickResetCoroutine(position));
            currPickRot = Quaternion.Euler(origPickRot.x + (pickAngle / 100f) * position,
                                            origPickRot.y,
                                            origPickRot.z);
            // Debug.Log(currPickRot.eulerAngles.x);
            _pick.transform.rotation = currPickRot;
        }

        public void PushPinStop(float position) {
            pickAnimator.enabled = false;
            StartCoroutine(PickResetCoroutine(position));
        }

        IEnumerator PickResetCoroutine(float position) {
            Debug.Log("Started PickResetCoroutine.");
            while (!currPickRot.Equals(origPickRot)) {
                position += resetPickStep;
                currPickRot = Quaternion.Euler(origPickRot.x + (pickAngle / 100f) * position,
                                            origPickRot.y,
                                            origPickRot.z);
                _pick.transform.rotation = currPickRot;
                yield return endOfFrame;
            }
        }

        IEnumerator MovePickCoroutine() {
            Vector3 newPos = _pick.transform.localPosition;
            float movementStep;

            pickIsMoving = true;

            while (true) {
                movementStep = pickSpeed * Time.deltaTime;

                // exit loop conditions
                if (_pick.transform.localPosition.z > currPickPos.z &&
                    _pick.transform.localPosition.z - movementStep < currPickPos.z + movementError) {
                    // close enough to reach current position
                    _pick.transform.localPosition = currPickPos;
                    break;
                }

                if (_pick.transform.localPosition.z < currPickPos.z &&
                    _pick.transform.localPosition.z + movementStep > currPickPos.z - movementError) {
                    // close enough to reach current position
                    _pick.transform.localPosition = currPickPos;
                    break;
                }

                // move outward
                if (_pick.transform.localPosition.z < currPickPos.z) {
                    newPos.z += movementStep;
                    // Debug.Log("+");
                }

                // move inward
                if (_pick.transform.localPosition.z > currPickPos.z) {
                    newPos.z -= movementStep;
                    // Debug.Log("-");
                }

                _pick.transform.localPosition = newPos;

                yield return endOfFrame;
            }

            pickIsMoving = false;
        }
    }
}