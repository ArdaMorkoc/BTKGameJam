using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StoryPanel : MonoBehaviour
{
    [Header("Ekranda Değişecek Parçalar")]
    public Image mangaEkrani;
    public TextMeshProUGUI altYazi;

    [Header("Gecme İkonu Ayarları")]
    public GameObject gecmeSimgesi;
    public float zorunluBekleme = 3.0f; // Butonun çıkması için gereken süre
    public float otomatikGecisSuresi = 10.0f; // Hiç basmazsa kendi geçeceği süre
    public float fadeHizi = 1.5f;

    [Header("Sırasıyla İçerikler")]
    public Sprite[] mangaResimleri;
    [TextArea(3, 10)]
    public string[] hikayeYazilari;
    public AudioClip[] sesEfektleri;

    [Header("Ayarlar")]
    public string oyunSahnesiAdi = "Day1";

    private int suankiSira = 0;
    private AudioSource sesKaynagi;
    private bool gecebilirMi = false;
    private CanvasGroup iconCanvasGroup;

    void Start()
    {
        sesKaynagi = gameObject.AddComponent<AudioSource>();

        if (gecmeSimgesi != null)
        {
            iconCanvasGroup = gecmeSimgesi.GetComponent<CanvasGroup>();
            if (iconCanvasGroup == null) iconCanvasGroup = gecmeSimgesi.AddComponent<CanvasGroup>();

            gecmeSimgesi.SetActive(false);
            iconCanvasGroup.alpha = 0;
        }

        Guncelle();
    }

    public void Ileri()
    {
        // Eğer zorunlu bekleme süresi dolmadıysa basamasın
        if (!gecebilirMi) return;

        suankiSira++;

        if (suankiSira >= mangaResimleri.Length)
        {
            SceneManager.LoadScene(oyunSahnesiAdi);
        }
        else
        {
            Guncelle();
        }
    }

    void Guncelle()
    {
        // -- HER ŞEYİ SIFIRLA --
        gecebilirMi = false;

        // Önemli: Eski sayaçları (Auto Skip dahil) durduruyoruz ki üst üste binmesin
        StopAllCoroutines();

        if (gecmeSimgesi != null)
        {
            gecmeSimgesi.SetActive(false);
            iconCanvasGroup.alpha = 0;
        }

        // 1. Resim
        if (suankiSira < mangaResimleri.Length)
        {
            mangaEkrani.sprite = mangaResimleri[suankiSira];
            mangaEkrani.preserveAspect = true;
        }

        // 2. Yazı
        if (altYazi != null && suankiSira < hikayeYazilari.Length)
        {
            altYazi.text = hikayeYazilari[suankiSira];
        }

        // 3. Ses
        if (sesEfektleri.Length > suankiSira && sesEfektleri[suankiSira] != null)
        {
            sesKaynagi.Stop();
            sesKaynagi.clip = sesEfektleri[suankiSira];
            sesKaynagi.Play();
        }

        // 4. Zamanlayıcıları Başlat
        StartCoroutine(AkisYonetimi());
    }

    // --- TEK COROUTINE İÇİNDE TÜM ZAMANLAMAYI YÖNETİYORUZ ---
    IEnumerator AkisYonetimi()
    {
        // A. ZORUNLU BEKLEME (3 Saniye)
        // Oyuncu bu sürede geçemez, buton görünmez.
        yield return new WaitForSeconds(zorunluBekleme);

        // B. BUTONU AKTİF ET
        gecebilirMi = true; // Artık tıklayabilir
        if (gecmeSimgesi != null)
        {
            gecmeSimgesi.SetActive(true);
            StartCoroutine(PulseEfekti()); // Işık efektini ayrı başlat
        }

        // C. OTOMATİK GEÇİŞ İÇİN GERİ KALAN SÜREYİ BEKLE
        // (Toplam süre - Zorunlu bekleme) kadar daha bekleriz.
        // Örneğin: 10 - 3 = 7 saniye daha bekler.
        float kalanSure = otomatikGecisSuresi - zorunluBekleme;

        // Eğer kalan süre negatifse (yani auto skip süresini 2 sn yaptıysan) beklemesin
        if (kalanSure > 0)
        {
            yield return new WaitForSeconds(kalanSure);
        }

        // D. SÜRE DOLDU, HALA GEÇİLMEDİYSE OTOMATİK GEÇ
        // Buraya geldiyse oyuncu butona basmamış demektir.
        Ileri();
    }

    // --- PULSE (ISIK KISILIP AÇILMA) EFEKTİ ---
    IEnumerator PulseEfekti()
    {
        float zamanSayaci = 0f;
        while (true)
        {
            zamanSayaci += Time.deltaTime;
            float alphaDegeri = 0.2f + Mathf.PingPong(zamanSayaci * fadeHizi, 0.8f);

            if (iconCanvasGroup != null)
                iconCanvasGroup.alpha = alphaDegeri;

            yield return null;
        }
    }
}