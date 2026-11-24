
using System.Collections;
using UnityEngine;

public class GasAttendantInteractable : MonoBehaviour
{
    public Transform jeepPlayer;
    public float interactRange = 3.5f;
    public DialogueLine[] refuelDialogue;
    public DialogueLine[] afterRefuelDialogue;

    public float refuelDuration = 60f;
    public bool hasRefueled = false;

    private bool waitingForRefuelToFinish = false;
    private bool isRefueling = false;

    void Update()
    {
        if (DialogueManager.Instance == null || jeepPlayer == null) return;

        if (waitingForRefuelToFinish)
        {
            if (!DialogueManager.Instance.isDialogueActive)
            {
                waitingForRefuelToFinish = false;
                StartCoroutine(RefuelJeep());
            }
            return;
        }

        if (isRefueling || DialogueManager.Instance.isDialogueActive) return;

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

    IEnumerator RefuelJeep()
    {
        isRefueling = true;

        Debug.Log("Refueling started. Please wait 60 seconds.");

        yield return new WaitForSeconds(refuelDuration);

        hasRefueled = true;
        isRefueling = false;

        Debug.Log("Diesel tank is full!");
    }
}
