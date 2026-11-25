using UnityEngine;

// Put on Main Camera. Camera must be Orthographic (default for 2D).
public class FollowJeff : MonoBehaviour
{
    public Transform target;              // leave empty to auto-find the object tagged "Player"
    public float smoothTime = 0.12f;      // 0 = snap instantly, higher = floatier
    public Vector2 offset = Vector2.zero;

    [Header("Keep the camera inside the house (optional)")]
    public bool useBounds = true;
    public Transform boundsFrom;                        // drag HouseLayout here: bounds are measured from its sprites
    public Vector2 boundsMin;                           // only used if Bounds From is empty
    public Vector2 boundsMax;

    Vector3 velocity;
    Camera cam;

    void Awake() { cam = GetComponent<Camera>(); }

    void Start()
    {
        if (!target)
        {
            var p = GameObject.FindWithTag("Player");
            if (p) target = p.transform;
            else Debug.LogWarning("FollowJeff: no target set and nothing is tagged 'Player'.");
        }
        if (useBounds && boundsFrom) MeasureBounds();
        else if (useBounds) { Debug.LogWarning("FollowJeff: Use Bounds is on but Bounds From is empty and Min/Max are 0, so bounds are off."); }
        if (target) transform.position = Desired();   // start on Jeff, no sliding in
    }

    void LateUpdate()   // after Jeff has moved this frame, so there's no jitter
    {
        if (!target) return;
        transform.position = Vector3.SmoothDamp(transform.position, Desired(), ref velocity, smoothTime);
    }

    void MeasureBounds()
    {
        var rs = boundsFrom.GetComponentsInChildren<Renderer>();
        if (rs.Length == 0) return;
        Bounds b = rs[0].bounds;
        for (int i = 1; i < rs.Length; i++) b.Encapsulate(rs[i].bounds);
        boundsMin = b.min;
        boundsMax = b.max;
    }

    bool HasBounds => useBounds && (boundsMax - boundsMin).sqrMagnitude > 0.001f;

    Vector3 Desired()
    {
        Vector3 pos = new Vector3(target.position.x + offset.x, target.position.y + offset.y, transform.position.z);

        if (HasBounds && cam && cam.orthographic)
        {
            float halfH = cam.orthographicSize;
            float halfW = halfH * cam.aspect;

            // if the area is smaller than the view, center on it instead of clamping
            pos.x = (boundsMax.x - boundsMin.x) <= halfW * 2f
                ? (boundsMin.x + boundsMax.x) * 0.5f
                : Mathf.Clamp(pos.x, boundsMin.x + halfW, boundsMax.x - halfW);
            pos.y = (boundsMax.y - boundsMin.y) <= halfH * 2f
                ? (boundsMin.y + boundsMax.y) * 0.5f
                : Mathf.Clamp(pos.y, boundsMin.y + halfH, boundsMax.y - halfH);
        }
        return pos;
    }

    void OnDrawGizmosSelected()
    {
        if (!HasBounds) return;
        Gizmos.color = Color.cyan;
        Vector3 c = (boundsMin + boundsMax) * 0.5f;
        Gizmos.DrawWireCube(c, boundsMax - boundsMin);
    }
}