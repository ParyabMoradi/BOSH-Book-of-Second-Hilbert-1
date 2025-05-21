using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class EnemyShooting : NetworkBehaviour
{
    public GameObject bulletPrefab;     // The bullet prefab to spawn
    public float shootInterval = 2f;    // Time interval between shots
    public float bulletSpeed = 10f;     // Speed of the bullet

    private Transform shootPoint;

    private void Start()
    {
        shootPoint = transform;

        if (IsServer)
        {
            StartCoroutine(ShootBullets());
        }
    }

    private IEnumerator ShootBullets()
    {
        while (true)
        {
            Transform target = GetRandomPlayer();
            if (target != null)
            {
                GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, Quaternion.identity);
                NetworkObject netObj = bullet.GetComponent<NetworkObject>();
                netObj.Spawn();

                Vector2 direction = (target.position - shootPoint.position).normalized;
                Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.linearVelocity = direction * bulletSpeed;
                }
            }

            yield return new WaitForSeconds(shootInterval);
        }
    }

    private Transform GetRandomPlayer()
    {
        if (PlayerController.AllPlayers.Count == 0) return null;

        int index = Random.Range(0, PlayerController.AllPlayers.Count);
        return PlayerController.AllPlayers[index].transform;
    }
}
