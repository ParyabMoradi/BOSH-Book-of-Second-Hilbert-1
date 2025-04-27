using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }

    [Header("List of all Virtual Cameras")]
    public List<CinemachineVirtualCamera> virtualCameras = new List<CinemachineVirtualCamera>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void SwitchCamera(CinemachineVirtualCamera targetCamera)
    {
        foreach (var cam in virtualCameras)
        {
            if (cam != null)
                cam.gameObject.SetActive(false);
        }

        if (targetCamera != null)
            targetCamera.gameObject.SetActive(true);
    }
}