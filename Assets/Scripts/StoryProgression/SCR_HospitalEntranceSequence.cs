using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SCR_HospitalEntranceSequence : MonoBehaviour
{
    [Header("Fade-In Settings")]
    public Image fadeImage;
    public float fadeDuration = 1.2f;

    [Header("Jeff Entrance Walk")]
    public Transform jeffTransform;
    public Transform doorSpawnPoint;
    public Transform walkInTarget;
    public float walkSpeed = 2f;

    [Header("Optional Animator Parameters")]
    public Animator jeffAnimator;
    public string isMovingBool = "isMoving";
    public string moveXFloat = "MoveX";
    public string moveYFloat = "MoveY";

    private SCR_PlayerControl playerControl;
    private Rigidbody2D jeffRb;

    private void Awake()
    {
        // Start completely black on frame 1 so there is no flicker after scene load
        if (fadeImage != null)
        {
            fadeImage.gameObject.SetActive(true);
            Color c = fadeImage.color;
            c.a = 1f;
            fadeImage.color = c;
        }

        if (jeffTransform != null)
        {
            playerControl = jeffTransform.GetComponent<SCR_PlayerControl>();
            jeffRb = jeffTransform.GetComponent<Rigidbody2D>();

            if (doorSpawnPoint != null)
            {
                jeffTransform.position = doorSpawnPoint.position;
            }
        }
    }

    private void Start()
    {
        StartCoroutine(EntranceRoutine());
    }

    private IEnumerator EntranceRoutine()
    {
        // Lock player input while Jeff steps through the doors
        if (playerControl != null)
        {
            playerControl.enabled = false;
        }

        if (jeffRb != null)
        {
            jeffRb.linearVelocity = Vector2.zero;
        }

        Vector2 walkDir = Vector2.up;
        if (doorSpawnPoint != null && walkInTarget != null)
        {
            Vector2 diff = walkInTarget.position - doorSpawnPoint.position;
            if (diff.sqrMagnitude > 0.001f)
            {
                walkDir = diff.normalized;
            }
        }

        SetWalkAnimation(true, walkDir);

        float elapsed = 0f;
        bool hasArrived = (jeffTransform == null || walkInTarget == null);

        // Fade from black to clear while Jeff walks into the hospital
        while (elapsed < fadeDuration || !hasArrived)
        {
            if (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                if (fadeImage != null)
                {
                    Color c = fadeImage.color;
                    c.a = 1f - Mathf.Clamp01(elapsed / fadeDuration);
                    fadeImage.color = c;
                }
            }

            if (!hasArrived)
            {
                jeffTransform.position = Vector2.MoveTowards(
                    jeffTransform.position,
                    walkInTarget.position,
                    walkSpeed * Time.deltaTime
                );

                if (Vector2.Distance(jeffTransform.position, walkInTarget.position) <= 0.02f)
                {
                    jeffTransform.position = walkInTarget.position;
                    hasArrived = true;
                    SetWalkAnimation(false, walkDir);
                }
            }

            yield return null;
        }

        SetWalkAnimation(false, walkDir);

        if (fadeImage != null)
        {
            Color c = fadeImage.color;
            c.a = 0f;
            fadeImage.color = c;
            fadeImage.gameObject.SetActive(false);
        }

        // Return movement control to the player
        if (playerControl != null)
        {
            playerControl.enabled = true;
        }
    }

    private void SetWalkAnimation(bool moving, Vector2 dir)
    {
        if (jeffAnimator == null) return;

        if (HasParameter(isMovingBool, AnimatorControllerParameterType.Bool))
        {
            jeffAnimator.SetBool(isMovingBool, moving);
        }
        if (HasParameter(moveXFloat, AnimatorControllerParameterType.Float))
        {
            jeffAnimator.SetFloat(moveXFloat, dir.x);
        }
        if (HasParameter(moveYFloat, AnimatorControllerParameterType.Float))
        {
            jeffAnimator.SetFloat(moveYFloat, dir.y);
        }
    }

    private bool HasParameter(string paramName, AnimatorControllerParameterType type)
    {
        if (string.IsNullOrEmpty(paramName)) return false;

        foreach (AnimatorControllerParameter param in jeffAnimator.parameters)
        {
            if (param.name == paramName && param.type == type)
                return true;
        }
        return false;
    }
}