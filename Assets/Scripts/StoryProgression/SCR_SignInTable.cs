using System.Collections;
using UnityEngine;

public class SCR_SignInTable : MonoBehaviour
{
    [Header("References")]
    public SCR_DialogueManager dialogueManager; // Drag 'DialogueManager' here
    public SCR_JeepneyExit jeepneyExit;         // Drag 'LeftJeepneyDoor_Inte...' here

    [Header("Dialogues")]
    public SCR_DialogueData signInDialogue;          // Played the first time Jeff signs in
    public SCR_DialogueData alreadySignedInDialogue; // Optional: Played if he interacts again

    [Header("Interaction")]
    public KeyCode interactKey = KeyCode.E;

    private bool playerInZone = false;
    private bool hasSignedIn = false;
    private bool isInteracting = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<SCR_PlayerControl>() != null)
        {
            playerInZone = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<SCR_PlayerControl>() != null)
        {
            playerInZone = false;
        }
    }

    private void Update()
    {
        if (playerInZone && !isInteracting && Input.GetKeyDown(interactKey))
        {
            // Prevent starting a new dialogue if one is already open
            if (dialogueManager != null && dialogueManager.IsDialogueActive)
                return;

            StartCoroutine(SignInRoutine());
        }
    }

    private IEnumerator SignInRoutine()
    {
        isInteracting = true;

        if (!hasSignedIn)
        {
            if (dialogueManager != null && signInDialogue != null)
            {
                dialogueManager.StartDialogue(signInDialogue);
                yield return null;
                yield return new WaitUntil(() => !dialogueManager.IsDialogueActive);
            }

            hasSignedIn = true;

            // Unlock the jeepney door
            if (jeepneyExit != null)
            {
                jeepneyExit.UnlockJeepney();
            }
        }
        else if (dialogueManager != null && alreadySignedInDialogue != null)
        {
            dialogueManager.StartDialogue(alreadySignedInDialogue);
            yield return null;
            yield return new WaitUntil(() => !dialogueManager.IsDialogueActive);
        }

        // Brief cooldown so the same keypress doesn't immediately re-trigger
        yield return new WaitForSeconds(0.2f);
        isInteracting = false;
    }
}