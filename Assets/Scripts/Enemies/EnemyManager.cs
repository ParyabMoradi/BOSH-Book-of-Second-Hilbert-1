using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class EnemyManager : NetworkBehaviour
{
    private const float sequenceTimeout = 5f;

    private struct PlayerSequenceStatus
    {
        public NetworkObjectReference playerReference;
        public bool sequenceFinished;
    }

    private struct EnemyStatus
    {
        public NetworkObjectReference enemyReference;
        public PlayerSequenceStatus[] playerStatuses;
        public float timer;  // Countdown timer
    }

    private List<PlayerSequenceStatus> playerStatuses = new();
    private List<EnemyStatus> enemyStatuses = new();
    
    private void Update()
    {
        if (!IsServer) return;

        for (int i = 0; i < enemyStatuses.Count; i++)
        {
            var enemy = enemyStatuses[i];
            var p0 = enemy.playerStatuses[0];
            var p1 = enemy.playerStatuses[1];

            // Debug.Log($"[Enemy {i}] Timer: {enemy.timer:F2} | P0 finished: {p0.sequenceFinished} | P1 finished: {p1.sequenceFinished}");

            // If exactly one player has finished
            if (p0.sequenceFinished != p1.sequenceFinished)
            {
                enemy.timer -= Time.deltaTime;
                // Debug.Log($"[Enemy {i}] One player finished. Timer counting down: {enemy.timer:F2}");

                if (enemy.timer <= 0f)
                {
                    // Debug.Log($"[Enemy {i}] Timer expired! Resetting finished player's sequence.");

                    var finishedPlayer = p0.sequenceFinished ? p0 : p1;

                    ResetPlayerSequence(finishedPlayer.playerReference, enemy.enemyReference);

                    // Reset finished status
                    for (int j = 0; j < enemy.playerStatuses.Length; j++)
                    {
                        if (enemy.playerStatuses[j].playerReference.Equals(finishedPlayer.playerReference))
                        {
                            enemy.playerStatuses[j].sequenceFinished = false;
                            break;
                        }
                    }

                    enemy.timer = sequenceTimeout;
                    enemyStatuses[i] = enemy;
                }
                else
                {
                    enemyStatuses[i] = enemy;
                }
            }
            else
            {
                // Both finished or both not finished
                if (enemy.timer < sequenceTimeout)
                    // Debug.Log($"[Enemy {i}] Both players same state (finished/not finished). Timer reset.");

                enemy.timer = sequenceTimeout;
                enemyStatuses[i] = enemy;
            }
        }
    }



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
            },
            timer = sequenceTimeout
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

        for (int i = 0; i < enemyStatuses.Count; i++)
        {
            if (enemyStatuses[i].enemyReference.Equals(enemyRef))
            {
                int finishedPlayerIndex = -1;

                // Mark the player who called this as finished
                for (int j = 0; j < enemyStatuses[i].playerStatuses.Length; j++)
                {
                    var netObj = enemyStatuses[i].playerStatuses[j].playerReference;
                    if (netObj.TryGet(out NetworkObject playerObj))
                    {
                        if (playerObj.OwnerClientId == playerId)
                        {
                            enemyStatuses[i].playerStatuses[j].sequenceFinished = true;
                            finishedPlayerIndex = j;
                            Debug.Log($"Player {playerId} finished sequence for enemy {i}");
                        }
                    }
                }

                // Now trigger timer for the other player (if they haven't finished)
                if (finishedPlayerIndex != -1)
                {
                    int otherPlayerIndex = 1 - finishedPlayerIndex; // Assumes only 2 players
                    var otherPlayerRef = enemyStatuses[i].playerStatuses[otherPlayerIndex].playerReference;

                    if (!enemyStatuses[i].playerStatuses[otherPlayerIndex].sequenceFinished &&
                        enemyStatuses[i].enemyReference.TryGet(out NetworkObject enemyObj) &&
                        otherPlayerRef.TryGet(out NetworkObject otherPlayerObj))
                    {
                        var enemyScript = enemyObj.GetComponent<EnemyClickSequence>();
                        if (enemyScript != null)
                        {
                            ulong otherClientId = otherPlayerObj.OwnerClientId;

                            var rpcParams = new ClientRpcParams
                            {
                                Send = new ClientRpcSendParams
                                {
                                    TargetClientIds = new[] { otherClientId }
                                }
                            };

                            Debug.Log($"Starting visual timer for Player {otherClientId} on Enemy {i}");
                            enemyScript.StartVisualTimerClientRpc(5f);
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
                    if (enemyRef.TryGet(out NetworkObject enemyObj))
                    {
                        EnemyClickSequence ecs = enemyObj.GetComponent<EnemyClickSequence>();
                        if (ecs != null)
                        {
                            ecs.DefeatClientRpc(); // Triggers defeat on both clients
                            Debug.Log($"Enemy defeated! RPC called on enemy: {enemyObj.name}");
                        }
                    }
                }

                return; // Exit once the correct enemy is found and processed
            }
        }

        Debug.LogWarning("Enemy not found for the given reference.");
    }


    private void ResetPlayerSequence(NetworkObjectReference playerRef, NetworkObjectReference enemyRef)
    {
        if (enemyRef.TryGet(out var enemyObject))
        {
            var enemyClickSequence = enemyObject.GetComponent<EnemyClickSequence>();

            if (enemyClickSequence != null)
            {
                if (playerRef.TryGet(out var playerObject))
                {
                    ulong clientId = playerObject.OwnerClientId;
                    enemyClickSequence.ResetSequenceClientRpc(clientId);
                }
            }
        }
    }

}
