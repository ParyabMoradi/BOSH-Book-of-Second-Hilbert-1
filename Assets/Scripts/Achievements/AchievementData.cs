using UnityEngine;

[CreateAssetMenu(fileName = "New Achievement", menuName = "Achievements/Achievement")]
public class AchievementData : ScriptableObject
{
    public string id;                 // Unique identifier
    public string description;       // Short description
    public Sprite icon;              // The image to show
}
