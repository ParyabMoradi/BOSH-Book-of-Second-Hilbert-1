using UnityEngine;
using UnityEngine.SceneManagement;

public class Interactible : MonoBehaviour
{
    public enum InteractibleType { PressurePlate, EnemySpawnerActivator, LevelTeleport, CameraFocus, CoopPressurePlate }
    [SerializeField] private InteractibleType type;
    [SerializeField] private GameObject objectToDisable; // For PressurePlate and CoopPressurePlate
    [SerializeField] private EnemySpawner enemySpawner;  // For EnemySpawnerActivator
    [SerializeField] private SceneLoader sceneLoader; // For LevelTeleport
    [SerializeField] private CameraController cameraController; // For CameraFocus
    [SerializeField] private Transform cameraFocusTarget; // For CameraFocus
    [SerializeField] private float zoomLevel = 8f; // For CameraFocus
    [SerializeField] private Collider2D cameraClampArea; // For CameraFocus

    // For CoopPressurePlate
    [SerializeField] private Collider2D button1;
    [SerializeField] private Collider2D button2;
    private bool player1OnButton = false;
    private bool player2OnButton = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        switch (type)
        {
            case InteractibleType.PressurePlate:
                if (objectToDisable != null)
                    objectToDisable.SetActive(false);
                break;
            case InteractibleType.EnemySpawnerActivator:
                if (enemySpawner != null)
                    enemySpawner.isSpawnable = true;
                break;
            case InteractibleType.LevelTeleport:
                if (sceneLoader != null)
                {
                    sceneLoader.RequestSceneLoad();
                }
                break;
            case InteractibleType.CameraFocus:
                if (cameraController != null && cameraFocusTarget != null)
                {
                    Bounds? area = cameraClampArea != null ? cameraClampArea.bounds : (Bounds?)null;
                    cameraController.FocusOnTarget(cameraFocusTarget, zoomLevel, area);
                }
                break;
            case InteractibleType.CoopPressurePlate:
                // Do nothing here, handled in button triggers
                break;
        }
    }

    // CoopPressurePlate logic
    void Update()
    {
        if (type != InteractibleType.CoopPressurePlate) return;
        if (objectToDisable == null || button1 == null || button2 == null) return;

        // Reset
        player1OnButton = false;
        player2OnButton = false;

        // Find all players
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
