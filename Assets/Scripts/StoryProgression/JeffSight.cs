using UnityEngine;

// Put on PF_Jeff. Jeff gets caught when he and a guard can see each other.
// The guard's own vision cone (PoliceGuard) still works as before; this adds Jeff's side.
public class JeffSight : MonoBehaviour
{
    [Header("Jeff's field of view")]
    public float sightDistance = 4f;
    [Range(10, 180)] public float sightAngle = 90f;
    public LayerMask obstacleMask;          // same Walls layer as the guard's Obstacle Mask

    [Header("Bumping into the guard")]
    public bool catchOnTouch = true;
    public float touchDistance = 0.6f;

    Vector2 facing = Vector2.down;
    Vector3 lastPos;
    PoliceGuard[] guards;

    void Start()
    {
        lastPos = transform.position;
        guards = FindObjectsByType<PoliceGuard>(FindObjectsSortMode.None);
    }

    void Update()
    {
        // Jeff faces whichever way he last moved
        Vector2 moved = transform.position - lastPos;
        if (moved.sqrMagnitude > 0.00001f && moved.magnitude < 1f) facing = moved.normalized;
        lastPos = transform.position;

        foreach (var g in guards)
        {
            if (g == null || !g.isActiveAndEnabled) continue;

            Vector2 to = g.transform.position - transform.position;
            float dist = to.magnitude;

            if (catchOnTouch && dist <= touchDistance) { GameEvents.Spotted(); return; }

            if (dist > sightDistance) continue;
            if (Vector2.Angle(facing, to) > sightAngle * 0.5f) continue;
            if (Physics2D.Raycast(transform.position, to.normalized, dist, obstacleMask)) continue;

            GameEvents.Spotted();   // CaughtManager handles the rest
            return;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Vector2 f = Application.isPlaying ? facing : Vector2.down;
        float half = sightAngle * 0.5f;
        Vector3 a = Quaternion.Euler(0, 0, half) * f * sightDistance;
        Vector3 b = Quaternion.Euler(0, 0, -half) * f * sightDistance;
        Gizmos.DrawLine(transform.position, transform.position + a);
        Gizmos.DrawLine(transform.position, transform.position + b);
        Gizmos.DrawLine(transform.position + a, transform.position + b);
    }
}