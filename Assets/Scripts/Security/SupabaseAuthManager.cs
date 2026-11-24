using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class SupabaseAuthManager : MonoBehaviour
{
    public static SupabaseAuthManager Instance { get; private set; }

    public SupabaseConfig config;

    public string AccessToken { get; private set; }
    public string UserId { get; private set; }
    public string UserEmail { get; private set; }
    public bool IsGuest { get; private set; }
    public bool IsAuthenticated => !IsGuest && !string.IsNullOrEmpty(AccessToken) && !string.IsNullOrEmpty(UserId);

    [Serializable]
    private class AuthPayload
    {
        public string email;
        public string password;
    }

    [Serializable]
    private class AuthUser
    {
        public string id;
        public string email;
    }

    [Serializable]
    private class AuthSessionResponse
    {
        public string access_token;
        public string refresh_token;
        public AuthUser user;
    }

    [Serializable]
    private class SupabaseErrorResponse
    {
        public string msg;
        public string message;
        public string error_description;
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void ContinueAsGuest()
    {
        AccessToken = null;
        UserId = null;
        UserEmail = null;
        IsGuest = true;
        Debug.Log("[Auth] Guest mode active. Progress will not be saved.");
    }

    public IEnumerator SignUp(string email, string password, Action<bool, string> onComplete)
    {
        string url = $"{config.AuthBaseUrl}/signup";
        yield return SendAuthRequest(url, email, password, isSignUp: true, onComplete);
    }

    public IEnumerator SignIn(string email, string password, Action<bool, string> onComplete)
    {
        string url = $"{config.AuthBaseUrl}/token?grant_type=password";
        yield return SendAuthRequest(url, email, password, isSignUp: false, onComplete);
    }

    public IEnumerator SignOut(Action<bool, string> onComplete = null)
    {
        if (!string.IsNullOrEmpty(AccessToken))
        {
            string url = $"{config.AuthBaseUrl}/logout";
            using (UnityWebRequest req = new UnityWebRequest(url, "POST"))
            {
                req.SetRequestHeader("apikey", config.anonKey);
                req.SetRequestHeader("Authorization", $"Bearer {AccessToken}");
                req.downloadHandler = new DownloadHandlerBuffer();
                yield return req.SendWebRequest();
            }
        }

        AccessToken = null;
        UserId = null;
        UserEmail = null;
        IsGuest = false;
        onComplete?.Invoke(true, "Signed out.");
    }

    private IEnumerator SendAuthRequest(string url, string email, string password, bool isSignUp, Action<bool, string> onComplete)
    {
        if (config == null || string.IsNullOrEmpty(config.projectUrl) || string.IsNullOrEmpty(config.anonKey))
        {
            onComplete?.Invoke(false, "SupabaseConfig is missing or incomplete.");
            yield break;
        }

        string jsonBody = JsonUtility.ToJson(new AuthPayload { email = email, password = password });
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);

        using (UnityWebRequest req = new UnityWebRequest(url, "POST"))
        {
            req.uploadHandler = new UploadHandlerRaw(bodyRaw);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");
            req.SetRequestHeader("apikey", config.anonKey);

            yield return req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
            {
                string errMsg = ParseError(req.downloadHandler.text, req.error);
                onComplete?.Invoke(false, errMsg);
                yield break;
            }

            AuthSessionResponse res = JsonUtility.FromJson<AuthSessionResponse>(req.downloadHandler.text);

            if (isSignUp && string.IsNullOrEmpty(res.access_token))
            {
                IsGuest = false;
                onComplete?.Invoke(true, "Account created! Check your email to confirm, then sign in.");
                yield break;
            }

            if (string.IsNullOrEmpty(res.access_token) || res.user == null || string.IsNullOrEmpty(res.user.id))
            {
                onComplete?.Invoke(false, "Authentication failed: incomplete session data.");
                yield break;
            }

            AccessToken = res.access_token;
            UserId = res.user.id;
            UserEmail = res.user.email;
            IsGuest = false;

            onComplete?.Invoke(true, "Authenticated successfully.");
        }
    }

    private string ParseError(string rawJson, string fallback)
    {
        if (!string.IsNullOrEmpty(rawJson))
        {
            SupabaseErrorResponse err = JsonUtility.FromJson<SupabaseErrorResponse>(rawJson);
            if (!string.IsNullOrEmpty(err.error_description)) return err.error_description;
            if (!string.IsNullOrEmpty(err.msg)) return err.msg;
            if (!string.IsNullOrEmpty(err.message)) return err.message;
        }
        return fallback;
    }
}