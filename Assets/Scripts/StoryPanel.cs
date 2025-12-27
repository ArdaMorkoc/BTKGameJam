using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

[System.Serializable]
public class StorySlide
{
    public Sprite image;
    public string subtitle;
    public AudioClip audioClip;
}

public class StoryPanel : MonoBehaviour
{
    [Header("Hikaye Slaytları")]
    public StorySlide[] storySlides;

    [Header("UI Elemanları")]
    public Image storyImageDisplay;
    public TextMeshProUGUI subtitleText;
    public TextMeshProUGUI skipText;
    public Button skipButton;

    [Header("Audio")]
    public AudioSource audioSource;

    [Header("Ayarlar")]
    public float imageDuration = 10f;
    public float skipTextStartTime = 3f;
    public float skipFadeDuration = 0.5f;
    public string nextSceneName = "GameScene";

    private int currentSlideIndex = 0;
    private float slideTimer = 0f;
    private bool canSkip = false;
    private bool sceneLoading = false;

    private CanvasGroup skipCanvasGroup;
    private Coroutine skipFadeCoroutine;
    private AsyncOperation preloadSceneOperation;

    void Start()
    {
        // Skip CanvasGroup
        skipCanvasGroup = skipText.GetComponent<CanvasGroup>();
        if (skipCanvasGroup == null)
            skipCanvasGroup = skipText.gameObject.AddComponent<CanvasGroup>();

        skipCanvasGroup.alpha = 0f;
        skipText.gameObject.SetActive(false);

        skipButton.onClick.AddListener(OnSkipButtonClicked);

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        // 🎯 SAHNEYİ ARKADA YÜKLE
        StartCoroutine(PreloadNextScene());

        ShowSlide(0);
    }

    void Update()
    {
        if (sceneLoading) return;

        slideTimer += Time.deltaTime;

        if (!canSkip && slideTimer >= skipTextStartTime)
        {
            ActivateSkipText();
        }

        if (slideTimer >= imageDuration)
        {
            NextSlide();
        }
    }

    IEnumerator PreloadNextScene()
    {
        preloadSceneOperation = SceneManager.LoadSceneAsync(nextSceneName);
        preloadSceneOperation.allowSceneActivation = false;

        while (preloadSceneOperation.progress < 0.9f)
            yield return null;

        // %90’da hazır, story bitmesini bekliyor
    }

    void ShowSlide(int index)
    {
        if (index < storySlides.Length)
        {
            StorySlide slide = storySlides[index];

            storyImageDisplay.sprite = slide.image;
            subtitleText.text = slide.subtitle;

            if (slide.audioClip != null)
            {
                audioSource.clip = slide.audioClip;
                audioSource.Play();
            }

            slideTimer = 0f;
            canSkip = false;

            HideSkipText();
        }
        else
        {
            FinishStory();
        }
    }

    void ActivateSkipText()
    {
        canSkip = true;
        skipText.gameObject.SetActive(true);

        if (skipFadeCoroutine != null)
            StopCoroutine(skipFadeCoroutine);

        skipFadeCoroutine = StartCoroutine(SkipFadePulse());
    }

    IEnumerator SkipFadePulse()
    {
        // Fade in
        yield return FadeSkip(0f, 1f);

        // Kısa bekleme
        yield return new WaitForSeconds(0.3f);

        // Fade out
        yield return FadeSkip(1f, 0f);
    }

    IEnumerator FadeSkip(float from, float to)
    {
        float t = 0f;
        while (t < skipFadeDuration)
        {
            t += Time.deltaTime;
            skipCanvasGroup.alpha = Mathf.Lerp(from, to, t / skipFadeDuration);
            yield return null;
        }
        skipCanvasGroup.alpha = to;
    }

    void HideSkipText()
    {
        if (skipFadeCoroutine != null)
            StopCoroutine(skipFadeCoroutine);

        skipCanvasGroup.alpha = 0f;
        skipText.gameObject.SetActive(false);
    }

    void OnSkipButtonClicked()
    {
        if (canSkip)
        {
            NextSlide();
        }
    }

    void NextSlide()
    {
        if (audioSource.isPlaying)
            audioSource.Stop();

        currentSlideIndex++;
        ShowSlide(currentSlideIndex);
    }

    void FinishStory()
    {
        sceneLoading = true;

        if (audioSource.isPlaying)
            audioSource.Stop();

        // 🎯 ŞAK DİYE SAHNE
        preloadSceneOperation.allowSceneActivation = true;
    }
}
