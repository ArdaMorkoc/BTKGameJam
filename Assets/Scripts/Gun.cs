using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.UI.Image;

public class Gun : MonoBehaviour
{
    [Header("Silah Ayarları")]
    public float range = 1000f;
    public int damage = 50;
    public float atisHizi = 1.0f; // Biraz hızlandırdım (Daha seri sıksın)
    public float reloadSuresi = 3f; // Reload süresini kısalttım (Oyun akıcı olsun)

    // Zombiyi itme gücünü artırdık çünkü artık zombiler 80 kilo :)
    public float itmeGucu = 50f;

    [Header("Nişan Ayarları")]
    public float mermiKalinligi = 0.5f; // Lazerimiz artık kalın (Iskalamak zor)

    [Header("Efekt Ayarları")]
    public float titremeGucu = 0.1f;
    public float titremeSuresi = 0.1f;
    public Light namluIsigi;

    public Camera fpsCamera;
    public LayerMask enemyLayer;

    // --- YENİ EKLENEN KISIM: Ses Efektleri ---
    [Header("Ses Efektleri")]
    public AudioSource gunAudioSource;
    public AudioClip shootClip;
    public AudioClip reloadClip;
    // -----------------------------------------

    [Header("Mermi Ayarları")]
    public int currentAmmo = 0;
    public int magCapasity = 12;

    [Header("UI Kısmı")]
    public TextMeshProUGUI ammoText;

    private float birSonrakiAtisZamani = 0f;
    private bool isReloading = false;
    public Animator animator;

    private void Start()
    {
        currentAmmo = magCapasity;
        GuncelleUI();
        if (namluIsigi != null) namluIsigi.intensity = 0;
    }

    private void Update()
    {
        // Hata ayıklama çizgisini kalın göremeyiz ama yönü görelim
        Debug.DrawRay(fpsCamera.transform.position, fpsCamera.transform.forward * range, Color.green);

        if (isReloading) return;

        if (currentAmmo <= 0)
        {
            StartCoroutine(ReloadYap());
            return;
        }

        // --- GELİŞMİŞ OTOMATİK ATEŞ SİSTEMİ (SPHERECAST) ---
        RaycastHit hit;

        // Raycast yerine SphereCast kullandık. Bu "kalın" bir ışın atar.
        if (Physics.SphereCast(fpsCamera.transform.position, mermiKalinligi, fpsCamera.transform.forward, out hit, range, enemyLayer))
        {
            // Eğer süre dolduysa ATEŞ ET
            if (Time.time >= birSonrakiAtisZamani)
            {
                Shoot(hit);
                animator.SetTrigger("isShooted");
                birSonrakiAtisZamani = Time.time + atisHizi;
            }
        }
    }

    public void ReloadButton()
    {
        StartCoroutine(ReloadYap());
    }

    IEnumerator ReloadYap()
    {
        isReloading = true;
        if (ammoText != null)
        {
            ammoText.text = "Reloading...";
            ammoText.color = Color.red;
        }

        // --- YENİ EKLENEN KISIM: Reload Sesi ---
        if (gunAudioSource != null && reloadClip != null)
        {
            gunAudioSource.PlayOneShot(reloadClip);
        }
        // ----------------------------------------

        yield return new WaitForSeconds(reloadSuresi);

        currentAmmo = magCapasity;
        isReloading = false;
        GuncelleUI();
    }

    void Shoot(RaycastHit hit)
    {
        currentAmmo--;
        GuncelleUI();

        // --- YENİ EKLENEN KISIM: Ateş Sesi ---
        if (gunAudioSource != null && shootClip != null)
        {
            gunAudioSource.PlayOneShot(shootClip);
        }
        // -------------------------------------

        StartCoroutine(EkranTitret());
        if (namluIsigi != null) StartCoroutine(NamluIsigiYak());

        HealthManager targetHealth = hit.transform.GetComponentInParent<HealthManager>();

        if (targetHealth != null)
        {
            targetHealth.HasarAl(damage);

            Rigidbody rb = hit.collider.attachedRigidbody;
            if (rb != null)
            {
                // Zombiyi geriye doğru it ama havaya kaldırma
                Vector3 itmeYonu = -hit.normal;
                itmeYonu.y = 0;

                rb.AddForce(itmeYonu * itmeGucu, ForceMode.Impulse);
            }
        }
    }

    IEnumerator NamluIsigiYak()
    {
        namluIsigi.intensity = 100f;
        yield return new WaitForSeconds(0.1f);
        namluIsigi.intensity = 0f;
    }

    IEnumerator EkranTitret()
    {
        Vector3 orjinalPozisyon = fpsCamera.transform.localPosition;
        float gecenSure = 0.0f;

        while (gecenSure < titremeSuresi)
        {
            float x = Random.Range(-1f, 1f) * titremeGucu;
            float y = Random.Range(-1f, 1f) * titremeGucu;
            fpsCamera.transform.localPosition = new Vector3(orjinalPozisyon.x + x, orjinalPozisyon.y + y, orjinalPozisyon.z);
            gecenSure += Time.deltaTime;
            yield return null;
        }
        fpsCamera.transform.localPosition = orjinalPozisyon;
    }

    void GuncelleUI()
    {
        if (ammoText != null)
        {
            ammoText.text = currentAmmo.ToString() + "/" + magCapasity.ToString();
            if (currentAmmo <= magCapasity / 2) ammoText.color = Color.yellow;
            else ammoText.color = Color.white;
        }
    }
}