using UnityEngine;

public class ShelfInteractable : MonoBehaviour
{
    public Transform jeff;
    public float interactRange = 2.5f;
    public int itemCost = 0;

    public DialogueLine[] buyDialogue;
    public DialogueLine[] alreadyBoughtDialogue;

    private bool hasBought = false;

    void Update()
    {
        if (DialogueManager.Instance == null || jeff == null) return;
        if (DialogueManager.Instance.isDialogueActive) return;

        float distance = Vector2.Distance(transform.position, jeff.position);
        if (distance <= interactRange && Input.GetKeyDown(KeyCode.E))
        {
            if (!hasBought)
            {
                hasBought = true;
                if (StoreMoneyManager.Instance != null)
                {
                    StoreMoneyManager.Instance.SpendMoney(itemCost);
                }
                DialogueManager.Instance.StartDialogue(buyDialogue);
            }
            else if (alreadyBoughtDialogue != null && alreadyBoughtDialogue.Length > 0)
            {
                DialogueManager.Instance.StartDialogue(alreadyBoughtDialogue);
            }
        }
    }
}
