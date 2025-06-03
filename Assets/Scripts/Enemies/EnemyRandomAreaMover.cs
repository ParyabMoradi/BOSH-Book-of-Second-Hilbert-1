using UnityEngine;
using DG.Tweening;
using Unity.Netcode;
using System.Collections;

public class EnemyRandomAreaMover : NetworkBehaviour
{
    [Header("Polygon Area (Clockwise or Counter-Clockwise)")]
    public Vector2[] polygonPoints;

    [Header("Movement Settings")]
    public float moveDuration = 2f;
    public float delayBetweenMoves = 0.5f;
    public Ease easeType = Ease.InOutSine;
    public bool lookAtTarget = false;

    private Bounds polygonBounds;
    private float originalMoveDuration;
    private bool isSlowed = false;

    private void Start()
    {
        if (polygonPoints == null || polygonPoints.Length < 3)
        {
            Debug.LogError("Polygon must have at least 3 points.");
            return;
        }

        polygonBounds = GetPolygonBounds(polygonPoints);
        originalMoveDuration = moveDuration;
        MoveToRandomPoint();
    }

    private void MoveToRandomPoint()
    {
        Vector2 randomPoint = GetRandomPointInPolygon();
        Vector3 targetPosition = new Vector3(randomPoint.x, randomPoint.y, transform.position.z);

        if (lookAtTarget)
        {
            Vector2 direction = (targetPosition - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        transform.DOMove(targetPosition, moveDuration)
            .SetEase(easeType)
            .OnComplete(() => Invoke(nameof(MoveToRandomPoint), delayBetweenMoves));
    }

    private Vector2 GetRandomPointInPolygon()
    {
        Vector2 point;
        int maxTries = 100, tries = 0;

        do
        {
            point = new Vector2(
                Random.Range(polygonBounds.min.x, polygonBounds.max.x),
                Random.Range(polygonBounds.min.y, polygonBounds.max.y)
            );
            tries++;
        } while (!IsPointInPolygon(point, polygonPoints) && tries < maxTries);

        return point;
    }

    private Bounds GetPolygonBounds(Vector2[] points)
    {
        Vector2 min = points[0];
        Vector2 max = points[0];

        foreach (Vector2 point in points)
        {
            min = Vector2.Min(min, point);
            max = Vector2.Max(max, point);
        }

        return new Bounds((min + max) / 2f, max - min);
    }

    private bool IsPointInPolygon(Vector2 point, Vector2[] poly)
    {
        int j = poly.Length - 1;
        bool inside = false;

        for (int i = 0; i < poly.Length; j = i++)
        {
            if (((poly[i].y > point.y) != (poly[j].y > point.y)) &&
                (point.x < (poly[j].x - poly[i].x) * (point.y - poly[i].y) / (poly[j].y - poly[i].y) + poly[i].x))
            {
                inside = !inside;
            }
        }

        return inside;
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
        moveDuration *= 2f; // slow movement
        yield return new WaitForSeconds(duration);
        moveDuration = originalMoveDuration;
        isSlowed = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (polygonPoints == null || polygonPoints.Length < 3)
            return;

        Gizmos.color = Color.yellow;
        for (int i = 0; i < polygonPoints.Length; i++)
        {
            Vector2 current = polygonPoints[i];
            Vector2 next = polygonPoints[(i + 1) % polygonPoints.Length];
            Gizmos.DrawLine(current, next);
        }
    }
}