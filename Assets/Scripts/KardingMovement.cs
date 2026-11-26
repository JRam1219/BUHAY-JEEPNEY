
using UnityEngine;

public class KardingMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform mayor;
    [SerializeField] private Animator animator;

    [Header("Movement")]
    [SerializeField] private float walkSpeed = 2f;
    [SerializeField] private float stopDistance = 0f;
    [SerializeField] private GameObject jeff;
    private float walkOutTargetX;
    private float walkOutTargetY;
    [SerializeField] private float walkOutDistance = 5f;
    public bool HasArrived { get; private set; }



    private bool isWalking;

    public void WalkToMayor()
    {
        HasArrived = false;
        isWalking = true;

        animator.SetBool("KardingIsWalking", true);

        walkOutTargetX = mayor.localPosition.x;
        walkOutTargetY = transform.localPosition.y;
    }

    private void Update()
    {
        if (!isWalking)
            return;

        Vector2 targetPosition = new Vector2(walkOutTargetX, walkOutTargetY);

        transform.localPosition = Vector2.MoveTowards(
            transform.localPosition,
            targetPosition,
            walkSpeed * Time.deltaTime
        );

        if (Vector2.Distance(transform.localPosition, targetPosition) <= stopDistance)
        {
            StopWalking();
        }
    }

    private void StopWalking()
    {
        isWalking = false;
        HasArrived = true;

        animator.SetBool("KardingIsWalking", false);
        animator.SetBool("KardingIsSideIdle", true);
    }

    public void WalkToPosition()
    {
        HasArrived = false;
        isWalking = true;

        animator.SetBool("KardingIsWalkingBack", true);

        walkOutTargetX = -2.3f;
        walkOutTargetY = -3.8f;
    }

    public void WalkOut()
    {
        HasArrived = false;
        isWalking = true;

        animator.SetBool("KardingIsWalking", true);

        float direction = Mathf.Sign(transform.localPosition.x - mayor.localPosition.x);

        walkOutTargetX = mayor.localPosition.x + (direction * walkOutDistance);
        walkOutTargetY = transform.localPosition.y;
    }


}
