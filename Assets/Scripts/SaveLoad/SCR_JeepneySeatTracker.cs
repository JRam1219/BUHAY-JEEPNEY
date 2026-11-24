using UnityEngine;
using TMPro;

public class SCR_JeepneySeatTracker : MonoBehaviour
{
    [Header("Seat & Exit Setup")]
    public Transform[] seatPositions;
    public Transform exitPoint;

    [Header("Endless Road Settings")]
    public bool isJeepneyMoving = false;
    public float roadSpeed = 10f;
    public bool canAcceptPassengers = true;
    public SCR_EndlessRoadManager roadManager;
    public GameObject defaultPassengerPrefab;

    [Header("Stop Dialogue UI")]
    public GameObject dialoguePanel;
    public TMP_Text dialogueText;
    public KeyCode stopKey = KeyCode.Space;

    [Header("Tracked Data")]
    public int totalFareCollected = 0;

    private SCR_PassengerNPC[] occupiedSeats;
    private bool isWaitingForStop = false;

    private void Awake()
    {
        occupiedSeats = new SCR_PassengerNPC[seatPositions.Length];
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
    }

    private void Start()
    {
        // Load passengers carried over from the Terminal scene
        if (SCR_TripDataTransfer.passengersOnBoard.Count > 0)
        {
            totalFareCollected = SCR_TripDataTransfer.collectedFare;

            foreach (SavedPassengerData data in SCR_TripDataTransfer.passengersOnBoard)
            {
                if (defaultPassengerPrefab == null) break;

                GameObject npcObj = Instantiate(defaultPassengerPrefab);
                if (npcObj.TryGetComponent(out SCR_PassengerNPC npc))
                {
                    npc.Initialize(data.destination, data.fare, data.distanceRemaining, null);
                    TryAssignSeat(npc, data.destination, 0); // Fare already collected at Terminal
                }
            }

            SCR_TripDataTransfer.passengersOnBoard.Clear();
        }
    }

    private void Update()
    {
        // Count down travel distance for boarded passengers while driving
        if (isJeepneyMoving)
        {
            for (int i = 0; i < occupiedSeats.Length; i++)
            {
                SCR_PassengerNPC npc = occupiedSeats[i];
                if (npc != null && !npc.wantsToAlight)
                {
                    npc.distanceRemaining -= roadSpeed * Time.deltaTime;

                    if (npc.distanceRemaining <= 0f)
                    {
                        npc.wantsToAlight = true;
                        isWaitingForStop = true;
                        string stopMessage = string.IsNullOrEmpty(npc.destination)
                            ? $"\"Para po!\" (Press {stopKey} to stop)"
                            : $"\"Para po! Dito na lang sa {npc.destination}!\" (Press {stopKey} to stop)";

                        ShowStopDialogue(stopMessage);
                    }
                }
            }
        }

        // Drop off requesting passengers when player presses the stop key
        if (isWaitingForStop && Input.GetKeyDown(stopKey))
        {
            DropOffRequestingPassengers();
        }
    }

    public bool TryAssignSeat(SCR_PassengerNPC npc, string destination, int fare)
    {
        if (!canAcceptPassengers)
        {
            Debug.Log("It's night! Not accepting new passengers.");
            return false;
        }

        for (int i = 0; i < occupiedSeats.Length; i++)
        {
            if (occupiedSeats[i] == null)
            {
                occupiedSeats[i] = npc;
                npc.AssignSeat(seatPositions[i]);
                totalFareCollected += fare;

                Debug.Log($"Boarded! Destination: {destination} | Fare: ₱{fare} | Seat #{i + 1}");
                return true;
            }
        }

        Debug.Log("Jeepney is full!");
        return false;
    }

    public bool IsFull()
    {
        for (int i = 0; i < occupiedSeats.Length; i++)
        {
            if (occupiedSeats[i] == null) return false;
        }
        return true;
    }

    public bool IsEmpty()
    {
        for (int i = 0; i < occupiedSeats.Length; i++)
        {
            if (occupiedSeats[i] != null) return false;
        }
        return true;
    }

    // Call this right before loading the Endless Road scene from the Terminal
    public void SavePassengersForTransfer()
    {
        SCR_TripDataTransfer.passengersOnBoard.Clear();
        SCR_TripDataTransfer.collectedFare = totalFareCollected;

        for (int i = 0; i < occupiedSeats.Length; i++)
        {
            if (occupiedSeats[i] != null)
            {
                SCR_TripDataTransfer.passengersOnBoard.Add(new SavedPassengerData
                {
                    destination = occupiedSeats[i].destination,
                    fare = occupiedSeats[i].fare,
                    distanceRemaining = occupiedSeats[i].distanceRemaining
                });
            }
        }
    }

    private void ShowStopDialogue(string message)
    {
        Debug.Log(message);
        if (dialoguePanel != null && dialogueText != null)
        {
            dialoguePanel.SetActive(true);
            dialogueText.text = message;
        }
    }

    public void DropOffRequestingPassengers()
    {
        for (int i = 0; i < occupiedSeats.Length; i++)
        {
            if (occupiedSeats[i] != null && occupiedSeats[i].wantsToAlight)
            {
                occupiedSeats[i].LeaveJeepney(exitPoint);
                occupiedSeats[i] = null;
            }
        }

        isWaitingForStop = false;
        if (dialoguePanel != null) dialoguePanel.SetActive(false);

        if (roadManager != null)
        {
            roadManager.CheckForGarageReturn();
        }
    }
}