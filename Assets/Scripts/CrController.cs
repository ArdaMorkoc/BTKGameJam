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

    [Header("Ses Ayarları")] // YENİ: Ses değişkenleri
    [SerializeField] private AudioSource backgroundMusicSource;
    [SerializeField] private AudioSource footstepSource;
    [SerializeField] private AudioClip backgroundMusicClip;
    [SerializeField] private AudioClip walkClip;
    [SerializeField] private AudioClip runClip; // Koşma sesi varsa buraya, yoksa boş kalabilir

    // Özel Değişkenler
    private Vector3 _velocity;
    private bool _isGrounded;
    float health = 100;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();

        if (jumpButton != null)
            jumpButton.onClick.AddListener(Jump);
        
        // YENİ: Müzik çalma başlatma
        if (backgroundMusicSource != null && backgroundMusicClip != null)
        {
            backgroundMusicSource.clip = backgroundMusicClip;
            backgroundMusicSource.loop = true; // Müziğin tekrar etmesi için
            backgroundMusicSource.Play();
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

        // 2. JOYSTICK VERİSİNİ AL
        float x = movementJoystick.Horizontal;
        float z = movementJoystick.Vertical;

        Vector3 move = transform.right * x + transform.forward * z;
        float inputSiddeti = new Vector2(x, z).magnitude;

        // 3. HIZI BELİRLE VE HAREKET ET
        float anlikHiz = (inputSiddeti > 0.8f) ? kosmaHizi : yurumeHizi;

        if (inputSiddeti < 0.1f) move = Vector3.zero;

        characterController.Move(move * anlikHiz * Time.deltaTime);

        // YENİ: Ayak sesi kontrolü fonksiyonunu çağır
        HandleFootsteps(inputSiddeti);

        // 4. ANİMASYONU GÜNCELLE
        animator.SetFloat("SpeedX", x, 0.1f, Time.deltaTime);
        animator.SetFloat("SpeedZ", z, 0.1f, Time.deltaTime);

        // 5. YERÇEKİMİ
        _velocity.y += _gravity * Time.deltaTime;
        characterController.Move(_velocity * Time.deltaTime);
    }

    // YENİ: Ayak seslerini yöneten fonksiyon
    private void HandleFootsteps(float inputSiddeti)
    {
        // Eğer ses kaynağı atanmamışsa işlem yapma
        if (footstepSource == null) return;

        // Yerdeysek ve hareket ediyorsak (inputSiddeti az da olsa varsa)
        if (_isGrounded && inputSiddeti > 0.1f)
        {
            // Koşuyor mu kontrolü
            bool isRunning = inputSiddeti > 0.8f;
            AudioClip targetClip = (isRunning && runClip != null) ? runClip : walkClip;

            // Eğer şu an ses çalmıyorsa veya çalan ses yanlış ise (örn: yürürken koşmaya geçtiyse)
            if (!footstepSource.isPlaying || footstepSource.clip != targetClip)
            {
                footstepSource.clip = targetClip;
                footstepSource.loop = true; // Adım sesinin sürekli gelmesi için loop
                footstepSource.Play();
            }

            // Opsiyonel: Koşarken sesi biraz inceltip hızlandırır (pitch)
            footstepSource.pitch = isRunning ? 1.2f : 1.0f;
        }
        else
        {
            // Duruyorsa veya havadaysa sesi kes
            if (footstepSource.isPlaying)
            {
                footstepSource.Stop();
            }
        }
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