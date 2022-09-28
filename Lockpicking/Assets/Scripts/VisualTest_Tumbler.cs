using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Lockpicking {
    public class VisualTest_Tumbler : MonoBehaviour {

        // GameObjects
        [SerializeField]
        private GameObject _pin;
        [SerializeField]
        private GameObject _indicator;
        [SerializeField]
        private TextMeshProUGUI positionValueText;
        [SerializeField]
        private TextMeshProUGUI currentValueText;

        // Materials and renderers
        private Material matOriginalPin;
        private Material matOriginalIndicator;
        [SerializeField]
        private Material matYellow;
        [SerializeField]
        private Material matGreen;
        [SerializeField]
        private Material matRed;

        private MeshRenderer pinRenderer;
        private MeshRenderer indicatorRenderer;

        // Internal data
        private float positionValue;
        private float currentValue;
        private PinStates pinState;

        // Other settings
        public float pinAlignmentOffset;

        private void Awake() {
            pinRenderer = _pin.GetComponent<MeshRenderer>();
            indicatorRenderer = _indicator.GetComponent<MeshRenderer>();

            matOriginalPin = pinRenderer.sharedMaterial;
            matOriginalIndicator = indicatorRenderer.sharedMaterial;
        }

        private void Start() {
            // Set values
            currentValue = 100f;
            positionValue = UnityEngine.Random.Range(0f, 100f);
            pinState = PinStates.Free;

            // Print values
            currentValueText.text = (((int)(currentValue * 10)) / 10f).ToString();
            positionValueText.text = (((int)(positionValue * 10)) / 10f).ToString();
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
            ChangePinMaterial(1); // Change to Green
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
            currentValueText.text = (((int)(currentValue * 10)) / 10f).ToString();
            CheckPinPosition();
        }

        public void BindPin() {
            pinState = PinStates.Bound;
            ChangePinMaterial(2); // Change to Red
        }

        public void ResetPin() {
            pinState = PinStates.Free;
            ChangePinMaterial(0); // Change to original
            currentValue = 100f;
        }

        public void ReleasePin() {
            // Bound or set pins do not move back
            if (pinState == PinStates.Free) {
                currentValue = 100f;
                currentValueText.text = (((int)(currentValue * 10)) / 10f).ToString();
            }
        }

        public void SetActive(bool active) {
            ChangeIndicatorMaterial(active);
        }

        public bool isSet() {
            return pinState == PinStates.Set;
        }

        public PinStates GetState() {
            return pinState;
        }

        private void ChangeIndicatorMaterial(bool active) {
            if (active) {
                indicatorRenderer.sharedMaterial = matYellow;
            } else {
                indicatorRenderer.sharedMaterial = matOriginalIndicator;
            }
        }

        private void ChangePinMaterial(int state) {
            switch (state) {
                case 1:
                    pinRenderer.sharedMaterial = matGreen;
                    break;
                case 2:
                    pinRenderer.sharedMaterial = matRed;
                    break;
                default:
                    pinRenderer.sharedMaterial = matOriginalPin;
                    break;
            }
        }
    }
}