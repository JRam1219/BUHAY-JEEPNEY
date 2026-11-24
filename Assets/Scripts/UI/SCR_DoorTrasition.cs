using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SCR_DoorTransition : MonoBehaviour
{
    public enum TransitionMode
    {
        SameScene,
        NewScene
    }

    [Header("Transition Mode")]
    public TransitionMode transitionMode = TransitionMode.SameScene;

    [Header("Same Scene Settings")]
    public Transform targetSpawnPoint;
    public GameObject cameraToDisable;
    public GameObject cameraToEnable;

    [Header("New Scene Settings")]
    public string targetSceneName;

    [Header("Fade Settings")]
    public Image blackFadePanel;
    public float fadeDuration = 0.5f;

    private bool isTransitioning = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isTransitioning) return;

        SCR_PlayerControl player = other.GetComponent<SCR_PlayerControl>();
        if (player != null)
        {
            StartCoroutine(TransitionRoutine(player));
        }
    }

    private IEnumerator TransitionRoutine(SCR_PlayerControl player)
    {
        isTransitioning = true;

        // Freeze Jeff during the fade
        player.enabled = false;
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = Vector2.zero;

        // 1. Fade to black
        yield return StartCoroutine(Fade(0f, 1f));

        if (transitionMode == TransitionMode.NewScene)
        {
            // 2A. Load the new scene (the new scene's SCR_SceneIntroFade handles fading back in)
            if (!string.IsNullOrEmpty(targetSceneName))
            {
                SCR_PlayerControl.savedFacingDirection = player.facingDirection;
                SCR_PlayerControl.hasSavedDirection = true;
                SceneManager.LoadScene(targetSceneName);
            }
            else
            {
                Debug.LogWarning("SCR_DoorTransition: Target Scene Name is empty!");
                yield return StartCoroutine(Fade(1f, 0f));
                player.enabled = true;
                isTransitioning = false;
            }
            yield break;
        }

        // 2B. Same Scene: Teleport Jeff & swap cameras
        if (targetSpawnPoint != null)
        {
            player.transform.position = targetSpawnPoint.position;
        }
        if (cameraToDisable != null) cameraToDisable.SetActive(false);
        if (cameraToEnable != null) cameraToEnable.SetActive(true);

        yield return new WaitForSeconds(0.15f);

        // 3. Fade back in
        yield return StartCoroutine(Fade(1f, 0f));

        // Unfreeze Jeff
        player.enabled = true;
        isTransitioning = false;
    }

    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        if (blackFadePanel == null) yield break;

        blackFadePanel.gameObject.SetActive(true);
        Color color = blackFadePanel.color;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, endAlpha, elapsed / fadeDuration);
            blackFadePanel.color = color;
            yield return null;
        }

        color.a = endAlpha;
        blackFadePanel.color = color;

        if (endAlpha <= 0f)
        {
            blackFadePanel.gameObject.SetActive(false);
        }
    }
}
