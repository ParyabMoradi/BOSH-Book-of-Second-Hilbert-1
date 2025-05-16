using UnityEngine;

public class PlatformMover : MonoBehaviour
{
    [Tooltip("Points in world space the platform will move between.")]
    public Vector3[] pathPoints;
    [Tooltip("Speed at which the platform moves.")]
    public float moveSpeed = 2f;

    private int currentTargetIndex = 0;
    private int direction = 1; // 1 = forward, -1 = backward

    void Start()
    {
        if (pathPoints != null && pathPoints.Length > 0)
        {
            transform.position = pathPoints[0];
        }
    }

    void Update()
    {
        if (pathPoints == null || pathPoints.Length < 2)
            return;

        Vector3 target = pathPoints[currentTargetIndex];
        transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target) < 0.01f)
        {
            currentTargetIndex += direction;
            if (currentTargetIndex >= pathPoints.Length)
            {
                currentTargetIndex = pathPoints.Length - 2;
                direction = -1;
            }
            else if (currentTargetIndex < 0)
            {
                currentTargetIndex = 1;
                direction = 1;
            }
        }
    }
}
