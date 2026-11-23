using UnityEngine;

public class SCR_SpriteIdleBounce : MonoBehaviour
{
    public enum IdleStyle { BreatheScale, BobPosition }

    [Header("Idle Settings")]
    public IdleStyle style = IdleStyle.BreatheScale;
    public float speed = 3f;          // How fast the cycle loops
    public float amount = 0.04f;      // Try 0.03-0.06 for scale, or 0.05-0.1 for position
    public bool randomizeStart = true; // Keeps multiple NPCs from syncing up

    private Vector3 startPos;
    private Vector3 startScale;
    private float timeOffset;

    private void Start()
    {
        startPos = transform.localPosition;
        startScale = transform.localScale;
        timeOffset = randomizeStart ? Random.Range(0f, Mathf.PI * 2f) : 0f;
    }

    private void Update()
    {
        // Smooth wave between 0 and 1
        float wave = (Mathf.Sin(Time.time * speed + timeOffset) + 1f) * 0.5f;

        if (style == IdleStyle.BreatheScale)
        {
            float scaleY = startScale.y + (wave * amount);
            float scaleX = startScale.x - (wave * amount * 0.5f); // Subtle squash-and-stretch
            transform.localScale = new Vector3(scaleX, scaleY, startScale.z);
        }
        else
        {
            float offsetY = wave * amount;
            transform.localPosition = new Vector3(startPos.x, startPos.y + offsetY, startPos.z);
        }
    }
}
