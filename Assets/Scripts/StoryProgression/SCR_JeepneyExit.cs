using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SCR_JeepneyExit : MonoBehaviour
{
    [Header("Dialogue Before Entering")]
    public SCR_DialogueManager dialogueManager; // Drag 'DialogueManager' here
    public SCR_DialogueData preEnterDialogue;   // Drag your DLG_... asset here

    [Header("References")]
    public Transform jeepneyTransform; // Drag your 'Jeepney' object here
    public Image blackFadePanel;       // Optional: Drag 'BlackFadePanel' here

    [Header("Interaction")]
    public KeyCode interactKey = KeyCode.E;

    [Header("Engine & Reverse Animation")]
    public float startupDuration = 0.6f;
    public float startupShakeAmount = 0.03f;
    public float reverseDistance = 6f;
    public float maxReverseSpeed = 3.5f;
    public float acceleration = 2.5f;
    public float drivingRumbleAmount = 0.015f;

    [Header("Next Scene (Optional)")]
    public string nextSceneName;
    public float fadeDuration = 0.7f;

    private bool playerInZone = false;
    private bool hasTriggered = false;
    private GameObject playerObj;

    [Header("Sign-In Requirement")]
    public bool hasSignedIn = false;
    public SCR_DialogueData notSignedInDialogue; // Optional: "I need to sign in first."

    public void UnlockJeepney()
    {
        hasSignedIn = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<SCR_PlayerControl>() != null)
        {
            playerInZone = true;
            playerObj = other.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<SCR_PlayerControl>() != null)
        {
            playerInZone = false;
            playerObj = null;
        }
    }

    private void Update()
    {
        if (playerInZone && !hasTriggered && Input.GetKeyDown(interactKey))
        {
            // Don't interrupt if a dialogue is already open
            if (dialogueManager != null && dialogueManager.IsDialogueActive)
                return;

            if (!hasSignedIn)
            {
                // Play reminder dialogue if assigned, then stop here
                if (dialogueManager != null && notSignedInDialogue != null)
                {
                    dialogueManager.StartDialogue(notSignedInDialogue);
                }
                return;
            }

            hasTriggered = true;
            StartCoroutine(EnterAndBackupRoutine());
        }
    }

    private IEnumerator EnterAndBackupRoutine()
    {
        // 1. Play Jeff's dialogue before getting in
        if (dialogueManager != null && preEnterDialogue != null)
        {
            dialogueManager.StartDialogue(preEnterDialogue);

            // Wait one frame so DialogueManager sets its active state
            yield return null;

            // Wait until the dialogue finishes
            yield return new WaitUntil(() => !dialogueManager.IsDialogueActive);

            yield return new WaitForSeconds(0.2f);
        }

        // 2. Hide Jeff (he's inside the jeepney now)
        if (playerObj != null)
        {
            playerObj.SetActive(false);
        }

        Vector3 startPos = jeepneyTransform.position;
        float baseX = startPos.x;

        // 3. Engine startup rumble
        float elapsed = 0f;
        while (elapsed < startupDuration)
        {
            elapsed += Time.deltaTime;
            float shakeX = Mathf.Sin(elapsed * 60f) * startupShakeAmount;
            jeepneyTransform.position = new Vector3(baseX + shakeX, startPos.y, startPos.z);
            yield return null;
        }

        // 4. Reverse downward with smooth acceleration + subtle rumble
        float traveled = 0f;
        float currentSpeed = 0f;
        float currentY = startPos.y;

        while (traveled < reverseDistance)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, maxReverseSpeed, acceleration * Time.deltaTime);
            float step = currentSpeed * Time.deltaTime;

            currentY -= step;
            traveled += step;

            float rumbleX = Mathf.Sin(Time.time * 45f) * drivingRumbleAmount;
            jeepneyTransform.position = new Vector3(baseX + rumbleX, currentY, startPos.z);

            yield return null;
        }

        // 5. Optional: Fade to black and load the next scene
        if (blackFadePanel != null && !string.IsNullOrEmpty(nextSceneName))
        {
            blackFadePanel.gameObject.SetActive(true);
            Color c = blackFadePanel.color;
            float fadeElapsed = 0f;

            while (fadeElapsed < fadeDuration)
            {
                fadeElapsed += Time.deltaTime;
                c.a = Mathf.Lerp(0f, 1f, fadeElapsed / fadeDuration);
                blackFadePanel.color = c;
                yield return null;
            }

            SceneManager.LoadScene(nextSceneName);
        }
    }
}