using UnityEngine;
using UnityEngine.InputSystem;

public class JeffPlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriterenderer;

    public bool IsMoving { get; private set; }
    private Rigidbody2D rb;
    private Vector2 movementInput;
    private int lastmovement;
    private bool isMoving;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    
    private void Update()
    {
        // Read WASD input.
        movementInput.x = 0f;
        movementInput.y = 0f;
        if (Input.anyKey)
        {
            IsMoving = movementInput != Vector2.zero;

            if (Input.GetKey(KeyCode.A))
            {
                movementInput.x = -1f;
                spriterenderer.flipX = true;
                animator.SetInteger("Direction", 3);
                lastmovement = 8;
            }

            if (Input.GetKey(KeyCode.D))
            {   
                movementInput.x = 1f;
                spriterenderer.flipX = false;
                animator.SetInteger("Direction", 3);
                lastmovement = 3;
            }

            if (Input.GetKey(KeyCode.W))
            {   
                movementInput.y = 1f;
                animator.SetInteger("Direction", 4);
                lastmovement = 4;
            }

            if (Input.GetKey(KeyCode.S))
            {   
                movementInput.y = -1f;
                animator.SetInteger("Direction", 1);
                lastmovement = 1;
            }
        }
        else
        {
            isMoving = false;
        }

        if (isMoving == false)
        {
            
            if (lastmovement == 8)
            {
                animator.SetInteger("Direction", 2);
            }
            if (lastmovement == 3)
            {
                animator.SetInteger("Direction", 2);
            }
            if (lastmovement == 4)
            {
                animator.SetInteger("Direction", 5);
            }
            if (lastmovement == 1)
            {
                animator.SetInteger("Direction", 0);
            }
        }
        movementInput = movementInput.normalized;
    }
    
    private void FixedUpdate()
    {
        rb.linearVelocity = movementInput * moveSpeed;
    }
}
