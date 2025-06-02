using UnityEngine;
using DG.Tweening;
using Unity.Netcode;
using System.Collections;

public class EnemyPathMover : NetworkBehaviour
{
    [Header("Path Settings")]
    public Vector2[] pathPositions;
    public float moveDuration = 5f;
    public PathType pathType = PathType.CatmullRom;
    public PathMode pathMode = PathMode.TopDown2D;
    public Ease easeType = Ease.Linear;
    public bool lookForward = false;

    private float originalMoveDuration;
    private Tween moveTween;
    private bool isSlowed = false;

    private void Start()
    {
        if (pathPositions.Length < 2)
        {
            Debug.LogWarning("EnemyPathMover needs at least 2 path points!");
            return;
        }

        originalMoveDuration = moveDuration;
        MoveAlongPath();
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
        {
            enabled = false;
            return;
        }
    }

    private void MoveAlongPath()
    {
        Vector3[] convertedPath = new Vector3[pathPositions.Length];
        for (int i = 0; i < pathPositions.Length; i++)
        {
            convertedPath[i] = new Vector3(pathPositions[i].x, pathPositions[i].y, 0f);
        }

        moveTween = transform.DOPath(convertedPath, moveDuration, pathType, pathMode)
            .SetEase(easeType)
            .SetOptions(lookForward)
            .SetLoops(-1, LoopType.Yoyo);
    }

    [ClientRpc]
    public void SlowMovementClientRpc(float duration)
    {
        if (!isSlowed)
        {
            StartCoroutine(SlowForDuration(duration));
        }
    }

    private IEnumerator SlowForDuration(float duration)
    {
        isSlowed = true;

        float slowedDuration = originalMoveDuration * 2f;
        moveTween.Kill(); // stop current tween
        moveDuration = slowedDuration;
        MoveAlongPath();

        yield return new WaitForSeconds(duration);

        moveTween.Kill();
        moveDuration = originalMoveDuration;
        MoveAlongPath();

        isSlowed = false;
    }

    private void OnDrawGizmos()
    {
        if (pathPositions == null || pathPositions.Length < 2)
            return;

        Gizmos.color = Color.green;

        for (int i = 0; i < pathPositions.Length - 1; i++)
        {
            Gizmos.DrawLine(pathPositions[i], pathPositions[i + 1]);
        }
    }
}
