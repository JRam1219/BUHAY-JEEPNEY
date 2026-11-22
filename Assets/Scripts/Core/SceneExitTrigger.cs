using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneExitTrigger : MonoBehaviour
{
    public string nextSceneName = "Driving_SQ1";

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<JeepMovement>() != null)
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}