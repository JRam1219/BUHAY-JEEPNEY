using UnityEngine;

public class WakeUpInteractable : MonoBehaviour
{
    public Transform jeff;
    public float interactRange = 2f;

    private bool hasWokenUp = false;

    void Start()
    {
        if (jeff == null)
        {
            GameObject j = GameObject.Find("Jeff");
            if (j != null)
                jeff = j.transform;
        }

        if (jeff != null)
        {
            PlayerController pc = jeff.GetComponent<PlayerController>();
            if (pc != null)
                pc.enabled = false;

            Animator anim = jeff.GetComponent<Animator>();
            if (anim != null)
                anim.speed = 0f;
        }
    }

    void Update()
    {
        if (hasWokenUp || jeff == null)
            return;

        if (DialogueManager.Instance == null ||
            DialogueManager.Instance.isDialogueActive)
            return;

        float distance = Vector2.Distance(transform.position, jeff.position);

        if (distance <= interactRange && Input.GetKeyDown(KeyCode.E))
        {
            hasWokenUp = true;

            PlayerController pc = jeff.GetComponent<PlayerController>();
            if (pc != null)
                pc.enabled = true;

            Debug.Log("Jeff has woken up.");
        }
    }
}