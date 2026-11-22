using UnityEngine;

public class PassengerInteractable : MonoBehaviour
{
    public Transform jeepPlayer;
    public float interactRange = 4.5f;
    public DialogueLine[] passengerDialogue;

    private bool hasTalked = false;
    private bool waitingForDialogueToFinish = false;

    void Update()
    {
        if (DialogueManager.Instance == null || jeepPlayer == null) return;

        // Wait until the dialogue box closes, then make the passenger disappear
        if (waitingForDialogueToFinish)
        {
            if (!DialogueManager.Instance.isDialogueActive)
            {
                waitingForDialogueToFinish = false;
                gameObject.SetActive(false);
            }
            return;
        }

        if (hasTalked || DialogueManager.Instance.isDialogueActive) return;

        float distance = Vector2.Distance(transform.position, jeepPlayer.position);
        if (distance <= interactRange && Input.GetKeyDown(KeyCode.E))
        {
            hasTalked = true;
            waitingForDialogueToFinish = true;
            DialogueManager.Instance.StartDialogue(passengerDialogue);
        }
    }
}