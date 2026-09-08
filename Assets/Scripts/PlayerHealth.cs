using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxLives = 3;
    [SerializeField] private Image[] hearts;
    [SerializeField] private Sprite fullHeart;
    [SerializeField] private Sprite emptyHeart;
    [SerializeField] private float deathFreezeDelay = 1.5f;
    [SerializeField] private float invulnerableTime = 4f;

    [Header("Game Over")]
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private TMP_Text finalTimeText;
    [SerializeField] private TMP_Text bestTimeText;
    [SerializeField] private CanvasManager canvasManager;

    private int currentLives;
    private bool isDead = false;
    private float lastHitTime = -999f;

    public int CurrentLives => currentLives;
    public bool IsDead => isDead;
    public bool IsInvulnerable => Time.time < lastHitTime + invulnerableTime;

    void Start()
    {
        currentLives = maxLives;
        UpdateHeartsUI();

        if (gameOverUI != null) gameOverUI.SetActive(false);
    }

    public bool TakeDamage(int amount = 1)
    {
        if (isDead) return false;
        if (IsInvulnerable) return false;

        lastHitTime = Time.time;

        currentLives -= amount;
        currentLives = Mathf.Max(currentLives, 0);
        UpdateHeartsUI();

        if (currentLives <= 0)
        {
            Die();
        }

        return true;
    }

    void UpdateHeartsUI()
    {
        if (hearts == null) return;

        for (int i = 0; i < hearts.Length; i++)
        {
            if (hearts[i] == null) continue;
            hearts[i].sprite = i < currentLives ? fullHeart : emptyHeart;
        }
    }

    void Die()
    {
        isDead = true;

        if (canvasManager != null) canvasManager.StopTimer();

        RecordBestTime();
        Invoke(nameof(FreezeGame), deathFreezeDelay);
    }

    void RecordBestTime()
    {
        float survived = canvasManager != null ? canvasManager.ElapsedTime : 0f;
        float best = PlayerPrefs.GetFloat(StartMenu.BestTimeKey, 0f);

        if (survived > best)
        {
            best = survived;
            PlayerPrefs.SetFloat(StartMenu.BestTimeKey, best);
            PlayerPrefs.Save();
        }

        if (finalTimeText != null)
            finalTimeText.text = "Time: " + FormatTime(survived);

        if (bestTimeText != null)
            bestTimeText.text = "Best: " + FormatTime(best);
    }

    string FormatTime(float seconds)
    {
        int m = Mathf.FloorToInt(seconds / 60f);
        int s = Mathf.FloorToInt(seconds % 60f);
        return string.Format("{0:00}:{1:00}", m, s);
    }

    void FreezeGame()
    {
        if (gameOverUI != null) gameOverUI.SetActive(true);
        Time.timeScale = 0f;
    }

    public void RestartScene()
    {
        Time.timeScale = 1f;
        CanvasManager.gameIsPaused = false;
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }

    public void ResetHealth()
    {
        currentLives = maxLives;
        isDead = false;
        lastHitTime = -999f;
        UpdateHeartsUI();

        if (gameOverUI != null) gameOverUI.SetActive(false);
    }
}