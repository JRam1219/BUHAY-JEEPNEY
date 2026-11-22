using UnityEngine;

public class GasAttendantInteractable : MonoBehaviour
{
    public Transform jeepPlayer;
    public float interactRange = 3.5f;
    public DialogueLine[] refuelDialogue;
    public DialogueLine[] afterRefuelDialogue;

    public bool hasRefueled = false;
    private bool waitingForRefuelToFinish = false;

    void Update()
    {
        if (DialogueManager.Instance == null || jeepPlayer == null) return;

        if (waitingForRefuelToFinish)
        {
            if (!DialogueManager.Instance.isDialogueActive)
            {
                waitingForRefuelToFinish = false;
                hasRefueled = true;
                Debug.Log("Diesel tank is full!");
            }
            return;
        }

        if (DialogueManager.Instance.isDialogueActive) return;

        float distance = Vector2.Distance(transform.position, jeepPlayer.position);
        if (distance <= interactRange && Input.GetKeyDown(KeyCode.E))
        {
            if (!hasRefueled)
            {
                DialogueManager.Instance.StartDialogue(refuelDialogue);
                waitingForRefuelToFinish = true;
            }
            else if (afterRefuelDialogue != null && afterRefuelDialogue.Length > 0)
            {
                DialogueManager.Instance.StartDialogue(afterRefuelDialogue);
            }
        }
    }
}