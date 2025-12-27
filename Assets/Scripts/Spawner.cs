using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Normal Zombi Ayarları")]
    public GameObject normalZombiPrefab;
    public float normalSure = 20f;

    [Header("Boss Zombi Ayarları")]
    public GameObject bossPrefab;
    public float bossSure = 30f;

    [Header("Ortak Ayarlar")]
    public float alanGenisligi = 20f;
    private Camera anaKamera; // Kamerayı tanımamız lazım

    void Start()
    {
        anaKamera = Camera.main; // Sahnedeki Main Camera'yı bulur

        InvokeRepeating("NormalUret", 2f, normalSure);
        InvokeRepeating("BossUret", 10f, bossSure);
    }

    void NormalUret()
    {
        PozisyonBulVeUret(normalZombiPrefab);
    }

    void BossUret()
    {
        PozisyonBulVeUret(bossPrefab);
    }

    void PozisyonBulVeUret(GameObject hangisi)
    {
        Vector3 dogumYeri = Vector3.zero;
        bool uygunYerBulundu = false;
        int denemeSayisi = 0;

        // Kameranın görmediği bir yer bulana kadar 10 kere dener
        while (!uygunYerBulundu && denemeSayisi < 10)
        {
            float rastgeleX = Random.Range(-alanGenisligi, alanGenisligi);
            float rastgeleZ = Random.Range(-alanGenisligi, alanGenisligi);
            dogumYeri = new Vector3(rastgeleX, 0, rastgeleZ);

            // Eğer bu nokta ekranda GÖRÜNMÜYORSA, yer uygundur
            if (!EkrandaMi(dogumYeri))
            {
                uygunYerBulundu = true;
            }
            denemeSayisi++;
        }

        // Eğer uygun yer bulduysak (veya 10 deneme bittiyse) üret
        if (uygunYerBulundu)
        {
            Instantiate(hangisi, dogumYeri, Quaternion.identity);
        }
    }

    // Bir noktanın kamerada görünüp görünmediğini kontrol eden sihirli fonksiyon
    bool EkrandaMi(Vector3 pozisyon)
    {
        Vector3 ekranNoktasi = anaKamera.WorldToViewportPoint(pozisyon);

        // Viewport koordinatlarında (0,0) sol alt, (1,1) sağ üsttür.
        // Eğer nokta 0 ile 1 arasındaysa ekrandadır.
        bool xEkranda = ekranNoktasi.x > 0 && ekranNoktasi.x < 1;
        bool yEkranda = ekranNoktasi.y > 0 && ekranNoktasi.y < 1;
        bool ondeMi = ekranNoktasi.z > 0; // Kameranın önünde mi?

        return xEkranda && yEkranda && ondeMi;
    }
}