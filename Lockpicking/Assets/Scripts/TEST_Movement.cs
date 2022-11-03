using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEST_Movement : MonoBehaviour {
    public bool moveCommand;
    public bool moveToRight;
    [Space]
    public float moveStep; // 0.5 on X
    public float moveSpeed;
    public float moveError;

    private Vector3 currentPos;
    private WaitForEndOfFrame frameEnd;
    private bool moving;

    // Start is called before the first frame update
    void Start() {
        moveCommand = false;
        moving = false;
        frameEnd = new();
        currentPos = this.transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (moveCommand) {
            if (moveToRight) {
                currentPos.x += moveStep;
            } else {
                currentPos.x -= moveStep;
            }

            if (!moving) {
                StartCoroutine(MoveCoroutine());
            }
            moveCommand = false;
        }
    }

    IEnumerator MoveCoroutine() {
        Vector3 newPos = this.transform.localPosition;
        float stepDist;
        moving = true;

        while (true) {
            stepDist = moveSpeed * Time.deltaTime;
            Debug.Log("CurrentPos: " + currentPos + ", StepDist: " + stepDist + ", LocalPos: " + 
                        this.transform.localPosition.x);
            // exit loop
            if (this.transform.localPosition.x > currentPos.x &&
                this.transform.localPosition.x - stepDist < currentPos.x + moveError) {
                // Debug.Log(this.transform.localPosition.x + " +/- " + stepDist + " vs. " + currentPos.x);
                this.transform.localPosition = currentPos;
                Debug.Log(". (<)");
                break;
            }

            if (this.transform.localPosition.x < currentPos.x &&
                this.transform.localPosition.x + stepDist > currentPos.x - moveError) {
                this.transform.localPosition = currentPos;
                Debug.Log(". (>)");
                break;
            }


            // move to right
            if (this.transform.localPosition.x < currentPos.x) {
                newPos.x += stepDist;
                Debug.Log("+");
            // move to left
            } else {
                newPos.x -= stepDist;
                Debug.Log("-");
            }
            this.transform.localPosition = newPos; // * Time.deltaTime
            yield return frameEnd;
        }
        moving = false;
    }
}
