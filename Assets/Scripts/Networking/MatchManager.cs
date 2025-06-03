using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class MatchManager : NetworkBehaviour
{
    public static MatchManager Instance;
    public CameraController cameraController;

    private void Awake() => Instance = this;

    private int deadCount = 0;

    // public NetworkVariable<bool> isGameFrozen = new NetworkVariable<bool>(
    //     false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Everyone
    // );

    public void OnPlayerDied()
    {
        deadCount++;
        //isGameFrozen.Value = true;
        Time.timeScale = 0;
        ShowFailureScreenClientRpc();
    }

    [ClientRpc]
    private void ShowFailureScreenClientRpc()
    {
        bool isHost = NetworkManager.Singleton.IsHost;
        UIManager.Instance.ShowEndScreen(isHost);
    }

    [ClientRpc]
private void SetCameraTargetClientRpc()
{
    StartCoroutine(WaitAndAssignCameraTarget());
}

private IEnumerator WaitAndAssignCameraTarget()
{
    ulong localClientId = NetworkManager.Singleton.LocalClientId;

    // Wait until the player's object exists in AllPlayers
    while (PlayerController.AllPlayers.Count <= (int)localClientId ||
           PlayerController.AllPlayers[(int)localClientId] == null)
    {
        yield return null;
    }

    Transform playerTransform = PlayerController.AllPlayers[(int)localClientId].transform;

    // Optional: wait until transform.position is valid (e.g. not Vector3.zero)
    yield return new WaitUntil(() => playerTransform.position != Vector3.zero);

    CameraController cameraController = Camera.main?.GetComponent<CameraController>();
    if (cameraController != null)
    {
        cameraController.SetCameraTarget(playerTransform);
        Debug.Log($"[Camera] Target set to player {localClientId} at {playerTransform.position}");
    }
    else
    {
        Debug.LogWarning("[Camera] CameraController not found on main camera.");
    }
}

    [ClientRpc]
    public void RestartLevelClientRpc()
    {
        Debug.Log(PlayerController.AllPlayers.Count + " players in the game");

        // Get camera
        var cameraFollow = Camera.main.GetComponent<CameraFollow>();

        foreach (var player in PlayerController.AllPlayers)
        {
            if (player.IsServer) 
            {
                player.GetComponent<PlayerHealth>().ResetHealthServerRpc();
            }

            // var health = player.GetComponent<PlayerHealth>();
            // health.ResetHealth();

            // write a line to reset the player's position
            Debug.Log($"Resetting position for player {player.OwnerClientId}");
            // Reset the player's position
            player.GetComponent<PlayerMovement>().resetPosition = true;
        }

        Debug.Log("Owner client ID: " + NetworkManager.Singleton.LocalClientId);
        // Remove host-only camera set
        cameraController.SetCameraTarget(PlayerController.AllPlayers[(int)NetworkManager.Singleton.LocalClientId].transform);

        // Notify all clients to set their camera target
        SetCameraTargetClientRpc();

        deadCount = 0;
        //isGameFrozen.Value = false;
        Time.timeScale = 1;

        NetworkManager.Singleton.SceneManager.LoadScene("Level 1", LoadSceneMode.Single);
    }

    // [ClientRpc]
    // public void RestartLevelClientRpc()
    // {
    //     Debug.Log(PlayerController.AllPlayers.Count + " players in the game");
    //     foreach (var player in PlayerController.AllPlayers)
    //     {
    //         var health = player.GetComponent<PlayerHealth>();
    //         health.ResetHealth();

    //         // write a line to reset the player's position
    //         Debug.Log($"Resetting position for player {player.OwnerClientId}");
    //         // Reset the player's position
    //         player.GetComponent<PlayerMovement>().resetPosition = true;



    //     }

    //     deadCount = 0;
    //     //isGameFrozen.Value = false;
    //     Time.timeScale = 1;
    //     // string currentSceneName = SceneManager.GetActiveScene().name;
    //     // NetworkManager.Singleton.SceneManager.LoadScene(currentSceneName, LoadSceneMode.Single);

     
    //     NetworkManager.Singleton.SceneManager.LoadScene("Level 1", LoadSceneMode.Single);
    // }
        
    
}
