using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SCR_Scene5Exit : MonoBehaviour
{
    [Header("Scene")]
    [SerializeField] private string nextSceneName = "SCENE6.SQ6"; // must match your hospital scene exactly
    [SerializeField] private CanvasGroup fadePanel;               // optional: BlackFadePanel + Canvas Group
    [SerializeField] private float fadeDuration = 0.7f;

    [Header("Sign-Out Requirement")]
    [SerializeField] private bool requireSignOut = true;
    [SerializeField] private bool hasSignedOut = false;
    [SerializeField] private SCR_DialogueManager dialogueManager;
    [SerializeField] private SCR_DialogueData notSignedOutDialogue; // optional

    private bool isLeaving;

    // Call this from the cashier/desk when Jeff finishes signing out
    public void SetSignedOut()
    {
        hasSignedOut = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isLeaving || !other.CompareTag("Player")) return;

        if (requireSignOut && !hasSignedOut)
        {
            if (dialogueManager != null && notSignedOutDialogue != null
                && !dialogueManager.IsDialogueActive)
            {
                dialogueManager.StartDialogue(notSignedOutDialogue);
            }
            return;
        }

        isLeaving = true;
        StartCoroutine(FadeAndLoad());
    }

    private IEnumerator FadeAndLoad()
    {
        if (fadePanel != null)
        {
            fadePanel.gameObject.SetActive(true);
            float t = 0f;
            while (t < fadeDuration)
            {
                t += Time.unscaledDeltaTime;
                fadePanel.alpha = Mathf.Clamp01(t / fadeDuration);
                yield return null;
            }
            fadePanel.alpha = 1f;
        }

        SceneManager.LoadScene(nextSceneName);
    }
}