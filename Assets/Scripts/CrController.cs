using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CrController : MonoBehaviour
{
    private CharacterController characterController;
    private Animator animator;

    [Header("Hareket Ayarları")]
    [SerializeField] private float yurumeHizi = 3f;
    [SerializeField] private float kosmaHizi = 6f;
    [SerializeField] private float _gravity = -9.81f;
    [SerializeField] private float _jump = 1.5f;

    [Header("Gerekli Bileşenler")]
    [SerializeField] private Joystick movementJoystick;
    [SerializeField] private Button jumpButton;

    [Header("Zemin Kontrolü")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.3f;
    [SerializeField] private LayerMask groundMask;

    [Header("Ses Ayarları (Efekt)")]
    [SerializeField] private AudioSource footstepSource;
    [SerializeField] private AudioClip walkClip;
    [SerializeField] private AudioClip runClip;

    [Header("Ses Ayarları (Müzik)")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioClip backgroundMusic;

    private Vector3 _velocity;
    private bool _isGrounded;
    private bool canJump = true;

    float health = 100;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();

        if (jumpButton != null)
            jumpButton.onClick.AddListener(Jump);

        PlayBackgroundMusic();
    }

    private void Update()
    {
        // 1️⃣ ZEMİN KONTROLÜ
        bool groundedNow = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (groundedNow && !_isGrounded)
        {
            // yeni yere bastı
            canJump = true;
        }

        _isGrounded = groundedNow;

        if (_isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f;
        }

        // 2️⃣ HAREKET
        float x = movementJoystick.Horizontal;
        float z = movementJoystick.Vertical;

        Vector3 move = transform.right * x + transform.forward * z;
        float inputSiddeti = new Vector2(x, z).magnitude;

        float anlikHiz = (inputSiddeti > 0.8f) ? kosmaHizi : yurumeHizi;
        if (inputSiddeti < 0.1f) move = Vector3.zero;

        characterController.Move(move * anlikHiz * Time.deltaTime);

        // 3️⃣ GRAVITY
        _velocity.y += _gravity * Time.deltaTime;
        characterController.Move(_velocity * Time.deltaTime);

        // 4️⃣ SES & ANİMASYON
        HandleFootsteps(inputSiddeti);

        if (animator != null)
        {
            animator.SetFloat("SpeedX", x, 0.1f, Time.deltaTime);
            animator.SetFloat("SpeedZ", z, 0.1f, Time.deltaTime);
        }
    }

    private void Jump()
    {
        if (!_isGrounded || !canJump) return;

        canJump = false;
        _isGrounded = false;

        if (animator != null)
            animator.SetTrigger("isJumped");

        _velocity.y = Mathf.Sqrt(_jump * -2f * _gravity);
    }

    private void HandleFootsteps(float inputSiddeti)
    {
        if (footstepSource == null || walkClip == null) return;

        if (_isGrounded && inputSiddeti > 0.1f)
        {
            bool isRunning = inputSiddeti > 0.8f;
            AudioClip targetClip = (isRunning && runClip != null) ? runClip : walkClip;

            if (!footstepSource.isPlaying || footstepSource.clip != targetClip)
            {
                footstepSource.clip = targetClip;
                footstepSource.loop = true;
                footstepSource.Play();
            }

            footstepSource.pitch = isRunning ? 1.25f : 1.0f;
        }
        else
        {
            if (footstepSource.isPlaying)
                footstepSource.Stop();
        }
    }

    private void PlayBackgroundMusic()
    {
        if (musicSource != null && backgroundMusic != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.loop = true;

            if (!musicSource.isPlaying)
                musicSource.Play();
        }
    }

    public void TakeDamage(float dam)
    {
        health = Mathf.Clamp(health - dam, 0, 100);
        if (health <= 0)
            Debug.Log("Öldün");
    }
}
