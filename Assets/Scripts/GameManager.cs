using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Paneller")]
    public GameObject startPanel;
    public GameObject pausePanel; // YENİ: Duraklatma paneli
    public GameObject settingsPanel;
    public GameObject creditsPanel;
    public GameObject gameOverPanel;

    [Header("UI Metinleri")]
    public TMP_Text scoreText;
    public TMP_Text finalScoreText;
    public TMP_Text highScoreText;

    [Header("Ayarlar")]
    public Slider volumeSlider;

    private int currentScore = 0;
    private int highScore = 0;
    private bool isPaused = false; // Oyun duraklatıldı mı?

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {


        // Tüm panelleri başlangıçta kapalı tut
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (creditsPanel != null) creditsPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);

        // Oyunun başında Start Panel açık olur ve zaman durur
        if (startPanel != null)
        {
            startPanel.SetActive(true);
            Time.timeScale = 0;
        }

        highScore = PlayerPrefs.GetInt("HighScore", 0);

        if (volumeSlider != null)
        {
            volumeSlider.value = PlayerPrefs.GetFloat("Volume", 1f);
            SetVolume(volumeSlider.value); // 🔥 KRİTİK SATIR
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }

        UpdateScoreUI();
    }

    // --- PAUSE SİSTEMİ ---

    public void PauseGame()
    {
        isPaused = true;
        pausePanel.SetActive(true);
        Time.timeScale = 0; // Zamanı dondur
    }

    public void ResumeGame()
    {
        isPaused = false;
        pausePanel.SetActive(false);
        settingsPanel.SetActive(false); // Eğer ayarlar açıksa onu da kapat
        Time.timeScale = 1; // Zamanı akıt
    }

    // Senin "Refreshleriz olur" dediğin mantık: Sahneyi baştan yükler
    public void ReturnToMainMenu()
    {
        Time.timeScale = 1; // Sahne yüklenmeden önce zamanı normale döndür (Önemli!)
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // --- DİĞER BUTONLAR ---

    public void StartGame()
    {
        startPanel.SetActive(false);
        Time.timeScale = 1;
    }

    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
    }

    public void OpenCredits()
    {
        creditsPanel.SetActive(true);
    }

    public void ClosePanels()
    {
        settingsPanel.SetActive(false);
        creditsPanel.SetActive(false);
    }

    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat("Volume", volume); // Kaydet
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    // --- SKOR VE OYUN SONU ---

    public void AddScore(int amount)
    {
        currentScore += amount;
        if (currentScore > highScore)
        {
            highScore = currentScore;
            PlayerPrefs.SetInt("HighScore", highScore);
        }
        UpdateScoreUI();
    }

    void UpdateScoreUI()
    {
        if (scoreText != null) scoreText.text = "Score: " + currentScore;
    }

    public void GameOver()
    {
        gameOverPanel.SetActive(true);
        finalScoreText.text = "Score: " + currentScore;
        highScoreText.text = "High Score: " + highScore;
        Time.timeScale = 0;
    }

    public void RestartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}