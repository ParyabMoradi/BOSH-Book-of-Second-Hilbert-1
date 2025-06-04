using UnityEngine;
using Unity.Netcode;

public class SpikyColumn : NetworkBehaviour
{
    [Tooltip("Speed at which the platform moves.")]
    [SerializeField] private float moveSpeed = 2f;

    [Tooltip("Distance the platform moves from its start position.")]
    [SerializeField] private float moveDistance = 2f;

    [Tooltip("Direction the platform moves in (normalized).")]
    [SerializeField] private Vector3 moveDirection = Vector3.down;

    [Tooltip("Points in world space the platform will move between.")]
    public Vector3[] pathPoints;

    private int currentTargetIndex = 0;
    private int direction = 1; // 1 = forward, -1 = backward

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            // Initialize the platform position on the server
            if (pathPoints != null && pathPoints.Length > 0)
            {
                // transform.position = pathPoints[0];
            }
        }
    }

    void Awake()
    {
        Debug.Log("Platform awake position: " + transform.position);
    }

    void Start()
    {
        if (IsServer)
        {
            // Normalize moveDirection to ensure consistent movement
            Vector3 dir = moveDirection.normalized;
            pathPoints = new Vector3[] { transform.position, transform.position + dir * moveDistance };
        }

        Debug.Log("Platform Start position: " + transform.position);
    }

    void Update()
    {
        if (!IsServer) return; // Prevent clients from moving it

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
