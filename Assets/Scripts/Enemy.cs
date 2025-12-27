using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform player;
    private Animator anim;
    private CapsuleCollider myCollider; // Zombi ölünce mermi geçsin diye
    private HealthManager healthManager;
    private bool isDead = false; // Zombi öldü mü kontrolü

    [Header("Boss Ayarı")]
    [SerializeField] private bool isBossZombie;

    private float giveDamage;
    private float saldiriHizi = 1.5f;
    private float sonVurusZamani;

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

        // --- AYARLAR ---
        if (isBossZombie)
        {
            giveDamage = 20;
            if (healthManager != null) healthManager.baslangicCani = 150;
            agent.speed = 1.5f;
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
        // Eğer öldüyse hiçbir şey yapma (Donup kalsın)
        if (isDead) return;

        // Can kontrolü: Eğer can 0'a indiyse ÖL
        if (healthManager.guncelCan <= 0)
        {
            OldurBeni();
            return;
        }

        if (player != null)
        {
            agent.SetDestination(player.position);
        }

        if (agent.velocity.magnitude > 0.1f)
        {
            anim.SetBool("IsMoving", true);
        }
        else
        {
            anim.SetBool("IsMoving", false);
        }
    }

    void OldurBeni()
    {
        isDead = true; // Artık ölü
        if (GameManager.instance != null)
        {
            int puan = isBossZombie ? 50 : 10;
            GameManager.instance.AddScore(puan);
        }
        agent.isStopped = true; // Yürümeyi kes
        agent.enabled = false; // NavMesh'i kapat (İçinden geçebilelim)
        myCollider.enabled = false; // Collider'ı kapat (Mermiler takılmasın)

        anim.SetTrigger("Die"); // Ölüm animasyonunu oynat

        // 4 saniye sonra cesedi sahneden sil
        Destroy(gameObject, 4f);
    }

    private void OnCollisionStay(Collision collision)
    {
        if (isDead) return; // Ölü zombi saldıramaz

        if (collision.transform.CompareTag("Player"))
        {
            if (Time.time > sonVurusZamani + saldiriHizi)
            {
                anim.SetTrigger("Attack");
                HealthManager playerHealth = collision.gameObject.GetComponent<HealthManager>();
                if (playerHealth != null)
                {
                    playerHealth.HasarAl((int)giveDamage);
                    sonVurusZamani = Time.time;
                }
            }
        }
    }
}