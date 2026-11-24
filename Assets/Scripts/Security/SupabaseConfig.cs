using UnityEngine;

[CreateAssetMenu(fileName = "SupabaseConfig", menuName = "Buhay Jeepney/Supabase Config")]
public class SupabaseConfig : ScriptableObject
{
    [Header("Supabase Project Settings")]
    public string projectUrl = "https://urvsrphqctzuwhutyyvv.supabase.co";
    [Tooltip("Client-side publishable / anon key ONLY. Never put secret or service_role keys here.")]
    public string anonKey = "sb_publishable_lI2g7yImB6k4ammCqKMt4w_EPk5leUj";

    public string AuthBaseUrl => $"{projectUrl.TrimEnd('/')}/auth/v1";
    public string RestBaseUrl => $"{projectUrl.TrimEnd('/')}/rest/v1";
}