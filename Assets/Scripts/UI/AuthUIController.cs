using System.Collections;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AuthUIController : MonoBehaviour
{
    [Header("Input Fields")]
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    [Tooltip("Optional: leave empty on the Login scene; assign on SignUp if you have a Confirm Password field.")]
    public TMP_InputField confirmPasswordInput;

    [Header("UI Feedback & Buttons")]
    public TMP_Text statusText;
    public Button submitButton;
    public Button backButton;

    [Header("Destination Scene")]
    [Tooltip("Scene to load after successful sign-in if no saved scene is found.")]
    public string defaultStartScene = "House_SQ1";
    [Tooltip("If true on SignUp, redirects back to Login after creating the account instead of entering gameplay immediately.")]
    public bool redirectToLoginAfterSignUp = false;

    private bool isRequestRunning = false;

    void Start()
    {
        if (statusText != null) statusText.text = "";
        if (passwordInput != null)
            passwordInput.contentType = TMP_InputField.ContentType.Password;
        if (confirmPasswordInput != null)
            confirmPasswordInput.contentType = TMP_InputField.ContentType.Password;
    }

    public void OnLoginButtonClicked()
    {
        if (isRequestRunning) return;

        string email = emailInput != null ? emailInput.text.Trim() : "";
        string password = passwordInput != null ? passwordInput.text : "";

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            SetStatus("Please enter both email and password.");
            return;
        }

        StartCoroutine(LoginRoutine(email, password));
    }

    public void OnSignUpButtonClicked()
    {
        if (isRequestRunning) return;

        string email = emailInput != null ? emailInput.text.Trim() : "";
        string password = passwordInput != null ? passwordInput.text : "";

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            SetStatus("Please fill in all required fields.");
            return;
        }

        if (password.Length < 6)
        {
            SetStatus("Password must be at least 6 characters.");
            return;
        }

        if (confirmPasswordInput != null && password != confirmPasswordInput.text)
        {
            SetStatus("Passwords do not match.");
            return;
        }

        StartCoroutine(SignUpRoutine(email, password));
    }

    private IEnumerator LoginRoutine(string email, string password)
    {
        if (SupabaseAuthManager.Instance == null)
        {
            SetStatus("Auth manager not found in scene.");
            yield break;
        }

        SetBusy(true, "Signing in...");

        bool success = false;
        string message = "";

        yield return StartCoroutine(SupabaseAuthManager.Instance.SignIn(email, password, (ok, msg) =>
        {
            success = ok;
            message = msg;
        }));

        if (!success)
        {
            SetBusy(false, message);
            yield break;
        }

        string targetScene = defaultStartScene;

        if (MenuNavigation.startNewGameAfterLogin)
        {
            MenuNavigation.startNewGameAfterLogin = false;
            SetStatus("Starting new game...");

            if (SupabaseSaveService.Instance != null)
            {
                bool archived = false;
                yield return StartCoroutine(SupabaseSaveService.Instance.ArchiveAndStartNewGame((ok, msg) =>
                {
                    Debug.Log($"[NewGame After Login] {msg} (Success: {ok})");
                    archived = ok;
                }));

                if (!archived)
                {
                    SetBusy(false, "Failed to reset save for new game.");
                    yield break;
                }

                yield return StartCoroutine(SupabaseSaveService.Instance.LoadActiveSave((ok, save, msg) =>
                {
                    Debug.Log($"[NewGame Reload] {msg} (Success: {ok})");
                    if (ok && save != null && !string.IsNullOrEmpty(save.current_scene))
                    {
                        targetScene = save.current_scene;
                    }
                }));
            }
        }
        else
        {
            SetStatus("Loading saved progress...");

            if (SupabaseSaveService.Instance != null)
            {
                yield return StartCoroutine(SupabaseSaveService.Instance.LoadActiveSave((ok, save, _) =>
                {
                    if (ok && save != null && !string.IsNullOrEmpty(save.current_scene))
                    {
                        targetScene = save.current_scene;
                    }
                }));
            }
        }

        SetBusy(false, "Welcome back!");
        SceneManager.LoadScene(targetScene);
    }

    private IEnumerator SignUpRoutine(string email, string password)
    {
        if (SupabaseAuthManager.Instance == null)
        {
            SetStatus("Auth manager not found in scene.");
            yield break;
        }

        SetBusy(true, "Creating account...");

        bool success = false;
        string message = "";

        yield return StartCoroutine(SupabaseAuthManager.Instance.SignUp(email, password, (ok, msg) =>
        {
            success = ok;
            message = msg;
        }));

        SetBusy(false, message);

        if (!success) yield break;

        // If email confirmation is required (no active token yet) or redirect is enabled, go to Login
        if (!SupabaseAuthManager.Instance.IsAuthenticated || redirectToLoginAfterSignUp)
        {
            yield return new WaitForSeconds(1.2f);
            SceneManager.LoadScene("Login");
            yield break;
        }

        // Otherwise load the newly triggered starter save and start the game
        if (SupabaseSaveService.Instance != null)
        {
            yield return StartCoroutine(SupabaseSaveService.Instance.LoadActiveSave((_, __, ___) => { }));
        }

        SceneManager.LoadScene(defaultStartScene);
    }

    private void SetBusy(bool busy, string status)
    {
        isRequestRunning = busy;
        if (submitButton != null) submitButton.interactable = !busy;
        if (backButton != null) backButton.interactable = !busy;
        SetStatus(status);
    }

    private void SetStatus(string msg)
    {
        if (statusText != null) statusText.text = msg;
        Debug.Log($"[AuthUI] {msg}");
    }
    private string CleanInput(string raw)
    {
        if (string.IsNullOrEmpty(raw)) return string.Empty;
        return raw.Replace("\u200B", "").Trim();
    }

    public bool ValidateInputs(out string email, out string password, bool requireConfirmPassword = false)
    {
        email = CleanInput(emailInput != null ? emailInput.text : "");
        password = CleanInput(passwordInput != null ? passwordInput.text : "");
        string confirm = CleanInput(confirmPasswordInput != null ? confirmPasswordInput.text : "");

        if (string.IsNullOrEmpty(email))
        {
            SetStatusMessage("Please enter your email address.");
            return false;
        }

        if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        {
            SetStatusMessage("Invalid email format. Use a format like name@gmail.com.");
            return false;
        }

        if (string.IsNullOrEmpty(password))
        {
            SetStatusMessage("Please enter your password.");
            return false;
        }

        if (password.Length < 6)
        {
            SetStatusMessage("Password must be at least 6 characters long.");
            return false;
        }

        if (requireConfirmPassword && confirmPasswordInput != null && password != confirm)
        {
            SetStatusMessage("Passwords do not match.");
            return false;
        }

        return true;
    }

    public string GetFriendlyAuthError(string rawError)
    {
        if (string.IsNullOrEmpty(rawError))
            return "Authentication failed. Please check your details and try again.";

        string lower = rawError.ToLower();

        if (lower.Contains("email address") && lower.Contains("invalid"))
            return "Supabase rejected that email address. Try a real, valid email address without spaces.";

        if (lower.Contains("user already registered"))
            return "This email is already registered. Try signing in instead.";

        if (lower.Contains("invalid login credentials"))
            return "Incorrect email or password.";

        if (lower.Contains("email not confirmed"))
            return "Please verify your email address in your inbox before signing in.";

        if (lower.Contains("password") && lower.Contains("6"))
            return "Password must be at least 6 characters long.";

        return rawError;
    }

    private void SetStatusMessage(string message)
    {
        if (statusText != null)
            statusText.text = message;

        Debug.Log($"[AuthUI] {message}");
    }
}