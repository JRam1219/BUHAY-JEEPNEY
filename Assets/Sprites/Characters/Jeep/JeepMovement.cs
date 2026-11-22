using UnityEngine;

public class JeepMovement : MonoBehaviour
{
    public float moveSpeed = 4f;
    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 movement;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Freeze the jeep while dialogue is open
        if (DialogueManager.Instance != null && DialogueManager.Instance.isDialogueActive)
        {
            movement = Vector2.zero;
            if (animator != null) animator.speed = 0f;
            return;
        }

        movement.x = Input.GetAxisRaw("Horizontal"); // A and D
        movement.y = Input.GetAxisRaw("Vertical");   // W and S
        movement = movement.normalized;

        // Play the animation while moving, pause it when parked
        if (animator != null)
        {
            animator.speed = movement.sqrMagnitude > 0 ? 1f : 0f;
        }
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }
}