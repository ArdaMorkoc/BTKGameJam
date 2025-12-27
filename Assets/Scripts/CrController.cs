using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrController : MonoBehaviour
{
    private CharacterController characterController;
    private Animator animator; // Animasyoncusu ekledik

    [Header("Hareket Ayarları")]
    [SerializeField] private float yurumeHizi = 3f;
    [SerializeField] private float kosmaHizi = 6f;
    [SerializeField] private float _gravity = -9.8f;
    [SerializeField] private float _jump = 1.5f;

    [Header("Gerekli Bileşenler")]
    [SerializeField] private Joystick movementJoystick;
    [SerializeField] private Button jumpButton;

    [Header("Zemin Kontrolü")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.3f;
    [SerializeField] private LayerMask groundMask;

    // Özel Değişkenler
    private Vector3 _velocity;
    private bool _isGrounded;
    float health = 100;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();

        // Eğer butona basılırsa zıpla
        if (jumpButton != null)
            jumpButton.onClick.AddListener(Jump);
    }

    private void Update()
    {
        // 1. ZEMİN KONTROLÜ
        _isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (_isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f; // Yere tam otursun
        }

        // 2. JOYSTICK VERİSİNİ AL
        float x = movementJoystick.Horizontal;
        float z = movementJoystick.Vertical;

        Vector3 move = transform.right * x + transform.forward * z;

        // Joystick ne kadar itiliyor? (0 ile 1 arası)
        float inputSiddeti = new Vector2(x, z).magnitude;

        // 3. HIZI BELİRLE VE HAREKET ET
        // Eğer joystick çok itiliyorsa (0.8'den büyükse) koş, azsa yürü
        float anlikHiz = (inputSiddeti > 0.8f) ? kosmaHizi : yurumeHizi;

        // Eğer hiç hareket yoksa hızı sıfırla ki kaymasın
        if (inputSiddeti < 0.1f) move = Vector3.zero;

        characterController.Move(move * anlikHiz * Time.deltaTime);

        // 4. ANİMASYONU GÜNCELLE (ÖNEMLİ KISIM)
        // inputSiddeti: 0 ise durur, 0.5 ise yürür, 1 ise koşar.
        // 0.1f parametresi animasyonun yumuşak geçiş yapmasını sağlar (DampTime).
        //animator.SetFloat("SpeedX", inputSiddeti, 0.1f, Time.deltaTime);
        animator.SetFloat("SpeedX", x, 0.1f, Time.deltaTime);
        animator.SetFloat("SpeedZ", z, 0.1f, Time.deltaTime);

        // 5. YERÇEKİMİ
        _velocity.y += _gravity * Time.deltaTime;
        characterController.Move(_velocity * Time.deltaTime);
    }

    private void Jump()
    {
        if (_isGrounded)
        {
            //_velocity.y = Mathf.Sqrt(_jump * -2f * _gravity);
            animator.SetTrigger("isJumped");
        }
    }

    public void TakeDamage(float dam)
    {
        health = Mathf.Clamp(health - dam, 0, 100);

        if (health <= 0)
        {
            //animasyon burada çalışacak ve game over verilecek

            Debug.Log("Öldün");
        }
    }
}
