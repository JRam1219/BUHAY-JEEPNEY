
using UnityEngine;
using System.Collections;

public class ScreenShake : MonoBehaviour
{
    [Header("Screen Shake Settings")]
    [SerializeField] private float shakeDuration = 0.3f;
    [SerializeField] private float shakeMagnitude = 0.15f;

    private Vector3 originalPosition;
    private Coroutine shakeCoroutine;

    private void Awake()
    {
        originalPosition = transform.localPosition;
    }

    public void Shake()
    {
        // Prevent overlapping shake routines
        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
        }

        shakeCoroutine = StartCoroutine(ShakeEffect());
    }

    private IEnumerator ShakeEffect()
    {
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            Vector2 randomOffset = Random.insideUnitCircle * shakeMagnitude;

            transform.localPosition = originalPosition +
                new Vector3(randomOffset.x, randomOffset.y, 0f);

            elapsed += Time.deltaTime;

            yield return null;
        }

        // Return camera to its original position
        transform.localPosition = originalPosition;

        shakeCoroutine = null;
    }
}
