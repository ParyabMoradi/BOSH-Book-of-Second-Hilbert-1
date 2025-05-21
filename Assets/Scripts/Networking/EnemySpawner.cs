using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class EnemySpawner : NetworkBehaviour
{
    [SerializeField]
    private int maxEnemyCount = 5;
    [SerializeField]
    private float spawnCooldown = 2f;
    [SerializeField]
    private Transform enemyPrefab;
    [SerializeField]
    private Vector2 minSpawnPos;
    [SerializeField]
    private Vector2 maxSpawnPos;

    public List<Transform> enemies = new List<Transform>();

    private void Start()
    {

    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
        {
            enabled = false;
            return;
        }
        StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies()
{
    while (true)
    {
        int toSpawn = maxEnemyCount - enemies.Count;

        for (int i = 0; i < toSpawn; i++)
        {
            Vector2 spawnPos = new Vector2(
                Random.Range(minSpawnPos.x, maxSpawnPos.x),
                Random.Range(minSpawnPos.y, maxSpawnPos.y)
            );

            Transform enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity, transform);

            var enemyMover = enemy.GetComponent<EnemyPathMover>();
            enemyMover.enemySpawner = this;

            // Optional: Set dynamic path
            enemyMover.pathPositions = new Vector2[] {
                spawnPos,
                spawnPos + new Vector2(2f, 0),
                spawnPos + new Vector2(2f, 2f),
                spawnPos + new Vector2(0f, 2f)
            };

            enemy.GetComponent<NetworkObject>().Spawn();
            enemies.Add(enemy);

            // Optional: add a delay between each clone
            yield return new WaitForSeconds(0.2f);
        }

        yield return new WaitForSeconds(spawnCooldown);
    }
}


}
