using UnityEngine;
// Scenemanager
using UnityEngine.SceneManagement;

public class LevelDoorInteraction2D : MonoBehaviour
{
    private bool canInteract = false;
    private GameObject currentDoor;


    void Update()
    {
        if (canInteract && Input.GetKeyDown(KeyCode.E))
        {
            InteractWithDoor();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("levelDoor"))
        {
            canInteract = true;
            currentDoor = other.gameObject;
            Debug.Log("Entered door zone: " + currentDoor.name);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("levelDoor"))
        {
            canInteract = false;
            Debug.Log("Exited door zone");
            currentDoor = null;
        }
    }

    void InteractWithDoor()
    {
        if (!LobbyManagerNetwork.IsHost)
        {
            Debug.Log("You are not the host. Cannot interact with door.");
            return;
        }

        Debug.Log("Interacted with: " + currentDoor.name);

        Debug.Log("Host started level " + currentDoor.name);
        // Load the scene
        SceneManager.LoadScene(currentDoor.name);

    }
}
