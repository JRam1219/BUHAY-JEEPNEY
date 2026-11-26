
using UnityEngine;
using UnityEngine.InputSystem;

public class TimingMinigame : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform timingBar;
    [SerializeField] private Transform target;
    [SerializeField] private Rigidbody2D marker;
    [SerializeField] private Collider2D targetCollider;
    [SerializeField] private GameObject timingMinigame;

    [Header("Position Limits (Local X)")]
    [SerializeField] private float leftLimit = -4f;
    [SerializeField] private float rightLimit = 4f;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;

    private float direction = 1f;
    private bool isRunning;
    public bool isInsideTarget;

    public bool HasFinished { get; private set; }
    public bool WasSuccessful { get; private set; }


    private void Start()
    {
        StartMinigame();
    }

    private void FixedUpdate()
    {
        if (!isRunning)
            return;

        float nextX = marker.position.x +
                      direction * moveSpeed * Time.fixedDeltaTime;

        float leftWorld = timingBar.TransformPoint(
            new Vector3(leftLimit, 0f, 0f)
        ).x;

        float rightWorld = timingBar.TransformPoint(
            new Vector3(rightLimit, 0f, 0f)
        ).x;

        if (nextX >= rightWorld)
        {
            nextX = rightWorld;
            direction = -1f;
        }
        else if (nextX <= leftWorld)
        {
            nextX = leftWorld;
            direction = 1f;
        }

        marker.MovePosition(new Vector2(nextX, marker.position.y));
    }

    private void Update()
    {
        if (!isRunning)
            return;

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            StopMarker();
        }
    }

    public void StartMinigame()

    {
        HasFinished = false;
        WasSuccessful = false;

        timingMinigame.SetActive(true);

        // Randomize target position
        float randomX = Random.Range(leftLimit, rightLimit);

        Vector3 targetPosition = target.localPosition;
        targetPosition.x = randomX;
        target.localPosition = targetPosition;

        // Reset marker to the left
        Vector3 markerPosition = timingBar.TransformPoint(
            new Vector3(leftLimit, 0f, 0f)
        );

        marker.position = new Vector2(
            markerPosition.x,
            markerPosition.y
        );

        direction = 1f;
        isInsideTarget = false;
        isRunning = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("ENTERED: " + other.name);

        if (other == targetCollider)
        {
            isInsideTarget = true;
            Debug.Log("INSIDE TARGET");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log("EXITED: " + other.name);

        if (other == targetCollider)
        {
            isInsideTarget = false;
            Debug.Log("OUTSIDE TARGET");
        }
    }

    private void StopMarker()
    {
        isRunning = false;

        isInsideTarget = marker.GetComponent<Collider2D>()
                               .IsTouching(targetCollider);

        WasSuccessful = isInsideTarget;
        HasFinished = true;

        if (WasSuccessful)
        {
            Debug.Log("SUCCESS!");
        }
        else
        {
            Debug.Log("FAILED!");
        }
    }

    public void RemoveMinigame()
    {
        timingMinigame.SetActive(false);

    }
}
