using UnityEngine;
using TMPro;

public class LobbyPlayerUI : MonoBehaviour
{
    public TMP_Text nameText;

    public void SetName(string playerName)
    {
        nameText.text = playerName;
    }
}
