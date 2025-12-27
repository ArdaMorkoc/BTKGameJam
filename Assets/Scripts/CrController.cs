using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrController : MonoBehaviour
{
    private CharacterController characterController;
    private Animator animator;

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

    // --- YENİ SES AYARLARI ---
    [Header("Ses Ayarları")]
    [Tooltip("Karakterin üzerindeki AudioSource bileşenini buraya sürükleyin")]
    [SerializeField] private AudioSource muzikKaynagi;

    [Tooltip("Karakterin üzerindeki İKİNCİ AudioSource bileşenini buraya sürükleyin")]
    [SerializeField] private AudioSource adimKaynagi;

    [Tooltip("Müzik dosyasını buraya sürükleyin")]
    [SerializeField] private AudioClip muzikDosyasi;

    [Tooltip("Ayak sesi dosyasını buraya sürükleyin")]
    [SerializeField] private AudioClip adimDosyasi;

    [SerializeField] private float adimSikligi = 0.5f; // Adım sesi çalma hızı
    private float adimZamanlayicisi;
    // -------------------------

    private Vector3 _velocity;
    private bool _isGrounded;
    float health = 100;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();

        if (jumpButton != null)
            jumpButton.onClick.AddListener(Jump);

        // MÜZİĞİ BAŞLAT
        if (muzikKaynagi != null && muzikDosyasi != null)
        {
            muzikKaynagi.clip = muzikDosyasi;
            muzikKaynagi.loop = true; // Döngüye al
            muzikKaynagi.Play();
        }
    }

    private void Update()
    {
        // 1. ZEMİN KONTROLÜ
        _isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (_isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f;
        }

        // 2. HAREKET INPUT
        float x = movementJoystick.Horizontal;
        float z = movementJoystick.Vertical;
        Vector3 move = transform.right * x + transform.forward * z;
        float inputSiddeti = new Vector2(x, z).magnitude;

        // 3. HIZ VE HAREKET
        float anlikHiz = (inputSiddeti > 0.8f) ? kosmaHizi : yurumeHizi;
        if (inputSiddeti < 0.1f) move = Vector3.zero;

        characterController.Move(move * anlikHiz * Time.deltaTime);

        // 4. ANİMASYON
        animator.SetFloat("SpeedX", x, 0.1f, Time.deltaTime);
        animator.SetFloat("SpeedZ", z, 0.1f, Time.deltaTime);

        // --- ADIM SESİ KONTROLÜ ---
        // Eğer yerdeysek VE hareket ediyorsak ses çalalım
        if (_isGrounded && inputSiddeti > 0.1f)
        {
            adimZamanlayicisi -= Time.deltaTime; // Geri sayım

            if (adimZamanlayicisi <= 0)
            {
                // Sesi çal
                if (adimKaynagi != null && adimDosyasi != null)
                {
                    adimKaynagi.PlayOneShot(adimDosyasi);
                }

                // Koşuyorsa (0.8'den büyükse) daha sık çalsın
                float hizCarpan = (inputSiddeti > 0.8f) ? 1.5f : 1f;
                adimZamanlayicisi = adimSikligi / hizCarpan;
            }
        }
        else
        {
            // Durduğunda sıfırla
            adimZamanlayicisi = 0;
        }
        // -------------------------

        // 5. YERÇEKİMİ
        _velocity.y += _gravity * Time.deltaTime;
        characterController.Move(_velocity * Time.deltaTime);
    }

    private void Jump()
    {
        if (_isGrounded)
        {
            animator.SetTrigger("isJumped");
        }
    }

    public void TakeDamage(float dam)
    {
        health = Mathf.Clamp(health - dam, 0, 100);
        if (health <= 0)
        {
            Debug.Log("Öldün");
        }
    }
}