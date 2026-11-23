using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

// Press E near the bed: optional dialogue -> Jeff walks onto the bed and lies down
// -> screen fades to black -> "END OF DAY 2" fades in -> (optional) load the next scene.
// Same style as your other interactables (distance check + E, uses DialogueManager).
public class BedInteractable : MonoBehaviour
{
    public Transform jeff;                 // leave empty: finds the object named "Jeff"
    public float interactRange = 2f;

    [Header("Can Jeff sleep yet?")]
    public bool canSleep = true;           // untick for the morning scene
    public DialogueLine[] notTiredLines;   // shown if canSleep is false

    [Header("Sleeping")]
    public DialogueLine[] beforeSleepLines;
    public Transform sleepSpot;            // empty object on the pillow (Jeff's feet land here)
    public float walkSpeed = 2f;
    public float lieDownPause = 0.8f;      // seconds before the screen goes dark

    [Header("Screen going dark")]
    public CanvasGroup fade;               // black full-screen Image with a CanvasGroup (alpha 0)
    public float fadeTime = 2f;
    public TextMeshProUGUI endText;        // optional: "END OF DAY 2"
    public float endTextFadeTime = 1.5f;
    public float endTextHoldTime = 3f;

    [Header("After")]
    public string nextSceneName = "";      // leave blank to stay on the black screen

    private bool busy = false;
    private bool waitingForDialogue = false;

    void Start()
    {
        if (jeff == null)
        {
            GameObject j = GameObject.Find("Jeff");
            if (j != null) jeff = j.transform;
        }
        if (fade != null) fade.alpha = 0f;
        if (endText != null) endText.alpha = 0f;
    }

    public void UnlockSleep() { canSleep = true; }   // call from another script/event when the day is done

    void Update()
    {
        if (DialogueManager.Instance == null || jeff == null || busy) return;

        if (waitingForDialogue)
        {
            if (!DialogueManager.Instance.isDialogueActive)
            {
                waitingForDialogue = false;
                StartCoroutine(SleepSequence());
            }
            return;
        }

        if (DialogueManager.Instance.isDialogueActive) return;

        float distance = Vector2.Distance(transform.position, jeff.position);
        if (distance <= interactRange && Input.GetKeyDown(KeyCode.E))
        {
            if (!canSleep)
            {
                if (notTiredLines != null && notTiredLines.Length > 0)
                    DialogueManager.Instance.StartDialogue(notTiredLines);
                return;
            }

            if (beforeSleepLines != null && beforeSleepLines.Length > 0)
            {
                DialogueManager.Instance.StartDialogue(beforeSleepLines);
                waitingForDialogue = true;
            }
            else
            {
                StartCoroutine(SleepSequence());
            }
        }
    }

    IEnumerator SleepSequence()
    {
        busy = true;

        // take control away from the player
        PlayerController pc = jeff.GetComponent<PlayerController>();
        if (pc != null) pc.enabled = false;
        Rigidbody2D rb = jeff.GetComponent<Rigidbody2D>();
        if (rb != null) { rb.linearVelocity = Vector2.zero; rb.simulated = false; }
        Animator anim = jeff.GetComponent<Animator>();

        // walk onto the bed
        if (sleepSpot != null)
        {
            if (anim != null) anim.speed = 1f;
            while (Vector2.Distance(jeff.position, sleepSpot.position) > 0.03f)
            {
                Vector2 pos = jeff.position;
                Vector2 dir = (Vector2)sleepSpot.position - pos;
                if (anim != null)
                {
                    if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y)) { anim.SetFloat("MoveX", Mathf.Sign(dir.x)); anim.SetFloat("MoveY", 0f); }
                    else { anim.SetFloat("MoveX", 0f); anim.SetFloat("MoveY", Mathf.Sign(dir.y)); }
                }
                Vector2 next = Vector2.MoveTowards(pos, sleepSpot.position, walkSpeed * Time.deltaTime);
                jeff.position = new Vector3(next.x, next.y, jeff.position.z);
                yield return null;
            }
            jeff.position = new Vector3(sleepSpot.position.x, sleepSpot.position.y, jeff.position.z);
        }

        // lie down: top-down, head up = the "facing down" sprite, standing still
        if (anim != null)
        {
            anim.SetFloat("MoveX", 0f);
            anim.SetFloat("MoveY", -1f);
            anim.speed = 0f;
        }
        yield return new WaitForSeconds(lieDownPause);

        // screen goes dark
        if (fade != null)
        {
            float t = 0f;
            while (t < fadeTime)
            {
                t += Time.deltaTime;
                fade.alpha = Mathf.Clamp01(t / fadeTime);
                yield return null;
            }
            fade.alpha = 1f;
        }

        // END OF DAY text
        if (endText != null)
        {
            float t = 0f;
            while (t < endTextFadeTime)
            {
                t += Time.deltaTime;
                endText.alpha = Mathf.Clamp01(t / endTextFadeTime);
                yield return null;
            }
            endText.alpha = 1f;
            yield return new WaitForSeconds(endTextHoldTime);
        }

        if (!string.IsNullOrEmpty(nextSceneName))
            SceneManager.LoadScene(nextSceneName);
    }
}
