using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamController : MonoBehaviour
{
    [SerializeField] float mouseSensitivity = 100f;
    [SerializeField] Transform playerBody;
    [SerializeField] Joystick lookJoystick;

    [Header("Camera Limits")]
    [SerializeField] float minY = -35f; // aşağı bakma
    [SerializeField] float maxY = 40f;  // yukarı bakma

    float xRotation = 0f;

    void Update()
    {
        float mouseX = lookJoystick.Horizontal * mouseSensitivity * Time.deltaTime;
        float mouseY = lookJoystick.Vertical * mouseSensitivity * Time.deltaTime;

        // YUKARI - AŞAĞI (KAMERA)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, minY, maxY);
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // SAĞ - SOL (PLAYER)
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
