using TMPro;
using UnityEngine;

public class StartMenu : MonoBehaviour
{
    [SerializeField] private GameObject startMenuUI;
    [SerializeField] private GameObject gameplayUI;
    [SerializeField] private TMP_Text bestTimeText;
    [SerializeField] private CanvasManager canvasManager;

    public const string BestTimeKey = "BestTimeSeconds";

    void Start()
    {
        Time.timeScale = 0f;
        CanvasManager.gameIsPaused = false;

        if (startMenuUI != null) startMenuUI.SetActive(true);
        if (gameplayUI != null) gameplayUI.SetActive(false);

        ShowBestTime();
    }

    void ShowBestTime()
    {
        if (bestTimeText == null) return;

        float best = PlayerPrefs.GetFloat(BestTimeKey, 0f);

        if (best <= 0f)
        {
            bestTimeText.text = "Best: --:--";
            return;
        }

        int minutes = Mathf.FloorToInt(best / 60f);
        int seconds = Mathf.FloorToInt(best % 60f);
        bestTimeText.text = string.Format("Best: {0:00}:{1:00}", minutes, seconds);
    }

    public void StartGame()
    {
        if (startMenuUI != null) startMenuUI.SetActive(false);
        if (gameplayUI != null) gameplayUI.SetActive(true);

        Time.timeScale = 1f;

        if (canvasManager != null) canvasManager.StartTimer();
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit pressed. This does nothing in the editor.");
    }
}