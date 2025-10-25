using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 8f;
    public float moveLimit = 4f; // left-right limit
    public float swipeSensitivity = 0.02f; // how much swipe moves player

    private Vector2 touchStart;
    private bool isSwiping;

    void Update()
    {
        float input = 0f;

        // --- Keyboard input (for testing in Editor) ---
        input = Input.GetAxis("Horizontal");

        // --- Mobile swipe input ---
#if UNITY_ANDROID || UNITY_IOS
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    touchStart = touch.position;
                    isSwiping = true;
                    break;

                case TouchPhase.Moved:
                    if (isSwiping)
                    {
                        Vector2 delta = touch.deltaPosition;
                        float horizontal = delta.x * swipeSensitivity * Time.deltaTime;
                        transform.Translate(horizontal * moveSpeed, 0, 0);
                    }
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    isSwiping = false;
                    break;
            }
        }
#endif

        // --- Keyboard move ---
        if (Mathf.Abs(input) > 0.01f)
        {
            Vector3 move = new Vector3(input, 0, 0) * moveSpeed * Time.deltaTime;
            transform.Translate(move);
        }

        // Clamp movement within arena bounds
        float clampedX = Mathf.Clamp(transform.position.x, -moveLimit, moveLimit);
        transform.position = new Vector3(clampedX, transform.position.y, transform.position.z);
    }
}