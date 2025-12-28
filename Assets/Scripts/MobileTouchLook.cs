using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileTouchLook : MonoBehaviour
{
    public float lookSensitivity = 0.15f;
    public Transform playerBody;

    private int lookFingerId = -1;
    private Vector2 lastTouchPosition;
    private float xRotation = 0f;

    void Update()
    {
        foreach (Touch touch in Input.touches)
        {
            // Sadece sağ yarı
            if (touch.position.x < Screen.width / 2f)
                continue;

            if (lookFingerId == -1 && touch.phase == TouchPhase.Began)
            {
                lookFingerId = touch.fingerId;
                lastTouchPosition = touch.position;
            }

            if (touch.fingerId == lookFingerId)
            {
                if (touch.phase == TouchPhase.Moved)
                {
                    Vector2 delta = touch.position - lastTouchPosition;

                    float mouseX = delta.x * lookSensitivity;
                    float mouseY = delta.y * lookSensitivity;

                    xRotation -= mouseY;
                    xRotation = Mathf.Clamp(xRotation, -80f, 80f);

                    transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
                    playerBody.Rotate(Vector3.up * mouseX);

                    lastTouchPosition = touch.position;
                }

                if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                {
                    lookFingerId = -1;
                }
            }
        }
    }
}
