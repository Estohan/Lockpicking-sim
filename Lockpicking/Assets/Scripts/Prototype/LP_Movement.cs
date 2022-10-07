using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lockpicking {
    public class LP_Movement : MonoBehaviour {
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
        [Tooltip("How much can the wrench rotate (degrees.")]
        [SerializeField]
        private float wrenchAngle;
        [SerializeField]
        private float wrenchSpeed;

        private Quaternion origWrenchRot;
        private Vector3 origPickPos;
        private Quaternion origPickRot;
        private Quaternion currWrenchRot;
        private Vector3 currPickPos;
        private Quaternion currPickRot;
        // distance between two tumblers
        private float tumblerChangeStep;

        private WaitForEndOfFrame endOfFrame;

        private void Awake() {
            endOfFrame = new WaitForEndOfFrame();
        }

        private void Start() {
            origWrenchRot = _wrench.transform.localRotation;
            origPickPos = _pick.transform.localPosition;
            origPickRot = _pick.transform.localRotation;
            currWrenchRot = origWrenchRot;
            currPickPos = origPickPos;
            currPickRot = origPickRot;

            tumblerChangeStep = lockLength / _lock.GetTumblersCount();
        }

        public void WrenchPressure() {
            StartCoroutine(RotateWrenchCoroutine());
        }

        /*IEnumerator RotatePickCoroutine() {

        }

        IEnumerator MovePickCoroutine() {

        }*/

        IEnumerator RotateWrenchCoroutine() {
            float rotationStep = wrenchAngle / wrenchSpeed;
            Debug.Log("Original rot: " + origWrenchRot.eulerAngles);

            while (currWrenchRot.eulerAngles.z < wrenchAngle) {
                Debug.Log("Current rot: " + currWrenchRot.eulerAngles);
                currWrenchRot = Quaternion.Euler(origWrenchRot.eulerAngles.x,
                                                 origWrenchRot.eulerAngles.y,
                                                 currWrenchRot.eulerAngles.z + rotationStep);
                _wrench.transform.localRotation = currWrenchRot;
                yield return endOfFrame;
            }
        }
    }
}