using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class Interactible : MonoBehaviour
{
    public enum InteractibleType { PressurePlate, EnemySpawnerActivator, LevelTeleport, CameraFocus, CoopPressurePlate }

    [SerializeField] private InteractibleType type;

    [Header("Optional Object To Disable")]
    [SerializeField] private GameObject objectToDisable;

    [Header("Enemy Spawner")]
    [SerializeField] private EnemySpawner enemySpawner;

    [Header("Level Teleport")]
    [SerializeField] private SceneLoader sceneLoader;

    [Header("Camera Focus")]
    [SerializeField] private CameraController cameraController;
    [SerializeField] private Transform cameraFocusTarget;
    [SerializeField] private float zoomLevel = 8f;
    [SerializeField] private Collider2D cameraClampArea;

    [Header("Co-op Pressure Plate")]
    [SerializeField] private Collider2D button1;
    [SerializeField] private Collider2D button2;

    private Tilemap tilemapToColor;
    private bool player1OnButton = false;
    private bool player2OnButton = false;
    private bool pressurePlateActivated = false;

    [Header("Tilemap Color Change")]
    private readonly Color triggeredColor = Color.green;
    private readonly Color normalColor = Color.white;

    void Awake()
    {
        // Automatically try to get Tilemap on this GameObject or its children
        tilemapToColor = GetComponent<Tilemap>();
        if (tilemapToColor == null)
            tilemapToColor = GetComponentInChildren<Tilemap>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        switch (type)
        {
            case InteractibleType.PressurePlate:
                if (!pressurePlateActivated)
                {
                    pressurePlateActivated = true;

                    if (objectToDisable != null)
                        objectToDisable.SetActive(false);

                    if (tilemapToColor != null)
                        tilemapToColor.color = triggeredColor;
                }
                break;

            case InteractibleType.EnemySpawnerActivator:
                if (enemySpawner != null)
                    enemySpawner.isSpawnable = true;
                break;

            case InteractibleType.LevelTeleport:
                if (sceneLoader != null)
                    sceneLoader.RequestSceneLoad();
                break;

            case InteractibleType.CameraFocus:
                if (cameraController != null && cameraFocusTarget != null)
                {
                    Bounds? area = cameraClampArea != null ? cameraClampArea.bounds : (Bounds?)null;
                    cameraController.FocusOnTarget(cameraFocusTarget, zoomLevel, area);
                }
                break;

            case InteractibleType.CoopPressurePlate:
                // Logic handled in Update
                break;
        }
    }

    void Update()
    {
        if (type != InteractibleType.CoopPressurePlate) return;
        if (objectToDisable == null || button1 == null || button2 == null) return;

        player1OnButton = false;
        player2OnButton = false;

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (var player in players)
        {
            if (button1.bounds.Contains(player.transform.position))
                player1OnButton = true;
            if (button2.bounds.Contains(player.transform.position))
                player2OnButton = true;
        }

        if (player1OnButton && player2OnButton)
        {
            objectToDisable.SetActive(false);
        }
    }
}
