using UnityEngine;

public class SCR_JeepneyDriveAnim : MonoBehaviour
{
    [Header("References")]
    public Animator anim;

    [Header("Speed Settings")]
    public float maxSpeed = 8f;
    public float maxReverseSpeed = 3.5f; // Slower than forward driving
    public float acceleration = 4f;
    public float deceleration = 6f;

    [Header("Reverse Option")]
    public bool allowReverse = true;       // Toggle reversing on/off in the Inspector
    public bool reverseAnimation = true;   // If true, plays animation backward when reversing

    [Header("Animation Scaling")]
    public float maxAnimSpeed = 2f;

    private float currentSpeed = 0f;

    private void Update()
    {
        bool isForward = Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);
        bool isReversing = allowReverse && (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow));

        if (isForward)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, maxSpeed, acceleration * Time.deltaTime);
        }
        else if (isReversing)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, -maxReverseSpeed, acceleration * Time.deltaTime);
        }
        else
        {
            // Gradually coast to a stop from either direction
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, deceleration * Time.deltaTime);
        }

        // Move the jeepney (positive = right, negative = left)
        transform.Translate(Vector3.right * currentSpeed * Time.deltaTime);

        // Scale animation speed (negative value automatically plays the clip in reverse)
        if (anim != null)
        {
            bool isMoving = Mathf.Abs(currentSpeed) > 0.05f;
            anim.SetBool("isMoving", isMoving);

            float speedRatio = reverseAnimation
                ? (currentSpeed / maxSpeed)
                : (Mathf.Abs(currentSpeed) / maxSpeed);

            anim.SetFloat("AnimSpeed", speedRatio * maxAnimSpeed);
        }
    }
}