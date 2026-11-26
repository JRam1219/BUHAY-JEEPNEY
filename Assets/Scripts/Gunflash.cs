
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Gunflash : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image flashImage;

    [Header("Flash Settings")]
    [SerializeField] private float flashDuration = 0.1f;

    private Coroutine flashCoroutine;

    private void Awake()
    {
        flashImage.color = Color.white;
        flashImage.enabled = false;
    }

    public void Flash()
    {
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }

        flashCoroutine = StartCoroutine(FlashEffect());
    }

    private IEnumerator FlashEffect()
    {
        flashImage.enabled = true;

        yield return new WaitForSeconds(flashDuration);

        flashImage.enabled = false;
        flashCoroutine = null;
    }
}
