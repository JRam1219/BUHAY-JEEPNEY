using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class SCR_PlayerControl : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Animator anim;
    private Vector2 moveInput;

    [Header("Movement Keys")]
    public KeyCode upKey = KeyCode.W;
    public KeyCode downkey = KeyCode.S;
    public KeyCode leftKey = KeyCode.A;
    public KeyCode rightKey = KeyCode.D;

    [Header("Interaction")]
    public Transform interactPoint;
    public float interactDistance = 0.6f;
    public float interactRadius = 0.25f;
    public LayerMask interactableLayer;
    public KeyCode interactKey = KeyCode.E;

    // Add these at the top of SCR_PlayerControl:
    public static Vector2 savedFacingDirection = Vector2.down;
    public static bool hasSavedDirection = false;

    // Make sure your existing facing direction variable is accessible (or expose it):
    public Vector2 facingDirection = Vector2.down;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        if (hasSavedDirection)
        {
            facingDirection = savedFacingDirection;
            hasSavedDirection = false;

            // Update Animator so Jeff immediately faces the right way
            if (anim != null)
            {
                anim.SetFloat("LastMoveX", facingDirection.x);
                anim.SetFloat("LastMoveY", facingDirection.y);
            }
        }
    }

    void Update()
    {
        moveInput = Vector2.zero;

        if (Input.GetKey(upKey))
        {
            moveInput.y += 1;
        }
        else if (Input.GetKey(downkey))
        {
            moveInput.y -= 1;
        }
        else if (Input.GetKey(leftKey))
        {
            moveInput.x -= 1;
        }
        else if (Input.GetKey(rightKey))
        {
            moveInput.x += 1;
        }

        bool isMoving = moveInput.magnitude > 0;
        anim.SetBool("isWalking", isMoving);

        if (isMoving)
        {
            anim.SetFloat("MoveX", moveInput.x);
            anim.SetFloat("MoveY", moveInput.y);
        }

        if (moveInput.x > 0)
        {
            // Facing right
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (moveInput.x < 0)
        {
            // Facing left
            transform.localScale = new Vector3(-1, 1, 1);
        }

        UpdateFacingDirection(moveInput);

        if (Input.GetKeyDown(interactKey))
        {
            TryInteract();
        }
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveInput.normalized * moveSpeed * Time.fixedDeltaTime);
    }

    private void UpdateFacingDirection(Vector2 currentMoveInput)
    {
        if (currentMoveInput.sqrMagnitude > 0.01f)
        {
            if (Mathf.Abs(currentMoveInput.x) > Mathf.Abs(currentMoveInput.y))
            {
                facingDirection = new Vector2(Mathf.Sign(currentMoveInput.x), 0f);
            }
            else
            {
                facingDirection = new Vector2(0f, Mathf.Sign(currentMoveInput.y));
            }

            if (interactPoint != null)
            {
                // Account for localScale.x flipping when facing left
                float adjustedX = facingDirection.x * interactDistance * Mathf.Sign(transform.localScale.x);
                float adjustedY = facingDirection.y * interactDistance;
                interactPoint.localPosition = new Vector3(adjustedX, adjustedY, 0f);
            }
        }
    }

    private void TryInteract()
    {
        if (interactPoint == null) return;

        Collider2D hit = Physics2D.OverlapCircle(interactPoint.position, interactRadius, interactableLayer);

        if (hit != null)
        {
            SCR_Interactable interactable = hit.GetComponent<SCR_Interactable>();
            if (interactable != null)
            {
                interactable.Interact();
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (interactPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(interactPoint.position, interactRadius);
        }
    }
}