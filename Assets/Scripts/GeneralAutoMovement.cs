
using UnityEngine;

public class GeneralAutoMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float stopDistance = 0.05f;

    private Vector2 targetPosition;
    private bool isMoving;

    private void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        if (animator == null)
            animator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        if (!isMoving)
            return;

        Vector2 currentPosition = rb.position;
        Vector2 direction = (targetPosition - currentPosition).normalized;

        if (Vector2.Distance(currentPosition, targetPosition) <= stopDistance)
        {
            rb.MovePosition(targetPosition);
            StopMovement();
            return;
        }

        rb.MovePosition(
            Vector2.MoveTowards(
                currentPosition,
                targetPosition,
                moveSpeed * Time.fixedDeltaTime
            )
        );

        UpdateAnimation(direction);
    }

    public void MoveTo(float x, float y)
    {
        targetPosition = new Vector2(x, y);
        isMoving = true;

        animator.SetBool("IsWalking", true);
    }

    private void UpdateAnimation(Vector2 direction)
    {
        animator.SetFloat("MoveX", direction.x);
        animator.SetFloat("MoveY", direction.y);
    }

    private void StopMovement()
    {
        isMoving = false;
        rb.linearVelocity = Vector2.zero;

        animator.SetBool("IsWalking", false);
        animator.SetFloat("MoveX", 0f);
        animator.SetFloat("MoveY", 0f);
    }

    public bool HasArrived()
    {
        return !isMoving;
    }
}
