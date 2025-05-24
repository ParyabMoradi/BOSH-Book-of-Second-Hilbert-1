using UnityEngine;
using UnityEngine.SceneManagement;

public class Interactible : MonoBehaviour
{
    public enum InteractibleType { PressurePlate, EnemySpawnerActivator, LevelTeleport /*, Add more types here */ }
    [SerializeField] private InteractibleType type;
    [SerializeField] private GameObject objectToDisable; // For PressurePlate
    [SerializeField] private EnemySpawner enemySpawner;  // For EnemySpawnerActivator
    [SerializeField] private SceneLoader sceneLoader; // For LevelTeleport

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
            // Add more cases for new types here
        }
    }
}