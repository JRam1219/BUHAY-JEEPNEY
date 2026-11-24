using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuNavigation : MonoBehaviour
{
    public static bool startNewGameAfterLogin = false;

    [SerializeField]
    private string defaultStartScene = "House_SQ1";

    public void OpenLogin()
    {
        startNewGameAfterLogin = false;
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
        startNewGameAfterLogin = false;
        if (SupabaseAuthManager.Instance != null)
        {
            SupabaseAuthManager.Instance.ContinueAsGuest();
        }

        SceneManager.LoadScene(defaultStartScene);
    }

    public void NewGame()
    {
        Debug.Log("[NewGame] Button clicked!");
        StartCoroutine(NewGameRoutine());
    }

    public void ContinueGame()
    {
        StartCoroutine(ContinueGameRoutine());
    }

    private IEnumerator NewGameRoutine()
    {
        var auth = SupabaseAuthManager.Instance;
        var saveService = SupabaseSaveService.Instance;

        if (auth != null && auth.IsAuthenticated && saveService != null)
        {
            startNewGameAfterLogin = false;
            bool archived = false;
            yield return StartCoroutine(saveService.ArchiveAndStartNewGame((ok, msg) =>
            {
                Debug.Log($"[NewGame] {msg} (Success: {ok})");
                archived = ok;
            }));

            if (!archived)
            {
                yield break;
            }

            bool loadedFresh = false;
            yield return StartCoroutine(saveService.LoadActiveSave((ok, save, msg) =>
            {
                Debug.Log($"[NewGame Reload] {msg} (Success: {ok})");
                loadedFresh = true;
            }));

            while (!loadedFresh)
            {
                yield return null;
            }
        }
        else if (auth != null && !auth.IsAuthenticated && !auth.IsGuest)
        {
            startNewGameAfterLogin = true;
            SceneManager.LoadScene("Login");
            yield break;
        }

        SceneManager.LoadScene(defaultStartScene);
    }

    private IEnumerator ContinueGameRoutine()
    {
        startNewGameAfterLogin = false;
        var auth = SupabaseAuthManager.Instance;
        var saveService = SupabaseSaveService.Instance;

        if (auth == null || !auth.IsAuthenticated)
        {
            if (auth != null && auth.IsGuest)
            {
                SceneManager.LoadScene(defaultStartScene);
                yield break;
            }

            SceneManager.LoadScene("Login");
            yield break;
        }

        string targetScene = defaultStartScene;

        if (saveService != null)
        {
            bool loaded = false;
            yield return StartCoroutine(saveService.LoadActiveSave((ok, save, msg) =>
            {
                Debug.Log($"[Continue] {msg} (Success: {ok})");
                if (ok && save != null && !string.IsNullOrEmpty(save.current_scene))
                {
                    targetScene = save.current_scene;
                }
                loaded = true;
            }));

            while (!loaded)
            {
                yield return null;
            }
        }

        SceneManager.LoadScene(targetScene);
    }

    public void SignOut()
    {
        startNewGameAfterLogin = false;
        StartCoroutine(SignOutRoutine());
    }

    private IEnumerator SignOutRoutine()
    {
        if (SupabaseAuthManager.Instance != null)
        {
            bool finished = false;
            yield return StartCoroutine(SupabaseAuthManager.Instance.SignOut((_, __) => finished = true));
            while (!finished)
            {
                yield return null;
            }
        }

        SceneManager.LoadScene("MainMenu");
    }
}