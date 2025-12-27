using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // Sahneyi yeniden başlatmak için gerekli kütüphane

public class HealthManager : MonoBehaviour
{
    public float baslangicCani = 100;
    public float guncelCan;

    [Header("UI Bağlantısı (Sadece Player İçin)")]
    public Image healthBar; // Can barı görselini buraya sürükle (Varsa)
    public Image damagePanel;

    private void Update()
    {
        // --- YENİ: Kırmızılık yavaşça kaybolsun ---
        if (damagePanel != null)
        {
            if (damagePanel.color.a > 0)
            {
                Color renk = damagePanel.color;
                renk.a -= Time.deltaTime; // Yavaşça saydamlaş
                damagePanel.color = renk;
            }
        }
    }
    void Start()
    {
        guncelCan = baslangicCani;
    }

    public void HasarAl(int hasarMiktari)
    {
        guncelCan -= hasarMiktari;

        // Can barı varsa güncelle (Sadece Player için çalışır)
        if (healthBar != null)
        {
            healthBar.fillAmount = guncelCan / baslangicCani;
        }

        if (damagePanel != null && gameObject.CompareTag("Player"))
        {
            Color renk = damagePanel.color;
            renk.a = 0.8f; // Aniden kırmızı yap (0.8 opaklık)
            damagePanel.color = renk;
        }

        // --- ÖLÜM KONTROLÜ ---
        if (guncelCan <= 0)
        {
            // Eğer canı biten obje "Player" ise...
            if (gameObject.CompareTag("Player"))
            {
                // GameManager'a "Oyun Bitti" de!
                if (GameManager.instance != null)
                {
                    GameManager.instance.GameOver();
                }
            }
        }
    }
}