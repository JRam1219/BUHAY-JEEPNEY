using UnityEngine;

public class NPCInteractable : MonoBehaviour
{
    public float interactRange = 1.8f;
    public DialogueLine[] conversationLines;

    private Transform player;
    private float cooldown;

    void Start()
    {
        GameObject jeffObj = GameObject.Find("Jeff");
        if (jeffObj != null) player = jeffObj.transform;
    }

    void Update()
    {
        if (DialogueManager.Instance.isDialogueActive)
        {
            cooldown = 0.2f;
            return;
        }

        if (cooldown > 0f)
        {
            cooldown -= Time.deltaTime;
            return;
        }

        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);
        if (distance <= interactRange && Input.GetKeyDown(KeyCode.E))
        {
            DialogueManager.Instance.StartDialogue(conversationLines);
        }
    }
}