using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerHealth : NetworkBehaviour
{
    [SerializeField] private int maxHealth = 4;

    private NetworkVariable<int> currentHealth = new NetworkVariable<int>(
        4, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public NetworkVariable<bool> isDead = new NetworkVariable<bool>(
        false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            currentHealth.Value = maxHealth;
            isDead.Value = currentHealth.Value <= 0;
        }

        // Update UI for this player when joining (update for both in next frame)
        UpdateHeartUIClientRpc(currentHealth.Value, OwnerClientId);
    }

    [ServerRpc(RequireOwnership = false)]
    public void TakeDamageServerRpc(int amount)
    {
        if (isDead.Value) return;

        currentHealth.Value -= amount;
        currentHealth.Value = Mathf.Max(currentHealth.Value, 0);

        Debug.Log($"Player {OwnerClientId} took damage. Current health: {currentHealth.Value}");

        // Update UI for everyone
        UpdateHeartUIClientRpc(currentHealth.Value, OwnerClientId);

        if (currentHealth.Value <= 0 && !isDead.Value)
        {
            isDead.Value = true;
            MatchManager.Instance?.OnPlayerDied(); // Safe null-check
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void ResetHealthServerRpc()
    {
        if (!IsServer) return;

        currentHealth.Value = maxHealth;
        isDead.Value = false;

        UpdateHeartUIClientRpc(currentHealth.Value, OwnerClientId);
    }

    [ClientRpc]
    private void UpdateHeartUIClientRpc(int health, ulong targetClientId)
    {
        if (HeartContainerUI.Instance != null)
        {
            HeartContainerUI.Instance.SetHeartCount((int)targetClientId, health);
        }
    }
}