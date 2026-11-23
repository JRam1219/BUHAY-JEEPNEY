using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SCR_GarageDoorBlock : MonoBehaviour
{
    [Header("Dialogue References")]
    public SCR_DialogueManager dialogueManager;
    public SCR_DialogueData forgotToPayDialogue;

    [Header("Boundary Status")]
    public bool hasPaidBoundary = false;
    public float retriggerCooldown = 1.5f;

    [Header("Hospital Transition")]
    public string hospitalSceneName = "SCN_Hospital";
    public Image fadeImage;
    public float fadeDuration = 1.5f;

    private BoxCollider2D doorCollider;
    private float lastTriggerTime = -10f;
    private bool isTransitioning = false;

    private void Awake()
    {
        doorCollider = GetComponent<BoxCollider2D>();
    }

    private void Start()
    {
        if (doorCollider != null)
        {
            doorCollider.isTrigger = hasPaidBoundary;
        }
    }

    // Blocks Jeff and plays dialogue before the boundary is paid
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (hasPaidBoundary) return;

        if (collision.gameObject.CompareTag("Player") || collision.gameObject.GetComponentInParent<SCR_PlayerControl>() != null)
        {
            if (Time.time - lastTriggerTime < retriggerCooldown) return;
            lastTriggerTime = Time.time;

            if (dialogueManager != null && forgotToPayDialogue != null)
            {
                dialogueManager.StartDialogue(forgotToPayDialogue);
            }
        }
    }

    // Called by SCR_GarageCashier once Jeff finishes paying
    public void UnlockGarageDoor()
    {
        hasPaidBoundary = true;

        if (doorCollider != null)
        {
            doorCollider.isTrigger = true;
        }
    }

    // Triggers the black fade and loads the Hospital scene once paid
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!hasPaidBoundary || isTransitioning) return;

        SCR_PlayerControl player = other.GetComponentInParent<SCR_PlayerControl>();
        if (other.CompareTag("Player") || player != null)
        {
            isTransitioning = true;

            // Stop Jeff from walking while the screen fades
            if (player != null)
            {
                player.enabled = false;
            }

            StartCoroutine(FadeAndLoadHospital());
        }
    }

    private IEnumerator FadeAndLoadHospital()
    {
        if (fadeImage != null)
        {
            fadeImage.gameObject.SetActive(true);
            Color c = fadeImage.color;
            c.a = 0f;
            fadeImage.color = c;

            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                c.a = Mathf.Clamp01(elapsed / fadeDuration);
                fadeImage.color = c;
                yield return null;
            }

            c.a = 1f;
            fadeImage.color = c;
        }

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(hospitalSceneName);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}