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

public class RelayManager : MonoBehaviour
{
    public static string LastJoinCode;
    [SerializeField] private TextMeshProUGUI joinCodeText;
    [SerializeField] private TMP_InputField joinCodeInputField;

    async Task Start()
    {
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public async void StartRelay()
    {
        string joinCode = await StartHostWithRelay();

        joinCodeText.text = joinCode;

        LastJoinCode = joinCode;

        // Proper way to load scene in a networked game
        NetworkManager.Singleton.SceneManager.LoadScene("Level 1", LoadSceneMode.Single);
        
    }

    public async void JoinRelay()
    {
        await StartClientWithRelay(joinCodeInputField.text);
    }

    private async Task<string> StartHostWithRelay(int maxConnections = 3)
    {
        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);

        var relayServerData = new RelayServerData(allocation, "udp");
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

        string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
        bool started = NetworkManager.Singleton.StartHost();
        return started ? joinCode : null;
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
