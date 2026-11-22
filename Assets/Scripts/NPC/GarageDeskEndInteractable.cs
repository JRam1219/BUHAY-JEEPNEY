using UnityEngine;

public class GarageDeskEndInteractable : MonoBehaviour
{
    public Transform jeff;
    public float interactRange = 3f;

    public DialogueLine[] payBoundaryDialogue;
    public DialogueLine[] signOutDialogue;
    public DialogueLine[] afterSignOutDialogue;

    public bool hasPaidBoundary = false;
    public bool hasSignedOut = false;

    void Update()
    {
        if (DialogueManager.Instance == null || jeff == null) return;
        if (DialogueManager.Instance.isDialogueActive) return;

        float distance = Vector2.Distance(transform.position, jeff.position);
        if (distance <= interactRange && Input.GetKeyDown(KeyCode.E))
        {
            if (!hasPaidBoundary)
            {
                hasPaidBoundary = true;
                DialogueManager.Instance.StartDialogue(payBoundaryDialogue);
            }
            else if (!hasSignedOut)
            {
                hasSignedOut = true;
                DialogueManager.Instance.StartDialogue(signOutDialogue);
            }
            else if (afterSignOutDialogue != null && afterSignOutDialogue.Length > 0)
            {
                DialogueManager.Instance.StartDialogue(afterSignOutDialogue);
            }
        }
    }
}