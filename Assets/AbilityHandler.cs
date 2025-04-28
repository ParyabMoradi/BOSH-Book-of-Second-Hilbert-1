using UnityEngine;
using System.Collections.Generic;

public class AbilityHandler : MonoBehaviour
{
    [System.Serializable]
    public class Ability
    {
        public GameObject AbilityIndicator; // Assign your AbilityIndicator GameObject in the Inspector
        public float CooldownDuration = 5f; // Cooldown duration in seconds
        [HideInInspector] public bool IsOnCooldown = false;
        [HideInInspector] public float CooldownTimer = 0f;
    }

    public List<Ability> Abilities = new List<Ability>();
    public float flashSpeed = 1.5f;

    void Start()
    {
        // Ensure all AbilityIndicators are hidden at the start
        foreach (var ability in Abilities)
        {
            if (ability.AbilityIndicator != null)
            {
                ability.AbilityIndicator.SetActive(false);
            }
        }
    }

    void Update()
    {
        // Check for ability activation (keys 1-9)
        for (int i = 0; i < Abilities.Count; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i) && !Abilities[i].IsOnCooldown)
            {
                UseAbility(i);
            }
        }

        // Handle cooldown logic for each ability
        foreach (var ability in Abilities)
        {
            if (ability.IsOnCooldown)
            {
                ability.CooldownTimer -= Time.deltaTime;

                // Pulse the AbilityIndicator
                if (ability.AbilityIndicator != null)
                {
                    float alpha = Mathf.PingPong(Time.time * flashSpeed, 1f); // Adjust flash speed
                    CanvasRenderer canvasRenderer = ability.AbilityIndicator.GetComponent<CanvasRenderer>();
                    if (canvasRenderer != null)
                    {
                        Color color = canvasRenderer.GetColor();
                        color.a = alpha;
                        canvasRenderer.SetColor(color);
                    }
                }

                if (ability.CooldownTimer <= 0f)
                {
                    EndCooldown(ability);
                }
            }
        }
    }

    void UseAbility(int index)
    {
        var ability = Abilities[index];

        // Start cooldown
        ability.IsOnCooldown = true;
        ability.CooldownTimer = ability.CooldownDuration;

        // Show the AbilityIndicator
        if (ability.AbilityIndicator != null)
        {
            ability.AbilityIndicator.SetActive(true);
        }

        // Add your ability logic here
        Debug.Log($"Ability {index + 1} used!");
    }

    void EndCooldown(Ability ability)
    {
        ability.IsOnCooldown = false;

        // Hide the AbilityIndicator
        if (ability.AbilityIndicator != null)
        {
            ability.AbilityIndicator.SetActive(false);
        }

        Debug.Log("Cooldown ended!");
    }
}
