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

    // private async void Awake()
    // {
    //     await UnityServices.InitializeAsync();

    //         if (!AuthenticationService.Instance.IsSignedIn)
    //         {
    //             await AuthenticationService.Instance.SignInAnonymouslyAsync();
                
    //     }
    // }
    private async void Awake()
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
            UIManager.Instance.HideLoadingScreen();

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


    private async Task<string> StartHostWithRelay(int maxConnections = 3)

    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);
            var relayServerData = new RelayServerData(allocation, "udp");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            bool started = NetworkManager.Singleton.StartHost();
            return started ? joinCode : null;
        }
        catch
        {
            Debug.LogError("Creating allocation failed");
            UIManager.Instance.HideLoadingScreen();
            throw;
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
