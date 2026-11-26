
using UnityEngine;

public class PoliceGuard : MonoBehaviour
{
    [Header("Vision Cone")]
    public GameObject visionCone;

    [Header("Patrol")]
    public Transform[] waypoints;
    public float speed = 2.5f;
    public float waitTime = 0f;

    [Header("Chase Route")]
    public Transform[] chaseWaypoints;

    [Header("Movement Style")]
    public bool axisByAxis = true;
    public bool verticalFirst = false;

    [Header("Animation")]
    public float walkAnimSpeed = 1.3f;
    public float horizontalBias = 1.5f;

    [Header("Look Around")]
    public bool lookAroundOnStop = true;
    public bool turnClockwise = true;

    [Header("Vision")]
    public float viewDistance = 4f;

    [Range(10, 180)]
    public float viewAngle = 70f;

    public LayerMask obstacleMask;
    public Vector2 eyeOffset = new Vector2(0f, 0.5f);

    public Vector2 EyePos =>
        (Vector2)transform.position + eyeOffset;

    [Header("Alert")]
    public float chaseSpeed = 3.5f;

    public Vector2 Facing { get; private set; } = Vector2.up;

    private Transform player;
    private Animator anim;
    private Rigidbody2D rb;

    private int index;
    private int chaseWaypointIndex;

    private float waitTimer;
    private float waitDuration;

    private Vector2 lookStartFacing;

    private int lastDirId = -1;
    private float animUnfreeze;

    private bool alert;
    private bool arrived;
    private bool stoppedAfterCatch;

    private Vector3 startPos;

    private void OnEnable()
    {
        GameEvents.BookStolen += OnBookStolen;
        GameEvents.PlayerRespawned += ResetGuard;
    }

    private void OnDisable()
    {
        GameEvents.BookStolen -= OnBookStolen;
        GameEvents.PlayerRespawned -= ResetGuard;
    }

    private void Start()
    {
        startPos = transform.position;

        rb = GetComponent<Rigidbody2D>();

        anim = GetComponent<Animator>();

        if (anim == null)
            anim = GetComponentInChildren<Animator>();

        if (rb == null)
        {
            Debug.LogError("PoliceGuard needs a Rigidbody2D!");
            enabled = false;
            return;
        }

        var p = GameObject.FindWithTag("Player");

        if (p != null)
            player = p.transform;

        SetDirection(Facing);
    }

    // Physics movement
    private void FixedUpdate()
    {
        if (rb == null) return;

        // Stop after catching Jeff with the book.
        if (stoppedAfterCatch)
        {
            if (anim != null)
                anim.speed = 0f;

            return;
        }

        if (alert)
        {
            if (player != null)
                Chase();
        }
        else
        {
            Patrol();
        }
    }

    // Vision checking
    
private void Update()
{
    if (stoppedAfterCatch) return;
    if (player == null) return;

    // Before stealing the book:
    // Police catches Jeff through normal vision.
    if (!GameEvents.HasBook)
    {
        if (CanSeePlayer())
        {
            GameEvents.Spotted();
        }
    }
    else
    {
        // After stealing the book:
        // Only catch Jeff when the police gets close.
        float distance = Vector2.Distance(
            transform.position,
            player.position
        );

        if (distance < 0.35f)
        {
            Debug.Log("POLICE CAUGHT JEFF!");
            GameEvents.Spotted();
        }
    }
}

    // NORMAL PATROL
    private void Patrol()
    {
        if (waypoints == null || waypoints.Length == 0)
            return;

        if (waitTimer > 0f)
        {
            waitTimer -= Time.fixedDeltaTime;

            if (lookAroundOnStop && waitDuration > 0f)
                LookAround();
            else if (anim != null)
                anim.speed = 0f;

            return;
        }

        Transform point = waypoints[index];

        if (point == null)
        {
            Debug.LogWarning(
                "Waypoint " + index + " is empty on " + name
            );

            index = (index + 1) % waypoints.Length;
            return;
        }

        Vector2 toPoint = (Vector2)point.position - rb.position;

        if (toPoint.magnitude < 0.05f)
        {
            if (!arrived)
            {
                arrived = true;

                var stair = point.GetComponent<StairPoint>();

                float wait = stair != null
                    ? stair.waitTime
                    : waitTime;

                if (wait > 0f)
                {
                    waitTimer = wait;
                    waitDuration = wait;

                    lookStartFacing = Facing;
                    return;
                }
            }

            index = (index + 1) % waypoints.Length;
            arrived = false;
            return;
        }

        Vector3 target = axisByAxis
            ? GetAxisTarget(point.position)
            : point.position;

        MoveTowardsTarget(target, speed);
    }

    // SEPARATE CHASE ROUTE
    private void Chase()
    {
        // Follow chase waypoints first.
        if (chaseWaypoints != null &&
            chaseWaypoints.Length > 0 &&
            chaseWaypointIndex < chaseWaypoints.Length)
        {
            Transform point = chaseWaypoints[chaseWaypointIndex];

            if (point == null)
            {
                Debug.LogWarning(
                    "Chase waypoint " + chaseWaypointIndex +
                    " is empty on " + name
                );

                chaseWaypointIndex++;
                return;
            }

            Vector2 distance =
                (Vector2)point.position - rb.position;

            // Reached current chase waypoint.
            if (distance.magnitude < 0.05f)
            {
                chaseWaypointIndex++;
                return;
            }

            Vector3 target = axisByAxis
                ? GetAxisTarget(point.position)
                : point.position;

            MoveTowardsTarget(target, chaseSpeed);
            return;
        }

        // After the final waypoint, follow Jeff.
        Vector3 playerTarget = axisByAxis
            ? GetAxisTarget(player.position)
            : player.position;

        MoveTowardsTarget(playerTarget, chaseSpeed);
    }

    // Shared movement function
    private void MoveTowardsTarget(Vector3 target, float moveSpeed)
    {
        Vector2 stepDir = (Vector2)target - rb.position;

        if (stepDir.sqrMagnitude > 0.000001f)
        {
            Facing = stepDir.normalized;
            SetDirection(Facing);
        }

        if (anim != null)
            anim.speed = walkAnimSpeed;

        Vector2 nextPosition = Vector2.MoveTowards(
            rb.position,
            (Vector2)target,
            moveSpeed * Time.fixedDeltaTime
        );

        rb.MovePosition(nextPosition);
    }

    // Move along one axis first
    private Vector3 GetAxisTarget(Vector3 dest)
    {
        Vector3 pos = new Vector3(
            rb.position.x,
            rb.position.y,
            transform.position.z
        );

        const float eps = 0.05f;

        bool needX = Mathf.Abs(dest.x - pos.x) > eps;
        bool needY = Mathf.Abs(dest.y - pos.y) > eps;

        if (needX && needY)
        {
            if (verticalFirst)
                return new Vector3(pos.x, dest.y, pos.z);
            else
                return new Vector3(dest.x, pos.y, pos.z);
        }

        if (needX)
            return new Vector3(dest.x, pos.y, pos.z);

        if (needY)
            return new Vector3(pos.x, dest.y, pos.z);

        return dest;
    }

    // Look around while stopped
    private void LookAround()
    {
        float progress = 1f - Mathf.Clamp01(
            waitTimer / waitDuration
        );

        float startAngle = Mathf.Atan2(
            lookStartFacing.y,
            lookStartFacing.x
        ) * Mathf.Rad2Deg;

        float angle = startAngle +
            (turnClockwise ? -360f : 360f) * progress;

        float rad = angle * Mathf.Deg2Rad;

        Facing = new Vector2(
            Mathf.Cos(rad),
            Mathf.Sin(rad)
        );

        SetDirection(Facing);

        if (anim != null)
        {
            if (animUnfreeze > 0f)
                animUnfreeze -= Time.fixedDeltaTime;
            else
                anim.speed = 0f;
        }
    }

    // 0 = Up
    // 1 = Right
    // 2 = Down
    // 3 = Left
    private void SetDirection(Vector2 d)
    {
        if (anim == null) return;

        int dirId;

        if (Mathf.Abs(d.x) > Mathf.Abs(d.y) * horizontalBias)
        {
            dirId = d.x > 0 ? 1 : 3;
        }
        else
        {
            dirId = d.y > 0 ? 0 : 2;
        }

        if (dirId != lastDirId)
        {
            lastDirId = dirId;

            animUnfreeze = 0.1f;
            anim.speed = walkAnimSpeed;
        }

        anim.SetInteger("Direction", dirId);
    }

    // Vision detection
    private bool CanSeePlayer()
    {
        Vector2 eye = EyePos;

        Vector2 to = (Vector2)player.position - eye;

        if (to.magnitude > viewDistance)
            return false;

        if (Vector2.Angle(Facing, to) > viewAngle * 0.5f)
            return false;

        if (Physics2D.Raycast(
            eye,
            to.normalized,
            to.magnitude,
            obstacleMask
        ))
            return false;

        return true;
    }

    // Jeff steals the book.
    private void OnBookStolen()
    {
        alert = true;
        stoppedAfterCatch = false;

        // Start the separate chase route.
        chaseWaypointIndex = 0;

        Debug.Log("PoliceGuard started the chase route!");
    }

 
    // Reset after respawn
    private void ResetGuard()
    {
        // Jeff was caught AFTER stealing the book.
        if (GameEvents.HasBook)
        {
            stoppedAfterCatch = true;
            alert = false;

            if (anim != null)
                anim.speed = 0f;

            // Make the vision cone invisible.
            if (visionCone != null)
                visionCone.SetActive(false);

            Debug.Log("PoliceGuard stopped after catching Jeff!");
            return;
        }

        // Jeff was caught BEFORE stealing the book.
        stoppedAfterCatch = false;
        alert = false;

        // Make the vision cone visible again.
        if (visionCone != null)
            visionCone.SetActive(true);

        // Reset police position.
        if (rb != null)
            rb.position = new Vector2(startPos.x, startPos.y);

        transform.position = startPos;

        // Reset patrol and chase.
        index = 0;
        chaseWaypointIndex = 0;

        waitTimer = 0f;
        waitDuration = 0f;

        arrived = false;

        Facing = Vector2.up;
        lastDirId = -1;

        if (anim != null)
            anim.speed = walkAnimSpeed;

        SetDirection(Facing);
    }
}
