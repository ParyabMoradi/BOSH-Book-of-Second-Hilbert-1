using UnityEngine;
using TMPro;
using System.Threading.Tasks;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay.Models;
using Unity.Services.Relay;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.Networking;

public class RelayManager : MonoBehaviour
{
    public static string LastJoinCode;
    [SerializeField] private TextMeshProUGUI joinCodeText;
    [SerializeField] private TMP_InputField joinCodeInputField;

    async Task Start()
    {
        await UnityServices.InitializeAsync();

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log("Signed in anonymously.");
        }
    }


    public async void StartRelay(string level)
{
    UIManager.Instance.ShowLoadingScreen();  // Enable it when starting

    string joinCode = await StartHostWithRelay();

    if (!string.IsNullOrEmpty(joinCode))
    {
        joinCodeText.text = joinCode;
        LastJoinCode = joinCode;

        UIManager.Instance.HideLoadingScreen(); // Hide it after success

        NetworkManager.Singleton.SceneManager.LoadScene(level, LoadSceneMode.Single);
    }
    else
    {
        Debug.LogError("Relay creation failed.");
        UIManager.Instance.HideLoadingScreen(); //  Hide it if failed
    }
}


    public async void JoinRelay()
    {
        UIManager.Instance.ShowLoadingScreen();

        bool success = await StartClientWithRelay(joinCodeInputField.text);

        UIManager.Instance.HideLoadingScreen();

        if (!success)
        {
            Debug.LogError("Failed to join relay with code: " + joinCodeInputField.text);
            // Optional: show an error popup or UI feedback
        }
    }

    private async Task<float> GetRegionLatency(string url)
    {
        var request = UnityWebRequest.Get(url);
        var startTime = Time.realtimeSinceStartup;
        await request.SendWebRequest();
        float latency = (Time.realtimeSinceStartup - startTime) * 1000f; // milliseconds
        return latency;
    }


    private async Task<string> StartHostWithRelay(int maxConnections = 2)
{
    try
    {
        // 1. Get all active Relay regions
        List<Region> regions = await RelayService.Instance.ListRegionsAsync();
        Dictionary<string, float> regionLatencies = new Dictionary<string, float>();

        // 2. Estimate latency to each (custom or proxy endpoints; fake here for logic)
        foreach (var region in regions)
        {
            string testUrl = $"https://{region.Id}-a1.ud-relay.unity3d.com";
            try
            {
                float latency = await GetRegionLatency(testUrl);
                regionLatencies[region.Id] = latency;
                Debug.Log($"Ping to {region.Id}: {latency:F1} ms");
            }
            catch
            {
                Debug.LogWarning($"Latency check failed for region: {region.Id}");
                regionLatencies[region.Id] = float.MaxValue;
            }
        }

        // 3. Sort regions by latency
        var sortedRegions = new List<string>(regionLatencies.Keys);
        sortedRegions.Sort((a, b) => regionLatencies[a].CompareTo(regionLatencies[b]));

        // 4. Try allocation in best regions first
        foreach (string regionId in sortedRegions)
        {
            try
            {
                Debug.Log($"Trying allocation in region: {regionId}");
                Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections, regionId);
                var relayServerData = new RelayServerData(allocation, "udp");
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

                string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
                bool started = NetworkManager.Singleton.StartHost();
                return started ? joinCode : null;
            }
            catch (RelayServiceException ex)
            {
                Debug.LogWarning($"Allocation failed in {regionId}: {ex.Message}");
                continue;
            }
        }

        Debug.LogError("All relay region allocation attempts failed.");
        return null;
    }
    catch (System.Exception e)
    {
        Debug.LogError("Relay setup failed: " + e.Message);
        return null;
    }
}



    private async Task<bool> StartClientWithRelay(string joinCode)
    {
        try
        {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            var relayServerData = new RelayServerData(joinAllocation, "udp");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            return NetworkManager.Singleton.StartClient();
        }
        catch
        {
            return false;
        }
    }
}
