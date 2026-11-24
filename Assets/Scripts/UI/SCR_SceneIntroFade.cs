using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[System.Serializable]
public struct CharacterTeleport
{
    public Transform characterTransform;
    public Transform targetSpawnPoint;
}

public class SCR_SceneIntroFade : MonoBehaviour
{
    [Header("Fade Settings")]
    [SerializeField] private Image blackFadeImage;
    [SerializeField] private float initialBlackDelay = 2f;
    [SerializeField] private float fadeDuration = 2.5f;

    [Header("Dialogue Timing")]
    [Tooltip("If checked, the screen stays black until the player finishes the dialogue. If unchecked, it fades in after the delay below.")]
    [SerializeField] private bool waitForDialogueToFinish = true;
    [SerializeField] private float delayAfterDialogueStarts = 1.5f;

    [Header("Dialogue References")]
    [SerializeField] private SCR_DialogueManager dialogueManager;
    [SerializeField] private SCR_DialogueData introDialogue;

    private void Start()
    {
        StartCoroutine(PlayIntroSequence());
    }

    private IEnumerator PlayIntroSequence()
    {
        if (blackFadeImage == null)
            yield break;

        // 1. Start completely black
        blackFadeImage.gameObject.SetActive(true);
        Color fadeColor = blackFadeImage.color;
        fadeColor.a = 1f;
        blackFadeImage.color = fadeColor;

        // 2. Hold on the black screen for a few seconds
        yield return new WaitForSeconds(initialBlackDelay);

        // 3. Show the intro dialogue
        if (dialogueManager != null && introDialogue != null)
        {
            dialogueManager.StartDialogue(introDialogue);

            if (waitForDialogueToFinish)
            {
                // Stay black until the dialogue box closes
                yield return new WaitUntil(() => !dialogueManager.IsDialogueActive);
            }
            else
            {
                // Wait a moment while the text is on screen, then start fading in
                yield return new WaitForSeconds(delayAfterDialogueStarts);
            }
        }

        // 4. Fade the scene in from black to clear
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float fadeProgress = Mathf.Clamp01(elapsedTime / fadeDuration);

            fadeColor.a = 1f - fadeProgress;
            blackFadeImage.color = fadeColor;

            yield return null;
        }

        // 5. Finish completely clear and disable the panel
        fadeColor.a = 0f;
        blackFadeImage.color = fadeColor;
        blackFadeImage.gameObject.SetActive(false);
    }


    public void FadeToBlack(string targetSceneName = "")
    {
        StartCoroutine(FadeToBlackRoutine(targetSceneName));
    }

    private IEnumerator FadeToBlackRoutine(string targetSceneName)
    {
        if (blackFadeImage == null)
            yield break;

        // 1. Re-enable the panel starting completely transparent
        blackFadeImage.gameObject.SetActive(true);
        Color fadeColor = blackFadeImage.color;
        fadeColor.a = 0f;
        blackFadeImage.color = fadeColor;

        // 2. Fade alpha from 0 (clear) to 1 (solid black)
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float fadeProgress = Mathf.Clamp01(elapsedTime / fadeDuration);

            fadeColor.a = fadeProgress;
            blackFadeImage.color = fadeColor;

            yield return null;
        }

        // 3. Ensure it finishes completely black
        fadeColor.a = 1f;
        blackFadeImage.color = fadeColor;

        // 4. Optional: load the next scene once the screen is black
        if (!string.IsNullOrEmpty(targetSceneName))
        {
            SceneManager.LoadScene(targetSceneName);
        }
    }

    public void FadeToBlackAfterDialogue(string targetSceneName = "")
    {
        StartCoroutine(FadeAfterDialogueRoutine(targetSceneName));
    }

    private IEnumerator FadeAfterDialogueRoutine(string targetSceneName)
    {
        if (dialogueManager != null)
        {
            yield return new WaitUntil(() => !dialogueManager.IsDialogueActive);
        }

        yield return StartCoroutine(FadeToBlackRoutine(targetSceneName));
    }

    public void SwitchCamera(GameObject currentCamera, GameObject targetCamera)
    {
        if (currentCamera != null)
            currentCamera.SetActive(false);

        if (targetCamera != null)
            targetCamera.SetActive(true);
    }

    public void FadeAndSwitchCamera(
        GameObject currentCamera,
        GameObject targetCamera,
        CharacterTeleport[] charactersToMove = null,
        float blackHoldDuration = 0.25f)
    {
        StartCoroutine(FadeAndSwitchCameraRoutine(
            currentCamera,
            targetCamera,
            charactersToMove,
            blackHoldDuration));
    }

    private IEnumerator FadeAndSwitchCameraRoutine(
        GameObject currentCamera,
        GameObject targetCamera,
        CharacterTeleport[] charactersToMove,
        float blackHoldDuration)
    {
        if (blackFadeImage == null)
            yield break;

        // 1. Fade from clear to black
        blackFadeImage.gameObject.SetActive(true);
        Color fadeColor = blackFadeImage.color;
        fadeColor.a = 0f;
        blackFadeImage.color = fadeColor;

        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            fadeColor.a = Mathf.Clamp01(elapsedTime / fadeDuration);
            blackFadeImage.color = fadeColor;
            yield return null;
        }

        fadeColor.a = 1f;
        blackFadeImage.color = fadeColor;

        // 2. Swap cameras and move all listed characters while screen is black
        SwitchCamera(currentCamera, targetCamera);

        if (charactersToMove != null)
        {
            foreach (CharacterTeleport teleport in charactersToMove)
            {
                if (teleport.characterTransform != null && teleport.targetSpawnPoint != null)
                {
                    teleport.characterTransform.position = teleport.targetSpawnPoint.position;
                }
            }
        }

        // 3. Brief pause on black
        if (blackHoldDuration > 0f)
        {
            yield return new WaitForSeconds(blackHoldDuration);
        }

        // 4. Fade from black back to clear
        elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            fadeColor.a = 1f - Mathf.Clamp01(elapsedTime / fadeDuration);
            blackFadeImage.color = fadeColor;
            yield return null;
        }

        fadeColor.a = 0f;
        blackFadeImage.color = fadeColor;
        blackFadeImage.gameObject.SetActive(false);
    }

    public void FadeAndSwitchCameraAfterDialogue(
        GameObject currentCamera,
        GameObject targetCamera,
        CharacterTeleport[] charactersToMove = null,
        float blackHoldDuration = 0.25f)
    {
        StartCoroutine(FadeAndSwitchCameraAfterDialogueRoutine(
            currentCamera,
            targetCamera,
            charactersToMove,
            blackHoldDuration));
    }

    private IEnumerator FadeAndSwitchCameraAfterDialogueRoutine(
        GameObject currentCamera,
        GameObject targetCamera,
        CharacterTeleport[] charactersToMove,
        float blackHoldDuration)
    {
        if (dialogueManager != null)
        {
            yield return new WaitUntil(() => !dialogueManager.IsDialogueActive);
        }

        yield return StartCoroutine(FadeAndSwitchCameraRoutine(
            currentCamera,
            targetCamera,
            charactersToMove,
            blackHoldDuration));
    }
}