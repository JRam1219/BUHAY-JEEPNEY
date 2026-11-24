using UnityEngine;

public class PassengerInteractable : MonoBehaviour
{
    public Transform jeepPlayer;
    public float interactRange = 4.5f;
    public DialogueLine[] passengerDialogue;

    [Header("Fare")]
    public int fareReward = 0;

    private bool hasTalked = false;
    private bool waitingForDialogueToFinish = false;

    void Update()
    {
        if (DialogueManager.Instance == null || jeepPlayer == null) return;

        if (waitingForDialogueToFinish)
        {
            if (!DialogueManager.Instance.isDialogueActive)
            {
                waitingForDialogueToFinish = false;

                if (MoneyManager.Instance != null && fareReward > 0)
                {
                    MoneyManager.Instance.AddMoney(fareReward);
                }

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