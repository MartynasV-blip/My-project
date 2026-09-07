using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxLives = 3;
    [SerializeField] private Image[] hearts;
    [SerializeField] private Sprite fullHeart;
    [SerializeField] private Sprite emptyHeart;
    [SerializeField] private float restartDelay = 1.5f;

    private int currentLives;
    private bool isDead = false;

    void Start()
    {
        currentLives = maxLives;
        UpdateHeartsUI();
    }

    public void TakeDamage(int amount = 1)
    {
        if (isDead) return;

        currentLives -= amount;
        currentLives = Mathf.Max(currentLives, 0);
        UpdateHeartsUI();

        if (currentLives <= 0)
        {
            Die();
        }
    }

    void UpdateHeartsUI()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].sprite = i < currentLives ? fullHeart : emptyHeart;
        }
    }

    void Die()
    {
        isDead = true;
        Debug.Log("Player died — restarting in " + restartDelay + "s");
        Invoke(nameof(RestartScene), restartDelay);
    }

    void RestartScene()
    {
        Time.timeScale = 1f;
        CanvasManager.gameIsPaused = false;
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }
}