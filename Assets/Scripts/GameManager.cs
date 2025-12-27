using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance; // Heryerden ulaşmak için

    [Header("UI Elemanları")]
    public TMP_Text scoreText;
    public GameObject gameOverPanel;
    public GameObject startPanel;
    public TMP_Text finalScoreText;
    public TMP_Text highScoreText;

    private int currentScore = 0;
    private int highScore = 0;

    void Awake()
    {
        // Singleton yapısı (Her yerden GameManager.instance diye ulaşabilmek için)
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // Oyun başlayınca Game Over kapalı olsun
        if (gameOverPanel != null) gameOverPanel.SetActive(false);

        // Eğer Start Panelimiz varsa...
        if (startPanel != null)
        {
            startPanel.SetActive(true); // Paneli aç
            Time.timeScale = 0; // Zamanı DURDUR (Oyun başlamasın)
        }
        else
        {
            // Panel yoksa direkt başlat (Eski mantık)
            Time.timeScale = 1;
        }

        highScore = PlayerPrefs.GetInt("HighScore", 0);
        UpdateScoreUI();
    }

    public void StartGame()
    {
        if (startPanel != null) startPanel.SetActive(false); // Paneli kapat
        Time.timeScale = 1; // Zamanı AKIT (Oyun başlasın)
    }

    public void AddScore(int amount)
    {
        currentScore += amount;

        // Eğer rekor kırdıysak kaydet
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
        // Paneli aç
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            finalScoreText.text = "Score: " + currentScore;
            highScoreText.text = "High Score: " + highScore;
        }

        // Oyunu dondur
        Time.timeScale = 0;
    }

    public void RestartGame()
    {
        // Sahneyi baştan yükle
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}