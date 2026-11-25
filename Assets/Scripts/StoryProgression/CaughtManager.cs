
using System.Collections;
using UnityEngine;
using TMPro;

public class CaughtManager : MonoBehaviour
{
    [Header("Jeff")]
    public Transform jeff;
    public Transform respawnPoint;
    public Transform secondRoomRespawnPoint;
    public MonoBehaviour jeffMovement;

    [Header("Book")]
    public GameObject blueBook;

    [Header("UI")]
    public TMP_Text messageText;
    public float freezeTime = 1.2f;

    private bool busy;
    private Coroutine msg;

    private void OnEnable()
    {
        GameEvents.PlayerSpotted += OnSpotted;
        GameEvents.BookStolen += OnBookStolen;
    }

    private void OnDisable()
    {
        GameEvents.PlayerSpotted -= OnSpotted;
        GameEvents.BookStolen -= OnBookStolen;
    }

    private void Start()
    {
        GameEvents.ResetAll();

        Say("Find the blue notebook in the mayor's office.", 3f);
    }

    private void OnBookStolen()
    {
        Say("Got the blue notebook! Now get out!", 3f);
    }

    private void OnSpotted()
    {
        if (busy) return;

        StartCoroutine(CaughtRoutine());
    }

    private IEnumerator CaughtRoutine()
    {
        busy = true;

        if (jeffMovement)
            jeffMovement.enabled = false;

        Say("CAUGHT!", freezeTime);

        yield return new WaitForSeconds(freezeTime);

        bool hasBook = GameEvents.HasBook;

        if (hasBook)
        {
            // Jeff has the book: respawn in SecondRoom.
            if (secondRoomRespawnPoint != null)
            {
                jeff.position = secondRoomRespawnPoint.position;
            }
            else
            {
                Debug.LogError(
                    "Second Room Respawn Point is not assigned!"
                );
            }

            // Keep the book permanently collected.
            GameEvents.Respawned();
        }
        else
        {
            // Jeff doesn't have the book: return to the door.
            if (respawnPoint != null)
            {
                jeff.position = respawnPoint.position;
            }
            else
            {
                Debug.LogError("Respawn Point is not assigned!");
            }

            GameEvents.ResetAll();

            // Make the book available again.
            if (blueBook)
                blueBook.SetActive(true);

            // Reset police patrol.
            GameEvents.Respawned();
        }

        if (jeffMovement)
            jeffMovement.enabled = true;

        busy = false;
    }

    public void Win()
    {
        if (busy) return;

        busy = true;

        if (jeffMovement)
            jeffMovement.enabled = false;

        Say("Mission complete! Jeff got away with the notebook.", 999f);
    }

    public void Say(string text, float seconds)
    {
        if (!messageText) return;

        if (msg != null)
            StopCoroutine(msg);

        msg = StartCoroutine(MsgRoutine(text, seconds));
    }

    private IEnumerator MsgRoutine(string text, float seconds)
    {
        messageText.text = text;

        yield return new WaitForSeconds(seconds);

        messageText.text = "";
    }
}
