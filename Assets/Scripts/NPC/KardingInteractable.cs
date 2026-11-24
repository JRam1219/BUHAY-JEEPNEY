using UnityEngine;

public class KardingInteractable : MonoBehaviour
{
    public Transform jeepPlayer;
    public float interactRange = 4.5f;
    public DialogueLine[] conversationLines;

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();

        if (jeepPlayer == null)
        {
            GameObject jeep = GameObject.Find("Jeep Player");
            if (jeep != null)
                jeepPlayer = jeep.transform;
        }
    }

    void Update()
    {
        if (jeepPlayer == null || DialogueManager.Instance == null)
            return;

        if (DialogueManager.Instance.isDialogueActive)
            return;

        float distance = Vector2.Distance(transform.position, jeepPlayer.position);

        if (distance <= interactRange && Input.GetKeyDown(KeyCode.E))
        {
            DialogueManager.Instance.StartDialogue(conversationLines);
        }
    }

    void FacePlayer()
    {
        Vector2 direction = jeepPlayer.position - transform.position;

        animator.SetFloat("DirectionX", 0);
        animator.SetFloat("DirectionY", 0);

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            animator.SetFloat("DirectionX", direction.x > 0 ? 1 : -1);
        else
            animator.SetFloat("DirectionY", direction.y > 0 ? 1 : -1);
    }
}