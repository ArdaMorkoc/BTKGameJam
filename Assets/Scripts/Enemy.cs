using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform player;
    private Animator anim;
    private CapsuleCollider myCollider;
    private HealthManager healthManager;
    private bool isDead = false;

    [Header("Boss Ayarı")]
    [SerializeField] private bool isBossZombie;

    private float giveDamage;
    private float saldiriHizi = 1.5f;
    private float sonVurusZamani;

    [Header("Mesafe Ayarları")]
    [SerializeField] private float saldiriMesafesi = 2.0f; // Örümceğin vuruş menzili

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        myCollider = GetComponent<CapsuleCollider>();
        healthManager = GetComponent<HealthManager>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        // --- İstatistik Ayarları ---
        if (isBossZombie)
        {
            giveDamage = 20;
            if (healthManager != null) healthManager.baslangicCani = 150;
            agent.speed = 2.0f;
        }
        else
        {
            giveDamage = 10;
            if (healthManager != null) healthManager.baslangicCani = 100;
            agent.speed = 3.5f;
        }

        if (healthManager != null) healthManager.guncelCan = healthManager.baslangicCani;
    }

    void Update()
    {
        if (isDead) return;

        // Can Kontrolü
        if (healthManager != null && healthManager.guncelCan <= 0)
        {
            OldurBeni();
            return;
        }

        if (player != null)
        {
            // Oyuncuyu takip et
            agent.SetDestination(player.position);

            // --- HAREKET ANİMASYONU ---
            // Animator'daki "Speed" (Float) parametresini ajanın hızıyla besliyoruz
            float speed = agent.velocity.magnitude;
            anim.SetFloat("Speed", speed);

            // --- SALDIRI KONTROLÜ ---
            // Mesafe kontrolü OnCollisionStay'den daha sağlıklıdır
            float mesafe = Vector3.Distance(transform.position, player.position);
            if (mesafe <= saldiriMesafesi)
            {
                SaldiriYap();
            }
        }
    }

    void SaldiriYap()
    {
        // Saldırı hızı kontrolü
        if (Time.time > sonVurusZamani + saldiriHizi)
        {
            // Animator'daki "Attack" trigger'ını ateşle
            anim.SetTrigger("Attack");

            // Oyuncuya hasar ver
            HealthManager playerHealth = player.GetComponent<HealthManager>();
            if (playerHealth != null)
            {
                playerHealth.HasarAl((int)giveDamage);
            }

            sonVurusZamani = Time.time;
        }
    }

    // Dışarıdan mermi/kılıç vurunca bu fonksiyonu çağırabilirsin
    public void HasarAl(int miktar)
    {
        if (isDead) return;

        // Varsa hasar alma animasyonunu oynat
        anim.SetTrigger("TakeDamage");

        // HealthManager üzerinden canı düşür (zaten o yönetiyor diye varsayıyoruz)
    }

    void OldurBeni()
    {
        if (isDead) return;

        isDead = true;
        agent.isStopped = true;
        agent.enabled = false;
        myCollider.enabled = false;

        // --- ÖLÜM ANİMASYONU ---
        // Animator'daki "IsDead" (Bool) parametresini aktif et
        anim.SetBool("IsDead", true);

        if (GameManager.instance != null)
        {
            int puan = isBossZombie ? 50 : 10;
            GameManager.instance.AddScore(puan);
        }

        Destroy(gameObject, 4f);
    }
}