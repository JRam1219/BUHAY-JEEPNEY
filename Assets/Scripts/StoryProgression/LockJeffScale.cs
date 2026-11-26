using UnityEngine;

// Put on PF_Jeff. Keeps Jeff the same size no matter what else tries to change it.
public class LockJeffScale : MonoBehaviour
{
    public bool includeChildren = false;   // turn on if the shrinking turns out to be on a child object of Jeff

    Transform[] targets;
    Vector3[] baseScales;

    void Awake()
    {
        targets = includeChildren ? GetComponentsInChildren<Transform>(true) : new[] { transform };
        baseScales = new Vector3[targets.Length];
        for (int i = 0; i < targets.Length; i++) baseScales[i] = targets[i].localScale;
    }

    void LateUpdate()
    {
        for (int i = 0; i < targets.Length; i++)
        {
            if (!targets[i]) continue;
            Vector3 cur = targets[i].localScale;
            Vector3 b = baseScales[i];
            // keep the original size, but let a left/right flip (negative X) through
            float sx = cur.x < 0 ? -Mathf.Abs(b.x) : Mathf.Abs(b.x);
            float sy = cur.y < 0 ? -Mathf.Abs(b.y) : Mathf.Abs(b.y);
            targets[i].localScale = new Vector3(sx, sy, b.z);
        }
    }
}