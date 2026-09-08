using TMPro;
using UnityEngine;

public class RagdollCountdownUI : MonoBehaviour
{
    [SerializeField] private Ragdoll playerRagdoll;
    [SerializeField] private TMP_Text countdownText;
    [SerializeField] private PlayerHealth playerHealth;

    void Start() {
        if (playerRagdoll == null)
            Debug.LogError($"{name}: Player Ragdoll is not assigned on RagdollCountdownUI.");

        if (countdownText == null)
            Debug.LogError($"{name}: Countdown Text is not assigned on RagdollCountdownUI.");
        else
            countdownText.gameObject.SetActive(false);
    }

    void Update() {
        if (playerRagdoll == null || countdownText == null) return;

        bool dead = playerHealth != null && playerHealth.IsDead;

        bool show = playerRagdoll.IsRagdolled
                    && playerRagdoll.RagdollTimeRemaining > 0f
                    && !dead;

        if (countdownText.gameObject.activeSelf != show)
            countdownText.gameObject.SetActive(show);

        if (show)
            countdownText.text = Mathf.CeilToInt(playerRagdoll.RagdollTimeRemaining).ToString();
    }
}