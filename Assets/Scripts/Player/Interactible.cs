using UnityEngine;
using UnityEngine.SceneManagement;

public class Interactible : MonoBehaviour
{
    public enum InteractibleType { PressurePlate, EnemySpawnerActivator, LevelTeleport /*, Add more types here */ }
    [SerializeField] private InteractibleType type;
    [SerializeField] private GameObject objectToDisable; // For PressurePlate
    [SerializeField] private EnemySpawner enemySpawner;  // For EnemySpawnerActivator

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
                int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
                if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
                {
                    SceneManager.LoadScene(nextSceneIndex);
                }
                break;
            // Add more cases for new types here
        }
    }
}