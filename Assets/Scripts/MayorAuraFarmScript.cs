
using UnityEngine;
using System.Collections;
using Unity.VisualScripting.ReorderableList;

public class MayorAuraFarmScript: MonoBehaviour
{
    [SerializeField] public float moveDistanceLeft = 0f;
    [SerializeField] private float moveDistanceRight = 0f;
    [SerializeField] public float walkSpeed = 0f;

    [SerializeField] private float idleDuration = 0f;
    [SerializeField] private Animator animator;
    [SerializeField] private string isWalkingParameter = "IsWalking";
    [SerializeField] private string isGoingIdleParameter = "IsGoingIdle";

    [SerializeField] private string IsSideIdleParameter = "IsSideIdle";

    [SerializeField] private float leftBoundary = 0f;

    private Vector3 startingPosition;
    private Vector3 leftPosition;
    private Vector3 rightPosition;

    private Coroutine movementCoroutine;
    public SpriteRenderer spriteRenderer;

    private void Start()
    {
        startingPosition = transform.position;

        leftPosition = startingPosition + Vector3.left * moveDistanceLeft;
        rightPosition = startingPosition + Vector3.right * moveDistanceRight;

        spriteRenderer = GetComponent<SpriteRenderer>();

        movementCoroutine = StartCoroutine(MovementCycle());
    }

    private IEnumerator MovementCycle()
    {
        while (true)
        {
            // Idle
            animator.SetBool(isWalkingParameter, false);
            yield return new WaitForSeconds(idleDuration);

            // Walk right
            spriteRenderer.flipX = false;
            animator.SetBool(isWalkingParameter, true);
            yield return MoveTo(rightPosition);

            // Walk left
            spriteRenderer.flipX = true;
            yield return MoveTo(leftPosition);

            // Return to starting position
            spriteRenderer.flipX = false;
            yield return MoveTo(startingPosition);

            // Idle again
            animator.SetBool(isWalkingParameter, false);
            animator.SetBool(isGoingIdleParameter, true);
            yield return new WaitForSeconds(idleDuration);
            animator.SetBool(isGoingIdleParameter, false);
        }
    }

    private IEnumerator MoveTo(Vector3 target)
    {
        while (Vector3.Distance(transform.position, target) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                target,
                walkSpeed * Time.deltaTime
            );

            yield return null;
        }

        transform.position = target;
    }

    public void FallBack()
    {
        // Stop the normal movement cycle
        if (movementCoroutine != null)
        {
            StopCoroutine(movementCoroutine);
            movementCoroutine = null;
        }

        spriteRenderer.flipX = true;
        walkSpeed = 0.5f;

        // Fixed fallback destination
        Vector3 fallbackPosition = transform.parent.TransformPoint(
    new Vector3(
        leftBoundary,
        transform.localPosition.y,
        transform.localPosition.z
    )
);

        // Reset animation parameters
        animator.SetBool(isGoingIdleParameter, false);
        animator.SetBool(isWalkingParameter, true);

        StartCoroutine(FallbackMovement(fallbackPosition));
    }

    private IEnumerator FallbackMovement(Vector3 target)
    {
        // Keep walking until the target is reached
        yield return MoveTo(target);

        // Stop walking only after movement finishes
        animator.SetBool(isWalkingParameter, false);
        spriteRenderer.flipX = false;
        animator.SetBool(IsSideIdleParameter, true);
    }
}
