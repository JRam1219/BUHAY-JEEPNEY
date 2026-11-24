using System.Collections.Generic;

public struct SavedPassengerData
{
    public string destination;
    public int fare;
    public float distanceRemaining;
}

public static class SCR_TripDataTransfer
{
    public static List<SavedPassengerData> passengersOnBoard = new List<SavedPassengerData>();
    public static int collectedFare = 0;
}