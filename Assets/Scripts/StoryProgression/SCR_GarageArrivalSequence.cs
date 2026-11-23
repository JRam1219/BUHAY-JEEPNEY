using System.Collections;
using UnityEngine;

public class SCR_GarageArrivalSequence : MonoBehaviour
{
    [Header("Jeepney Arrival")]
    public Transform jeepney;
    public Transform parkingSpot;
    public float driveSpeed = 3.5f;
    public Animator jeepneyAnimator;
    public string movingBoolName = "isMoving";

    [Header("Jeff Exit Setup")]
    public GameObject jeffPlayer;
    public Transform jeffExitPoint;
    public float exitDelay = 0.4f;

    private void Start()
    {
        // Keep Jeff hidden while the jeepney pulls into the garage
        if (jeffPlayer != null)
        {
            jeffPlayer.SetActive(false);
        }

        if (jeepney != null && parkingSpot != null)
        {
            StartCoroutine(ArrivalRoutine());
        }
    }

    private IEnumerator ArrivalRoutine()
    {
        if (jeepneyAnimator != null && !string.IsNullOrEmpty(movingBoolName))
        {
            jeepneyAnimator.SetBool(movingBoolName, true);
        }

        // Drive smoothly up into the left parking bay
        while (Vector2.Distance(jeepney.position, parkingSpot.position) > 0.02f)
        {
            jeepney.position = Vector2.MoveTowards(
                jeepney.position,
                parkingSpot.position,
                driveSpeed * Time.deltaTime
            );
            yield return null;
        }

        jeepney.position = parkingSpot.position;

        if (jeepneyAnimator != null && !string.IsNullOrEmpty(movingBoolName))
        {
            jeepneyAnimator.SetBool(movingBoolName, false);
        }

        // Brief pause after parking before Jeff steps out on the left
        yield return new WaitForSeconds(exitDelay);

        if (jeffPlayer != null)
        {
            if (jeffExitPoint != null)
            {
                jeffPlayer.transform.position = jeffExitPoint.position;
            }
            jeffPlayer.SetActive(true);
        }
    }
}
