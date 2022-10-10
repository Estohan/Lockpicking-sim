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
        private float movementError;

        private Vector3 origPickPos;
        private Quaternion origPickRot;
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
            origPickRot = _pick.transform.localRotation;
            currPickPos = origPickPos;
            currPickRot = origPickRot;

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
            if (direction > 0) {
                currPickPos.z += tumblerChangeStep;
            } else {
                currPickPos.z -= tumblerChangeStep;
            }

            if (!pickIsMoving) {
                StartCoroutine(ChangeTumblerCoroutine());
            } 
        }

        /*IEnumerator RotatePickCoroutine() {

        }

        IEnumerator MovePickCoroutine() {

        }*/

        IEnumerator ChangeTumblerCoroutine() {
            float movementStep = tumblerChangeStep / (pickSpeed * 100); // distance / ms
            //Debug.Log("[ Angle offset: " + wrenchAngleOffset + ", Final angle: " + finalAngle + " ");
            //Debug.Log("Original rot: " + origWrenchRot.eulerAngles + " ]");
            Vector3 newPos = _pick.transform.localPosition;

            pickIsMoving = true;
            Debug.Log("Orig pos: " + origPickPos);

            while (_pick.transform.localPosition.z > currPickPos.z + movementError || 
                    _pick.transform.localPosition.z < currPickPos.z - movementError) {
                // Debug.Log("Current rot: " + currWrenchRot.eulerAngles);
                Debug.Log("newPos: " + newPos + ", currentPos: " + currPickPos);
                if (_pick.transform.localPosition.z > currPickPos.z + movementError) {
                    newPos.z += tumblerChangeStep * Time.deltaTime;
                }
                if (_pick.transform.localPosition.z < currPickPos.z - movementError) {
                    newPos.z -= tumblerChangeStep + Time.deltaTime;
                }

                _pick.transform.localPosition = newPos;
                yield return endOfFrame;
            }

            pickIsMoving = false;
        }
    }
}