using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DiscordController : MonoBehaviour
{
    private const long clientId = 1235329861980127343; // Your actual Client ID
    private Discord.Discord discord;
    private Discord.ActivityManager activityManager;
    private static long gameStartTime; // Store the start time of the entire game

    [System.Serializable]
    public class SceneNameMapping
    {
        public string sceneName;
        public string displayName;
    }

    public SceneNameMapping[] customSceneNames;

    void Start()
    {
        DontDestroyOnLoad(gameObject);

        try
        {
            discord = new Discord.Discord(clientId, (UInt64)Discord.CreateFlags.Default);
            activityManager = discord.GetActivityManager();

            if (gameStartTime == 0)
                gameStartTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds(); // Set only once on game start

            UpdatePresence("Loading...", "default_icon", string.Empty, "Unity Version", gameStartTime);

            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to initialize Discord SDK: " + e.Message);
        }
    }

    void Update()
    {
        discord?.RunCallbacks();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string roomName = GetCustomSceneName(scene.name);
        SetRoom(roomName);
    }

    private string GetCustomSceneName(string sceneName)
    {
        foreach (var mapping in customSceneNames)
        {
            if (mapping.sceneName == sceneName)
            {
                return mapping.displayName;
            }
        }

        return sceneName;
    }

    public void SetRoom(string roomName)
    {
        string roomImage = "default_icon";
        string detailsText = roomName;
        string stateText = $"Unity {Application.unityVersion}";

        UpdatePresence(stateText, roomImage, detailsText, string.Empty, gameStartTime);
    }

    private void UpdatePresence(string state, string imageKey, string details, string party, long startTimestamp)
    {
        var activity = new Discord.Activity
        {
            State = state,
            Details = details,
            Assets =
            {
                LargeImage = imageKey,
                LargeText = state
            },
            Timestamps =
            {
                Start = startTimestamp // Game start time remains constant
            }
        };

        activityManager.UpdateActivity(activity, result =>
        {
            if (result == Discord.Result.Ok)
                Debug.Log($"Discord RPC Updated: {state} - {details} (Game started at: {startTimestamp})");
            else
                Debug.LogError("Failed to update Discord RPC.");
        });
    }

    void OnApplicationQuit()
    {
        discord?.Dispose();
    }
}
