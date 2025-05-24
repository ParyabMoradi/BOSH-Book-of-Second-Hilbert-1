using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class EnemyManager : NetworkBehaviour
{
    private struct PlayerSequenceStatus
    {
        public NetworkObjectReference playerReference;
        public bool sequenceFinished;
    }

    private struct EnemyStatus
    {
        public NetworkObjectReference enemyReference;
        public PlayerSequenceStatus[] playerStatuses;
    }

    private List<PlayerSequenceStatus> playerStatuses = new();
    private List<EnemyStatus> enemyStatuses = new();

    // Called by players to register themselves with the manager (only works on server/host)
    [ServerRpc(RequireOwnership = false)]
    public void RegisterPlayerServerRpc(NetworkObjectReference playerRef)
    {
        if (!IsServer)
        {
            Debug.Log("Not server");
            return;
        }

        playerStatuses.Add(new PlayerSequenceStatus
        {
            playerReference = playerRef,
            sequenceFinished = false
        });

        Debug.Log("Player registered. Total players: " + playerStatuses.Count);
        for (int i = 0; i < playerStatuses.Count; i++)
        {
            Debug.Log($"Player {playerStatuses[i].playerReference}: Finished = {playerStatuses[i].sequenceFinished}");
        }
    }

    // Register an enemy using the first 2 registered players
    [ServerRpc(RequireOwnership = false)]
    public void RegisterEnemyServerRpc(NetworkObjectReference enemyRef)
    {
        if (!IsServer)
        {
            Debug.Log("Not server enemy");
            return;
        }

        StartCoroutine(WaitAndRegisterEnemy(enemyRef));
    }

    private IEnumerator WaitAndRegisterEnemy(NetworkObjectReference enemyRef)
    {
        // Wait until both players are registered
        yield return new WaitUntil(() => playerStatuses.Count >= 2);

        var enemyStatus = new EnemyStatus
        {
            enemyReference = enemyRef,
            playerStatuses = new PlayerSequenceStatus[2]
            {
                playerStatuses[0],
                playerStatuses[1]
            }
        };

        enemyStatuses.Add(enemyStatus);

        Debug.Log("Enemy registered. Total enemies: " + enemyStatuses.Count);
        for (int i = 0; i < enemyStatuses.Count; i++)
        {
            Debug.Log($"Enemy {i}: Ref = {enemyStatuses[i].enemyReference}");
            for (int j = 0; j < enemyStatuses[i].playerStatuses.Length; j++)
            {
                Debug.Log($"   Player {j}: Finished = {enemyStatuses[i].playerStatuses[j].sequenceFinished}");
            }
        }
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void PlayerFinishedSequenceServerRpc(ulong playerId, NetworkObjectReference enemyRef)
    {
        if (!IsServer) return;

        Debug.Log(playerId);
        for (int i = 0; i < enemyStatuses.Count; i++)
        {
            if (enemyStatuses[i].enemyReference.Equals(enemyRef))
            {
                for (int j = 0; j < enemyStatuses[i].playerStatuses.Length; j++)
                {
                    var netObj = enemyStatuses[i].playerStatuses[j].playerReference;
                    if (netObj.TryGet(out NetworkObject playerObj))
                    {
                        if (playerObj.OwnerClientId == playerId)
                        {
                            enemyStatuses[i].playerStatuses[j].sequenceFinished = true;
                            Debug.Log($"Player {playerId} finished sequence for enemy {i}");
                        }
                    }
                }

                // Check if both players are done
                bool bothFinished = true;
                foreach (var status in enemyStatuses[i].playerStatuses)
                {
                    if (!status.sequenceFinished)
                    {
                        bothFinished = false;
                        break;
                    }
                }

                if (bothFinished)
                {
                    Debug.Log($"Enemy {i} defeated by both players!");
                    // Later: Call Defeat() on the enemy object here
                }

                return; // Exit once the correct enemy is found and processed
            }
        }

        Debug.LogWarning("Enemy not found for the given reference.");
    }


}
