using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GazePointScript : MonoBehaviour
{
    public GameObject gazePoint;     // UI element
    public GameObject targetObject;  // World space object
    public GameObject msg_congrate;

    private RectTransform gazeRectTransform;
    private Collider2D targetCollider;
    private Animator targetAnimator;
    private bool isAnimationEnded;

    void Start()
    {
        msg_congrate = GameObject.Find("msg_congrate");

        msg_congrate.SetActive(false);

        // Get the RectTransform of the gazePoint UI element
        gazeRectTransform = gazePoint.GetComponent<RectTransform>();

        // Get the Collider2D and Animator components of the target object
        targetCollider = targetObject.GetComponent<Collider2D>();
        targetAnimator = targetObject.GetComponent<Animator>();


    }

    void Update()
    {
        // Check if the gazePoint is active before proceeding
        if (!gazePoint.activeInHierarchy)
        {
            targetAnimator.enabled = false;
            return;
        }

        // Convert the gazePoint's position to world space
        Vector3 worldPoint;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(gazeRectTransform, gazeRectTransform.position, Camera.main, out worldPoint))
        {
            // Check if the world point is within the bounds of the target's collider
            if (targetCollider.OverlapPoint(worldPoint))
            {
                targetAnimator.enabled = true;
                Debug.Log("Gaze point is on target");
            }
            else
            {
                targetAnimator.enabled = false;
                Debug.Log("Gaze point is not on target");
            }
        }
        else
        {
            targetAnimator.enabled = false;
            Debug.Log("Failed to convert gaze point to world point");
        }

         // 애니메이션 상태를 검사
        if (targetAnimator.GetCurrentAnimatorStateInfo(0).IsName("penguinMove"))
        {
            if (targetAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
            {
                if (!isAnimationEnded)
                {
                    isAnimationEnded = true;
                    OnAnimationEnd();
                }
            }
            else
            {
                isAnimationEnded = false;
            }
        }
    }

    // 애니메이션 종료 시 호출할 함수
    private void OnAnimationEnd()
    {
        Debug.Log("Animation End");
        msg_congrate.SetActive(true);
    }
}

