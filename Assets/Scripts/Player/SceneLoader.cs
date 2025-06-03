using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class SceneLoader : NetworkBehaviour
{
    public string sceneName = "MyScene"; // The name of your scene

    [ServerRpc(RequireOwnership = false)]
    public void LoadSceneServerRpc()
    {
        // Load the scene on the server
        NetworkManager.Singleton.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

    // Call this function from a client (e.g., a button click)
    public void RequestSceneLoad()
    {
        LoadSceneServerRpc(); // Call the server method
    }
}
