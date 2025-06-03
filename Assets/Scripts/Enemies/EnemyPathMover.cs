using UnityEngine;
using DG.Tweening;
using Unity.Netcode;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;


public class EnemyPathMover : NetworkBehaviour
{
    [Header("Path Settings")]
    public Vector2[] pathPositions; // Array of 2D positions for the path (Vector2)
    public float moveDuration = 5f;
    public PathType pathType = PathType.CatmullRom; // Smooth path
    public PathMode pathMode = PathMode.TopDown2D; // Use 2D movement
    public Ease easeType = Ease.Linear;
    public bool lookForward = false;

    public EnemySpawner enemySpawner;
    [SerializeField]
    private Transform enemyPrefab;
    private void Start()
    {
        if (pathPositions.Length < 2)
        {
            Debug.LogWarning("EnemyPathMover needs at least 2 path points!");
            return;
        }

        // Start looping movement along the path
        MoveAlongPath();
    }
    
    public override void OnNetworkSpawn()
    {
        // Only the server should handle the path movement
        if (!IsServer)
        {
            enabled = false;
            return;
        }
    }

    

    // Method to start moving along the path
    private void MoveAlongPath()
    {
        // Convert the Vector2 path to Vector3 (ignoring z-axis) for DOTween compatibility
        Vector3[] convertedPath = new Vector3[pathPositions.Length];
        for (int i = 0; i < pathPositions.Length; i++)
        {
            convertedPath[i] = new Vector3(pathPositions[i].x, pathPositions[i].y, 0f);
        }

        // Loop infinitely and smoothly along the 2D path
        transform.DOPath(convertedPath, moveDuration, pathType, pathMode)
            .SetEase(easeType)
            .SetOptions(lookForward)
            .SetLoops(-1, LoopType.Yoyo); // Yoyo loop to go back and forth smoothly
    }

    // Draw the path in the Scene view for visualizing
    private void OnDrawGizmos()
    {
        if (pathPositions == null || pathPositions.Length < 2)
            return;

        Gizmos.color = Color.green;

        // Draw lines between path positions (2D)
        for (int i = 0; i < pathPositions.Length - 1; i++)
        {
            Gizmos.DrawLine(pathPositions[i], pathPositions[i + 1]);
        }

        // Close the loop by drawing a line from the last point back to the first point
        // Gizmos.DrawLine(pathPositions[pathPositions.Length - 1], pathPositions[0]);
    }
}
