using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class EnemyShooting : NetworkBehaviour
{
    public GameObject bulletPrefab;
    public float shootInterval = 2f;
    public float bulletSpeed = 10f;

    [Header("Slow Effect")]
    public float slowDuration = 7f;
    public float slowMultiplier = 0.5f;         // bullet speed reduced by this
    public float shootIntervalMultiplier = 2f;  // interval increased by this

    private Transform shootPoint;
    private bool isSlowed = false;
    private float originalShootInterval;
    private float originalBulletSpeed;

    private void Start()
    {
        shootPoint = transform;

        originalShootInterval = shootInterval;
        originalBulletSpeed = bulletSpeed;

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
                    float speed = isSlowed ? originalBulletSpeed * slowMultiplier : originalBulletSpeed;
                    rb.linearVelocity = direction * speed;
                }
            }

            float waitTime = isSlowed ? originalShootInterval * shootIntervalMultiplier : originalShootInterval;
            yield return new WaitForSeconds(waitTime);
        }
    }

    private Transform GetRandomPlayer()
    {
        if (PlayerController.AllPlayers.Count == 0) return null;
        int index = Random.Range(0, PlayerController.AllPlayers.Count);
        return PlayerController.AllPlayers[index].transform;
    }

    [ClientRpc]
    public void SlowEnemyClientRpc(float duration)
    {
        if (isSlowed) return;

        StartCoroutine(ApplySlow(duration));
    }

    private IEnumerator ApplySlow(float duration)
    {
        isSlowed = true;
        Debug.Log("Enemy is slowed!");
        yield return new WaitForSeconds(duration);
        isSlowed = false;
        Debug.Log("Enemy slow ended.");
    }
}