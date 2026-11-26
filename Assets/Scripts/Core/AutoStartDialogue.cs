using System.Collections;
using UnityEngine;

public class SCR_AutoStartDialogue : MonoBehaviour
{
    [Header("Assign these")]
    public SCR_DialogueManager dialogueManager;
    public SCR_DialogueData dialogueToPlay;

    [Header("Settings")]
    public float delay = 2f;      // wait so the scene fade-in finishes first
    public bool playOnce = true;

    static bool hasPlayed = false;

    IEnumerator Start()
    {
        if (playOnce && hasPlayed) yield break;

        yield return new WaitForSeconds(delay);

        if (dialogueManager == null || dialogueToPlay == null)
        {
            Debug.LogWarning("AutoStartDialogue: manager or dialogue is not assigned.");
            yield break;
        }

        hasPlayed = true;
        dialogueManager.StartDialogue(dialogueToPlay);
    }
}