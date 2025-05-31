using UnityEngine;
using UnityEngine.SceneManagement;

public class Interactible : MonoBehaviour
{
    public enum InteractibleType { PressurePlate, EnemySpawnerActivator, LevelTeleport, CameraFocus /*, Add more types here */ }
    [SerializeField] private InteractibleType type;
    [SerializeField] private GameObject objectToDisable; // For PressurePlate
    [SerializeField] private EnemySpawner enemySpawner;  // For EnemySpawnerActivator
    [SerializeField] private SceneLoader sceneLoader; // For LevelTeleport
    [SerializeField] private CameraController cameraController; // For CameraFocus
    [SerializeField] private Transform cameraFocusTarget; // For CameraFocus
    [SerializeField] private float zoomLevel = 8f; // New field for zoom
    [SerializeField] private Collider2D cameraClampArea; // New field for bounds

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
            // Add more cases for new types here
        }
    }
}