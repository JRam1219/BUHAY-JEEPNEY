
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class Gamemanager : MonoBehaviour
{
    [Header("Ending UI")]
    [SerializeField] private Image fadeImage;
    [SerializeField] private TextMeshProUGUI postGameText;

    [Header("Post-Game Text")]
    [SerializeField] private string[] postGameTexts;

    [Header("Timing")]
    [SerializeField] private float fadeDuration = 2f;
    [SerializeField] private float textDisplayDuration = 4f;
    [SerializeField] private float endingDelay = 2f;

    private bool isEnding;

    private void Awake()
    {
        // Start with a transparent fade image.
        Color fadeColor = fadeImage.color;
        fadeColor.a = 0f;
        fadeImage.color = fadeColor;

        fadeImage.gameObject.SetActive(false);
        postGameText.gameObject.SetActive(false);
    }

    public void StartEnding()
    {
        if (isEnding)
            return;

        isEnding = true;
        StartCoroutine(EndingSequence());
    }

    private IEnumerator EndingSequence()
    {
        // Fade the screen to black.
        fadeImage.gameObject.SetActive(true);

        yield return StartCoroutine(FadeToBlack());

        // Display the post-game texts one by one.
        postGameText.gameObject.SetActive(true);

        for (int i = 0; i < postGameTexts.Length; i++)
        {
            postGameText.text = postGameTexts[i];

            yield return new WaitForSeconds(textDisplayDuration);
        }

        // Wait before the ending finishes.
        yield return new WaitForSeconds(endingDelay);

        postGameText.gameObject.SetActive(false);
    }

    private IEnumerator FadeToBlack()
    {
        float elapsed = 0f;

        Color fadeColor = fadeImage.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;

            float alpha = Mathf.Clamp01(elapsed / fadeDuration);

            fadeColor.a = alpha;
            fadeImage.color = fadeColor;

            yield return null;
        }

        fadeColor.a = 1f;
        fadeImage.color = fadeColor;
    }
}
