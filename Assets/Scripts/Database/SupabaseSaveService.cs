using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

[Serializable]
public class PlayerSaveData
{
    public string user_id;
    public int current_sequence = 1;
    public string current_scene = "House_SQ1";
    public float cash_on_hand = 0f;
    public float savings_balance = 0f;
    public float daily_quota = 2500f;
    public float gross_fares = 0f;
    public float diesel_cost = 0f;
    public float boundary_paid = 0f;
    public bool ejected_student = false;
    public bool gave_project_budget = false;
    public bool gave_checkup_money = false;
    public bool is_archived = false;
}

public class SupabaseSaveService : MonoBehaviour
{
    public static SupabaseSaveService Instance { get; private set; }

    public SupabaseConfig config;
    public PlayerSaveData currentSave = new PlayerSaveData();

    [Serializable]
    private class SaveArrayWrapper
    {
        public PlayerSaveData[] items;
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

    void OnEnable()
    {
        SceneManager.sceneLoaded += HandleSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Skip non-gameplay menu/auth scenes
        if (scene.name == "MainMenu" || scene.name == "Login" || scene.name == "SignUp")
            return;

        var auth = SupabaseAuthManager.Instance;
        if (auth == null || !auth.IsAuthenticated)
            return;

        int seq = currentSave != null ? currentSave.current_sequence : 1;
        StartCoroutine(SaveSceneProgress(seq, scene.name, (ok, msg) =>
        {
            Debug.Log($"[AutoSceneSave] {scene.name}: {msg} (Success: {ok})");
        }));
    }

    public IEnumerator SaveSceneProgress(int sequenceNumber, string sceneName, Action<bool, string> onComplete = null)
    {
        if (currentSave == null)
        {
            currentSave = new PlayerSaveData();
        }

        currentSave.current_sequence = sequenceNumber;
        currentSave.current_scene = string.IsNullOrEmpty(sceneName)
            ? SceneManager.GetActiveScene().name
            : sceneName;
        currentSave.is_archived = false;

        yield return StartCoroutine(SaveActiveProgress(currentSave, onComplete));
    }

    public IEnumerator LoadActiveSave(Action<bool, PlayerSaveData, string> onComplete)
    {

        var auth = SupabaseAuthManager.Instance;
        if (auth == null || !auth.IsAuthenticated)
        {
            onComplete?.Invoke(false, null, "Not signed in (Guest or unauthenticated).");
            yield break;
        }

        string url = $"{config.RestBaseUrl}/player_saves?user_id=eq.{auth.UserId}&is_archived=eq.false&order=id.desc&select=*";
        using (UnityWebRequest req = UnityWebRequest.Get(url))
        {
            ApplySupabaseHeaders(req, auth.AccessToken);
            yield return req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
            {
                onComplete?.Invoke(false, null, $"Load failed: {req.error}");
                yield break;
            }

            string wrappedJson = "{\"items\":" + req.downloadHandler.text + "}";
            SaveArrayWrapper wrapper = JsonUtility.FromJson<SaveArrayWrapper>(wrappedJson);

            if (wrapper != null && wrapper.items != null && wrapper.items.Length > 0)
            {
                currentSave = wrapper.items[0];
                onComplete?.Invoke(true, currentSave, "Active save loaded.");
            }
            else
            {
                currentSave = new PlayerSaveData { user_id = auth.UserId, is_archived = false };
                onComplete?.Invoke(true, currentSave, "No active save found; initialized default.");
            }
        }
    }

    public IEnumerator SaveActiveProgress(PlayerSaveData dataToSave, Action<bool, string> onComplete)
    {
        var auth = SupabaseAuthManager.Instance;
        if (auth == null || !auth.IsAuthenticated)
        {
            onComplete?.Invoke(false, "Guest mode: progress not saved to Supabase.");
            yield break;
        }

        dataToSave.user_id = auth.UserId;
        dataToSave.is_archived = false;

        string url = $"{config.RestBaseUrl}/player_saves?user_id=eq.{auth.UserId}&is_archived=eq.false";
        byte[] bodyRaw = Encoding.UTF8.GetBytes(JsonUtility.ToJson(dataToSave));

        using (UnityWebRequest req = new UnityWebRequest(url, "PATCH"))
        {
            req.uploadHandler = new UploadHandlerRaw(bodyRaw);
            req.downloadHandler = new DownloadHandlerBuffer();
            ApplySupabaseHeaders(req, auth.AccessToken);
            req.SetRequestHeader("Content-Type", "application/json");
            req.SetRequestHeader("Prefer", "return=minimal");

            yield return req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
            {
                onComplete?.Invoke(false, $"Save failed: {req.error}");
                yield break;
            }

            currentSave = dataToSave;
            onComplete?.Invoke(true, "Progress saved to Supabase.");
        }
    }

    public IEnumerator ArchiveAndStartNewGame(Action<bool, string> onComplete)
    {
        var auth = SupabaseAuthManager.Instance;
        if (auth == null || !auth.IsAuthenticated)
        {
            onComplete?.Invoke(false, "Must be signed in to archive cloud save.");
            yield break;
        }

        string url = $"{config.RestBaseUrl}/rpc/archive_current_playthrough";
        byte[] emptyJson = Encoding.UTF8.GetBytes("{}");

        using (UnityWebRequest req = new UnityWebRequest(url, "POST"))
        {
            req.uploadHandler = new UploadHandlerRaw(emptyJson);
            req.downloadHandler = new DownloadHandlerBuffer();
            ApplySupabaseHeaders(req, auth.AccessToken);
            req.SetRequestHeader("Content-Type", "application/json");

            yield return req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
            {
                onComplete?.Invoke(false, $"Archive failed: {req.error}");
                yield break;
            }

            currentSave = new PlayerSaveData
            {
                user_id = auth.UserId,
                current_sequence = 1,
                current_scene = "House_SQ1",
                is_archived = false
            };
            onComplete?.Invoke(true, "Previous playthrough archived. Starting new save.");
        }
    }

    private void ApplySupabaseHeaders(UnityWebRequest req, string jwt)
    {
        req.SetRequestHeader("apikey", config.anonKey);
        req.SetRequestHeader("Authorization", $"Bearer {jwt}");
    }

    private void OnApplicationQuit()
    {
        SaveCurrentActiveSceneImmediately();
    }

    public void SaveCurrentActiveSceneImmediately()
    {
        string activeScene = SceneManager.GetActiveScene().name;
        if (activeScene == "MainMenu" || activeScene == "Login" || activeScene == "SignUp")
            return;

        var auth = SupabaseAuthManager.Instance;
        if (auth == null || !auth.IsAuthenticated || config == null)
            return;

        if (currentSave == null)
        {
            currentSave = new PlayerSaveData();
        }

        currentSave.user_id = auth.UserId;
        currentSave.current_scene = activeScene;
        currentSave.is_archived = false;

        string url = $"{config.RestBaseUrl}/player_saves?user_id=eq.{auth.UserId}&is_archived=eq.false";
        byte[] bodyRaw = Encoding.UTF8.GetBytes(JsonUtility.ToJson(currentSave));

        UnityWebRequest req = new UnityWebRequest(url, "PATCH");
        req.uploadHandler = new UploadHandlerRaw(bodyRaw);
        req.downloadHandler = new DownloadHandlerBuffer();
        ApplySupabaseHeaders(req, auth.AccessToken);
        req.SetRequestHeader("Content-Type", "application/json");
        req.SetRequestHeader("Prefer", "return=minimal");
        req.SendWebRequest();
    }
}