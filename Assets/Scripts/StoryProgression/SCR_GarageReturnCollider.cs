using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SCR_GarageReturnCollider : MonoBehaviour
{
    public string garageSceneName = "SCN_Sq5_Scn2";
    public float scrollSpeed = 10f;

    [Header("Screen Fade")]
    public Image fadeImage;
    public float fadeDuration = 1.5f;

    private bool isTransitioning = false;

    private void Update()
    {
        transform.Translate(Vector2.left * scrollSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isTransitioning) return;

        if (other.CompareTag("Player") || other.GetComponentInParent<SCR_JeepneySeatTracker>() != null)
        {
            isTransitioning = true;
            StartCoroutine(FadeAndLoadScene());
        }
    }

    private IEnumerator FadeAndLoadScene()
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
                elapsed += Time.deltaTime;
                c.a = Mathf.Clamp01(elapsed / fadeDuration);
                fadeImage.color = c;
                yield return null;
            }

            c.a = 1f;
            fadeImage.color = c;
        }

        SceneManager.LoadScene(garageSceneName);
    }
}