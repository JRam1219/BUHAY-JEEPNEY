
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuNavigation : MonoBehaviour
{
    public void OpenLogin()
    {
        SceneManager.LoadScene("Login");
    }

    public void OpenSignUp()
    {
        SceneManager.LoadScene("SignUp");
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void BackToLogin()
    {
        SceneManager.LoadScene("Login");
    }

    public void PlayAsGuest()
    {
    SceneManager.LoadScene("House_SQ1");
    }
}
