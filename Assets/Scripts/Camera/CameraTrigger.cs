using UnityEngine;
using Unity.Cinemachine;

public class CameraTrigger : MonoBehaviour
{
    [Header("The camera to activate when triggered")]
    public CinemachineVirtualCamera targetCamera;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            CameraManager.Instance.SwitchCamera(targetCamera);
        }
    }
}