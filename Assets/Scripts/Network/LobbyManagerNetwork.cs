using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using TMPro;
using System;
using System.Threading.Tasks;

public class LobbyManagerNetwork : MonoBehaviour
{
    [Header("Player UI")]
    public GameObject playerUIItemPrefab; // assign PlayerUIItem prefab here
    public Transform healthBarContainer; // assign HealthBars here

    [Header("UI")]
    public TMP_Text lobbyCodeText;
    public TMP_Text playerListText;

    private Lobby currentLobby;
    private float heartbeatTimer;

    public static bool IsHost = false;

    async void Start()
    {
        await UnityServices.InitializeAsync();

        if (!AuthenticationService.Instance.IsSignedIn)
            await AuthenticationService.Instance.SignInAnonymouslyAsync();

        Debug.Log("Signed in as " + AuthenticationService.Instance.PlayerId);

        if (PlayerPrefs.GetInt("isHost") == 1)
        {
            HostLobby();
            PlayerPrefs.SetInt("isHost", 0); // Reset
        }
        else if (PlayerPrefs.HasKey("joinCode"))
        {
            string code = PlayerPrefs.GetString("joinCode");
            JoinLobbyByCode(code);
            PlayerPrefs.DeleteKey("joinCode");
        }

        InvokeRepeating(nameof(UpdateLobbyState), 1f, 2f);

    }

    async void UpdateLobbyState()
    {
        if (currentLobby == null) return;

        try
        {
            currentLobby = await LobbyService.Instance.GetLobbyAsync(currentLobby.Id);
            RefreshLobbyUI();
        }
        catch (Exception e)
        {
            Debug.LogWarning("Failed to update lobby state: " + e.Message);
        }
    }


    void RefreshLobbyUI()
    {
        foreach (Transform child in healthBarContainer)
            Destroy(child.gameObject);

        if (currentLobby == null) return;

        foreach (var player in currentLobby.Players)
        {
            string id = player.Id;
            bool isYou = id == AuthenticationService.Instance.PlayerId;
            bool isHost = currentLobby.HostId == id;

            AddPlayerUI(id, isYou, isHost);
        }
    }

    void AddPlayerUI(string playerName, bool isYou, bool isHost)
    {
        var ui = Instantiate(playerUIItemPrefab, healthBarContainer);
        ui.GetComponent<LobbyPlayerUI>().SetName(playerName);

        if (isHost)
        {
            playerName += " (Host)";
        }
        if (isYou)
        {
            playerName += " (You)";
        }

        ui.GetComponent<LobbyPlayerUI>().SetName(playerName);
    }

    public async void HostLobby()
    {
        try
        {
            // Create Relay allocation
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(4);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            // Create Lobby
            CreateLobbyOptions options = new CreateLobbyOptions
            {
                Data = new System.Collections.Generic.Dictionary<string, DataObject>
                {
                    {"joinCode", new DataObject(DataObject.VisibilityOptions.Public, joinCode)}
                }
            };

            currentLobby = await LobbyService.Instance.CreateLobbyAsync("MyGameLobby", 4, options);
            Debug.Log("Lobby created: " + currentLobby.Id);
            lobbyCodeText.text = "Lobby Code: " + currentLobby.LobbyCode;

            // Set IsHost to true
            IsHost = true;

            // Start heartbeat to keep it alive
            InvokeRepeating(nameof(SendHeartbeat), 15, 15);

            // Start Relay Host
            var utp = NetworkManager.Singleton.GetComponent<UnityTransport>();
            utp.SetHostRelayData(allocation.RelayServer.IpV4, (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes, allocation.Key, allocation.ConnectionData);

            NetworkManager.Singleton.StartHost();
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to host lobby: " + e);
        }

        // Add player UI
        AddPlayerUI(AuthenticationService.Instance.PlayerId, true, IsHost); RefreshLobbyUI();
    }

    public async void JoinLobbyByCode(string lobbyCode)
    {
        if (currentLobby != null)
        {
            await LobbyService.Instance.RemovePlayerAsync(currentLobby.Id, AuthenticationService.Instance.PlayerId);
        }

        try
        {
            // Join Lobby
            JoinLobbyByCodeOptions options = new JoinLobbyByCodeOptions();
            currentLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode, options);

            Debug.Log("Joined lobby: " + currentLobby.Id);
            lobbyCodeText.text = "Lobby Code: " + currentLobby.LobbyCode;

            // Get joinCode for Relay from lobby data
            string relayJoinCode = currentLobby.Data["joinCode"].Value;

            // Join Relay as Client
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(relayJoinCode);

            var utp = NetworkManager.Singleton.GetComponent<UnityTransport>();
            utp.SetClientRelayData(joinAllocation.RelayServer.IpV4, (ushort)joinAllocation.RelayServer.Port,
                joinAllocation.AllocationIdBytes, joinAllocation.Key, joinAllocation.ConnectionData, joinAllocation.HostConnectionData);

            NetworkManager.Singleton.StartClient();
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to join lobby: " + e);
        }

        IsHost = false;

        // Add player UI
        AddPlayerUI(AuthenticationService.Instance.PlayerId, true, IsHost); RefreshLobbyUI();
    }

    async void SendHeartbeat()
    {
        if (currentLobby != null)
        {
            try
            {
                await LobbyService.Instance.SendHeartbeatPingAsync(currentLobby.Id);
            }
            catch (Exception e)
            {
                Debug.LogWarning("Heartbeat failed: " + e.Message);
            }
        }
    }
}
