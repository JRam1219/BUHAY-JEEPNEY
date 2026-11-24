using UnityEngine;

public class SCR_NPCDialogueTrigger : MonoBehaviour
{
    [Header("References")]
    public SCR_DialogueManager dialogueManager; // Drag 'DialogueManager' here
    public SCR_DialogueData dialogueData;       // Drag 'DLG_GasStation-At...' here

    [Header("Interaction Settings")]
    public KeyCode interactKey = KeyCode.E;
    public bool triggerAutomatically = false;   // Check this to start dialogue as soon as the jeep pulls up
    public bool playOnce = false;               // Prevents re-triggering after it plays once

    [Header("Unlock After Dialogue")]
    public GameObject barrierToRemove; // Drag 'GasExitBarrier' here

    private bool inZone = false;
    private bool hasPlayed = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Trigger hit by: " + other.name);

        if (IsPlayerOrJeep(other))
        {
            inZone = true;

            if (triggerAutomatically)
            {
                TryStartDialogue();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (IsPlayerOrJeep(other))
        {
            inZone = false;
        }
    }

    private void Update()
    {
        if (inZone && !triggerAutomatically && Input.GetKeyDown(interactKey))
        {
            TryStartDialogue();
        }
    }

    private void TryStartDialogue()
    {
        Debug.Log($"TryStartDialogue | Manager:{dialogueManager != null} | Data:{dialogueData != null} | Active:{dialogueManager?.IsDialogueActive}");

        if (playOnce && hasPlayed)
            return;

        if (dialogueManager == null || dialogueData == null)
            return;

        // Don't interrupt if a dialogue is already open
        if (dialogueManager.IsDialogueActive)
            return;

        hasPlayed = true;
        dialogueManager.StartDialogue(dialogueData);

        if (barrierToRemove != null)
        {
            barrierToRemove.SetActive(false);
        }
    }

    private bool IsPlayerOrJeep(Collider2D other)
    {
        return other.GetComponent<SCR_PlayerControl>() != null ||
               other.GetComponent<SCR_JeepneyDriveAnim>() != null;
    }
}