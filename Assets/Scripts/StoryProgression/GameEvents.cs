using System;

public static class GameEvents
{
    public static bool HasBook;
    public static event Action PlayerSpotted;
    public static event Action BookStolen;
    public static event Action PlayerRespawned;

    public static void Spotted() => PlayerSpotted?.Invoke();
    public static void Stolen() { HasBook = true; BookStolen?.Invoke(); }
    public static void Respawned() => PlayerRespawned?.Invoke();
    public static void ResetAll() => HasBook = false;
}