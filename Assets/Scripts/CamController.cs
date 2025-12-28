using UnityEngine;

public class CamController : MonoBehaviour
{
    [SerializeField] float lookSensitivity = 0.15f;
    [SerializeField] Transform player;
    [SerializeField] RectTransform movementJoystick;

    [Header("Camera Limits")]
    [SerializeField] float minY = -35f;
    [SerializeField] float maxY = 40f;

    Vector2 lastTouchPosition;
    int lookFingerId = -1;
    float xRotation = 0f;

    void Start()
    {
        GameObject fixedUI = GameObject.Find("Fixed");

        if (fixedUI != null)
        {
            movementJoystick = fixedUI.GetComponent<RectTransform>();
        }
        else
        {
            Debug.Log("Fixed bulunamadı!");
        }
    }

    void Update()
    {
        foreach (Touch touch in Input.touches)
        {
            // Eğer joystick alanındaysa görmezden gel
            if (RectTransformUtility.RectangleContainsScreenPoint(movementJoystick, touch.position))
                continue;

            // Yeni swipe başlat
            if (lookFingerId == -1 && touch.phase == TouchPhase.Began)
            {
                lookFingerId = touch.fingerId;
                lastTouchPosition = touch.position;
            }

            // Sadece bizim swipe parmağımız
            if (touch.fingerId != lookFingerId)
                continue;

            if (touch.phase == TouchPhase.Moved)
            {
                Vector2 delta = touch.position - lastTouchPosition;

                float mouseX = delta.x * lookSensitivity;
                float mouseY = delta.y * lookSensitivity;

                // Kamera yukarı-aşağı
                xRotation -= mouseY;
                xRotation = Mathf.Clamp(xRotation, minY, maxY);
                transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

                // Karakter sağ-sol
                player.Rotate(Vector3.up * mouseX);

                lastTouchPosition = touch.position;
            }

            if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                lookFingerId = -1;
            }
        }
    }
}
