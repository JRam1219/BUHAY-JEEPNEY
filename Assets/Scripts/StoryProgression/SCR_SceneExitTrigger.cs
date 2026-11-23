using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SCR_SceneExitTrigger : MonoBehaviour
{
    [Header("Scene Transition")]
    public string nextSceneName;
    public Image blackFadePanel;       // Drag this scene's 'BlackFadePanel' here
    public float fadeDuration = 1.2f;

    private bool hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasTriggered)
            return;

        // Detects the Jeepney (or Jeff if reused in a walking scene)
        if (other.GetComponent<SCR_JeepneyDriveAnim>() != null ||
            other.GetComponent<SCR_PlayerControl>() != null)
        {
            hasTriggered = true;
            StartCoroutine(FadeAndLoadScene());
        }
    }

    private IEnumerator FadeAndLoadScene()
    {
        if (blackFadePanel != null)
        {
            blackFadePanel.gameObject.SetActive(true);
            Color c = blackFadePanel.color;
            float elapsed = 0f;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                c.a = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
                blackFadePanel.color = c;
                yield return null;
            }

            c.a = 1f;
            blackFadePanel.color = c;
        }

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}