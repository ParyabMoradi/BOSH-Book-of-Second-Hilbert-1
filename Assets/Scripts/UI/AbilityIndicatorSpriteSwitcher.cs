using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class AbilityIndicatorSpriteSwitcher : MonoBehaviour
{
    [Header("References")]
    public Image abilityIndicatorImage;

    [Header("Role Sprites")]
    public Sprite boySprite;
    public Sprite girlSprite;

    private void Start()
    {
        if (abilityIndicatorImage == null)
        {
            abilityIndicatorImage = GetComponent<Image>(); // Or use GetComponentInChildren<Image>()
            Debug.Log(abilityIndicatorImage.name);
        }


        if (RoleManager.Instance == null)
        {
            Debug.LogError("RoleManager.Instance is null. Make sure it initializes before this script runs.");
            return;
        }

        ulong localId = NetworkManager.Singleton.LocalClientId;
        // CharacterType role = RoleManager.Instance.GetOrAssignRole(localId);

        switch ((int)localId)
        {
            case 0:
                abilityIndicatorImage.sprite = boySprite;
                break;

            case 1:
                abilityIndicatorImage.sprite = girlSprite;
                break;

            default:
                Debug.LogWarning("Unknown character role for sprite switch.");
                break;
        }
    }


}