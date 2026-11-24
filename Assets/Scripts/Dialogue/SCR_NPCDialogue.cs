using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SCR_NPCDialogue : MonoBehaviour
{
    [Header("Dialogue References")]
    public SCR_DialogueManager dialogueManager;
    public SCR_DialogueData mainDialogue;
    public SCR_DialogueData repeatDialogue;

    [Header("Interaction Settings")]
    public GameObject interactPrompt;
    public bool lockPlayerMovement = true;

    [Header("Post-Dialogue Exit & Fade")]
    public bool walkAwayAndFadeAfterDialogue = false;
    public Transform[] exitWaypoints;
    public float walkSpeed = 2f;
    public bool hideOnArrival = true;
    public float delayBeforeFade = 0.6f;
    public Image fadeImage;
    public float fadeDuration = 1.5f;
    public string nextSceneName = "";

    [Header("Optional NPC Animator")]
    public Animator npcAnimator;
    public string isMovingBool = "isMoving";
    public string moveXFloat = "MoveX";
    public string moveYFloat = "MoveY";

    private bool playerInRange = false;
    private bool hasTalkedOnce = false;
    private bool isTalking = false;
    private SCR_PlayerControl cachedPlayer;

    private void Awake()
    {
        if (npcAnimator == null)
        {
            npcAnimator = GetComponent<Animator>();
        }
    }

    private void Start()
    {
        if (interactPrompt != null)
        {
            interactPrompt.SetActive(false);
        }
    }

    private void Update()
    {
        if (!playerInRange || isTalking) return;
        if (dialogueManager != null && dialogueManager.IsDialogueActive) return;

        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            Interact();
        }
    }

    public void Interact()
    {
        if (isTalking) return;
        if (dialogueManager != null && dialogueManager.IsDialogueActive) return;

        SCR_DialogueData chosenDialogue = (!hasTalkedOnce || repeatDialogue == null)
            ? mainDialogue
            : repeatDialogue;

        if (dialogueManager != null && chosenDialogue != null)
        {
            StartCoroutine(RunDialogueRoutine(chosenDialogue));
        }
    }

    private IEnumerator RunDialogueRoutine(SCR_DialogueData dataToPlay)
    {
        isTalking = true;

        if (interactPrompt != null)
        {
            interactPrompt.SetActive(false);
        }

        if (lockPlayerMovement && cachedPlayer != null)
        {
            cachedPlayer.enabled = false;
            Rigidbody2D rb = cachedPlayer.GetComponent<Rigidbody2D>();
            if (rb != null) rb.linearVelocity = Vector2.zero;
        }

        dialogueManager.StartDialogue(dataToPlay);

        yield return null;
        while (dialogueManager.IsDialogueActive)
        {
            yield return null;
        }

        hasTalkedOnce = true;

        if (walkAwayAndFadeAfterDialogue)
        {
            yield return StartCoroutine(ExitAndFadeRoutine());
            yield break;
        }

        isTalking = false;

        if (lockPlayerMovement && cachedPlayer != null)
        {
            cachedPlayer.enabled = true;
        }

        if (playerInRange && interactPrompt != null)
        {
            interactPrompt.SetActive(true);
        }
    }

    private IEnumerator ExitAndFadeRoutine()
    {
        // Disable solid colliders on Emma so Jeff cannot block her exit path
        foreach (Collider2D col in GetComponents<Collider2D>())
        {
            if (!col.isTrigger)
            {
                col.enabled = false;
            }
        }

        Vector2 lastDir = Vector2.down;

        if (exitWaypoints != null)
        {
            for (int i = 0; i < exitWaypoints.Length; i++)
            {
                Transform target = exitWaypoints[i];
                if (target == null) continue;

                while (Vector2.Distance(transform.position, target.position) > 0.02f)
                {
                    Vector2 dir = ((Vector2)target.position - (Vector2)transform.position).normalized;
                    if (dir.sqrMagnitude > 0.001f)
                    {
                        lastDir = dir;
                    }

                    SetWalkAnimation(true, lastDir);

                    transform.position = Vector2.MoveTowards(
                        transform.position,
                        target.position,
                        walkSpeed * Time.deltaTime
                    );

                    yield return null;
                }

                transform.position = target.position;
            }
        }

        SetWalkAnimation(false, lastDir);

        // Hide Emma's sprite once she reaches the door without stopping this coroutine
        if (hideOnArrival)
        {
            foreach (SpriteRenderer sr in GetComponentsInChildren<SpriteRenderer>())
            {
                sr.enabled = false;
            }
        }

        // Brief pause leaving Jeff and Marielle alone in the room
        if (delayBeforeFade > 0f)
        {
            yield return new WaitForSeconds(delayBeforeFade);
        }

        // Fade screen to black
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

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }

    private void SetWalkAnimation(bool moving, Vector2 dir)
    {
        if (npcAnimator == null) return;

        if (HasParameter(isMovingBool, AnimatorControllerParameterType.Bool))
        {
            npcAnimator.SetBool(isMovingBool, moving);
        }
        if (HasParameter(moveXFloat, AnimatorControllerParameterType.Float))
        {
            npcAnimator.SetFloat(moveXFloat, dir.x);
        }
        if (HasParameter(moveYFloat, AnimatorControllerParameterType.Float))
        {
            npcAnimator.SetFloat(moveYFloat, dir.y);
        }
    }

    private bool HasParameter(string paramName, AnimatorControllerParameterType type)
    {
        if (string.IsNullOrEmpty(paramName)) return false;

        foreach (AnimatorControllerParameter param in npcAnimator.parameters)
        {
            if (param.name == paramName && param.type == type)
                return true;
        }
        return false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        SCR_PlayerControl player = other.GetComponentInParent<SCR_PlayerControl>();
        if (other.CompareTag("Player") || player != null)
        {
            playerInRange = true;
            if (player != null) cachedPlayer = player;

            if (interactPrompt != null && !isTalking)
            {
                interactPrompt.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        SCR_PlayerControl player = other.GetComponentInParent<SCR_PlayerControl>();
        if (other.CompareTag("Player") || player != null)
        {
            playerInRange = false;
            if (interactPrompt != null)
            {
                interactPrompt.SetActive(false);
            }
        }
    }
}