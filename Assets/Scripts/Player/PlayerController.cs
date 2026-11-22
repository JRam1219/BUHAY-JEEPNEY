using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 4f;
    private Rigidbody2D rb;
    private Animator anim;
    private Vector2 moveInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        // Face down by default when the game starts
        anim.SetFloat("MoveX", 0f);
        anim.SetFloat("MoveY", -1f);
        anim.speed = 0f;
    }

    void Update()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        // Keep movement strictly 4-directional
        if (moveInput.x != 0) moveInput.y = 0;

        if (moveInput != Vector2.zero)
        {
            anim.speed = 1f; // Play walking animation
            anim.SetFloat("MoveX", moveInput.x);
            anim.SetFloat("MoveY", moveInput.y);
        }
        else
        {
            anim.speed = 0f; // Freeze on the current direction when stopped
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = moveInput * moveSpeed;
    }
}