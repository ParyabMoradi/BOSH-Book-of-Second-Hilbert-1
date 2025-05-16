using UnityEngine;
using TMPro;

public class JoinCodeDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI codeText;

    void Start()
    {
        if (codeText != null)
            codeText.text = RelayManager.LastJoinCode;
    }
}
