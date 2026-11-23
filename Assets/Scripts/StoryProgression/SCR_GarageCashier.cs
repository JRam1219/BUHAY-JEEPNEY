using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class SCR_GarageCashier : MonoBehaviour
{
    [Header("References")]
    public SCR_DialogueManager dialogueManager;
    public SCR_GarageDoorBlock garageDoorBlock;

    [Header("Dialogue Data")]
    public SCR_DialogueData payBoundaryDialogue;
    public SCR_DialogueData alreadyPaidDialogue;

    [Header("Optional Prompt UI")]
    public GameObject interactPrompt;

    private bool playerInRange = false;
    private bool hasPaid = false;
    private bool isProcessingPayment = false;

    private void Start()
    {
        if (interactPrompt != null)
        {
            interactPrompt.SetActive(false);
        }
    }

    private void Update()
    {
        if (!playerInRange || isProcessingPayment) return;
        if (dialogueManager != null && dialogueManager.IsDialogueActive) return;

        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            Interact();
        }
    }

    public void Interact()
    {
        if (isProcessingPayment) return;
        if (dialogueManager != null && dialogueManager.IsDialogueActive) return;

        if (!hasPaid)
        {
            StartCoroutine(PayBoundaryRoutine());
        }
        else if (alreadyPaidDialogue != null && dialogueManager != null)
        {
            dialogueManager.StartDialogue(alreadyPaidDialogue);
        }
    }

    private IEnumerator PayBoundaryRoutine()
    {
        isProcessingPayment = true;

        if (interactPrompt != null)
        {
            interactPrompt.SetActive(false);
        }

        if (dialogueManager != null && payBoundaryDialogue != null)
        {
            dialogueManager.StartDialogue(payBoundaryDialogue);

            // Wait until the dialogue box closes before unlocking the exit
            yield return null;
            while (dialogueManager.IsDialogueActive)
            {
                yield return null;
            }
        }

        hasPaid = true;
        isProcessingPayment = false;

        if (garageDoorBlock != null)
        {
            garageDoorBlock.UnlockGarageDoor();
            Debug.Log("Boundary paid! Garage door unlocked.");
        }

        if (playerInRange && interactPrompt != null)
        {
            interactPrompt.SetActive(true);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.GetComponentInParent<SCR_PlayerControl>() != null)
        {
            playerInRange = true;
            if (interactPrompt != null && !isProcessingPayment)
            {
                interactPrompt.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.GetComponentInParent<SCR_PlayerControl>() != null)
        {
            playerInRange = false;
            if (interactPrompt != null)
            {
                interactPrompt.SetActive(false);
            }
        }
    }
}