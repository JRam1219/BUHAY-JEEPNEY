using UnityEngine;

[RequireComponent(typeof(PoliceGuard))]
public class GuardVisionCone : MonoBehaviour
{
    public Color calmColor  = new Color(1f, 1f, 1f, 0.4f);
    public Color alertColor = new Color(1f, 0.15f, 0.1f, 0.45f);
    public int rays = 28;
    public int sortingOrder = 100;

    PoliceGuard guard;
    Mesh mesh;
    MeshRenderer mr;
    Transform cone;

    void Start()
    {
        guard = GetComponent<PoliceGuard>();

        var go = new GameObject(name + "_VisionCone");
        cone = go.transform;
        mesh = new Mesh { name = "VisionConeMesh" };
        go.AddComponent<MeshFilter>().sharedMesh = mesh;
        mr = go.AddComponent<MeshRenderer>();
        mr.material = new Material(Shader.Find("Sprites/Default"));
        mr.sortingOrder = sortingOrder;
    }

    void OnDestroy() { if (cone) Destroy(cone.gameObject); }
    void OnEnable()  { if (cone) cone.gameObject.SetActive(true); }
    void OnDisable() { if (cone) cone.gameObject.SetActive(false); }

    void LateUpdate()
    {
        if (!mesh || guard == null) return;

        // starts at his eyes/body instead of his feet
        Vector2 origin = guard.EyePos;
        cone.position = new Vector3(origin.x, origin.y, transform.position.z);
        mr.material.color = GameEvents.HasBook ? alertColor : calmColor;

        float facingAngle = Mathf.Atan2(guard.Facing.y, guard.Facing.x) * Mathf.Rad2Deg;
        float start = facingAngle - guard.viewAngle * 0.5f;
        float step = guard.viewAngle / rays;

        var verts = new Vector3[rays + 2];
        var tris = new int[rays * 3];
        verts[0] = Vector3.zero;

        for (int i = 0; i <= rays; i++)
        {
            float a = (start + step * i) * Mathf.Deg2Rad;
            Vector2 dir = new Vector2(Mathf.Cos(a), Mathf.Sin(a));
            RaycastHit2D hit = Physics2D.Raycast(origin, dir, guard.viewDistance, guard.obstacleMask);
            float len = hit.collider ? hit.distance : guard.viewDistance;
            verts[i + 1] = dir * len;

            if (i < rays)
            {
                tris[i * 3] = 0;
                tris[i * 3 + 1] = i + 1;
                tris[i * 3 + 2] = i + 2;
            }
        }

        mesh.Clear();
        mesh.vertices = verts;
        mesh.triangles = tris;
    }
}