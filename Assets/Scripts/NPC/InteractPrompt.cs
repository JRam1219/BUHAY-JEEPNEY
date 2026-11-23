using System.Reflection;
using UnityEngine;

// Shows a floating "E" above an interactable when the player is in range, hides it during dialogue.
// Add to the same object as NPCInteractable / JeepInteractable / GasAttendantInteractable /
// PassengerInteractable / GarageDeskEndInteractable / ShelfInteractable.
// It copies that script's "interactRange" automatically, so the E appears exactly when pressing E works.
public class InteractPrompt : MonoBehaviour
{
    public GameObject prompt;              // child object showing the "E"
    public Transform target;               // leave empty: finds "Jeff", otherwise the jeep
    public bool useInteractableRange = true;
    public float showRange = 2f;           // used if no interactRange is found
    public float bobHeight = 0.05f;
    public float bobSpeed = 4f;

    Vector3 baseLocalPos;

    void Start()
    {
        if (useInteractableRange)
        {
            foreach (var mb in GetComponents<MonoBehaviour>())
            {
                if (mb == null || mb == this) continue;
                FieldInfo f = mb.GetType().GetField("interactRange");
                if (f != null && f.FieldType == typeof(float)) { showRange = (float)f.GetValue(mb); break; }
            }
        }

        if (target == null)
        {
            GameObject jeff = GameObject.Find("Jeff");
            if (jeff != null) target = jeff.transform;
            else
            {
                JeepMovement jeep = FindFirstObjectByType<JeepMovement>();
                if (jeep != null) target = jeep.transform;
            }
        }

        if (prompt != null)
        {
            baseLocalPos = prompt.transform.localPosition;
            prompt.SetActive(false);
        }
    }

    void Update()
    {
        if (prompt == null || target == null) return;

        bool talking = DialogueManager.Instance != null && DialogueManager.Instance.isDialogueActive;
        bool inRange = Vector2.Distance(transform.position, target.position) <= showRange;
        bool show = inRange && !talking;

        if (prompt.activeSelf != show) prompt.SetActive(show);
        if (show)
            prompt.transform.localPosition = baseLocalPos + Vector3.up * (Mathf.Sin(Time.time * bobSpeed) * bobHeight);
    }
}
