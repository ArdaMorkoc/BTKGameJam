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
    public GameObject settingsPanel;
    public GameObject creditsPanel;
    public GameObject gameOverPanel;

    [Header("UI Metinleri")]
    public TMP_Text scoreText;
    public TMP_Text finalScoreText;
    public TMP_Text highScoreText;

    [Header("Ayarlar")]
    public Slider volumeSlider; // Settings panelindeki slider

    private int currentScore = 0;
    private int highScore = 0;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // Başlangıçta her şeyi kapat, sadece start paneli aç
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (creditsPanel != null) creditsPanel.SetActive(false);

        if (startPanel != null)
        {
            startPanel.SetActive(true);
            Time.timeScale = 0;
        }

        highScore = PlayerPrefs.GetInt("HighScore", 0);

        // Ses ayarını slider'a yükle (0 ile 1 arası)
        if (volumeSlider != null)
        {
            volumeSlider.value = AudioListener.volume;
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }

        UpdateScoreUI();
    }

    // --- BUTON FONKSİYONLARI ---

    public void StartGame()
    {
        startPanel.SetActive(false);
        Time.timeScale = 1;
    }

    // Settings Panelini Aç
    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
        // Start panelini kapatmak istersen: startPanel.SetActive(false);
    }

    // Credits Panelini Aç
    public void OpenCredits()
    {
        creditsPanel.SetActive(true);
    }

    // Panelleri Kapatma (Geri Tuşu İçin)
    public void ClosePanels()
    {
        settingsPanel.SetActive(false);
        creditsPanel.SetActive(false);
        if (startPanel != null) startPanel.SetActive(true);
    }

    // Ses Ayarı
    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
    }

    // Oyundan Çıkış
    public void ExitGame()
    {
        Debug.Log("Oyun Kapatılıyor..."); // Editörde görmek için
        Application.Quit();
    }

    // --- DİĞER FONKSİYONLAR ---

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
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}