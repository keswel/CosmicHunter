using System.Collections.Generic;
using UnityEngine;

public class HeartUpdate : MonoBehaviour
{
    public static HeartUpdate instance; // singleton instance
    private List<GameObject> hearts = new List<GameObject>();
    private PlayerController player; // reference to the player
    void Awake()
    {
        // Ensure only one instance exists
        instance = this;
    }

    void Start()
    {
        // Find the PlayerController in the scene
        player = FindFirstObjectByType<PlayerController>();

        // Store all child heart objects
        foreach (Transform child in transform)
        {
            hearts.Add(child.gameObject);
        }
    }

    void Update()
    {
        if (player == null) return;

        UpdateHearts(player.health); // use player's current health every frame
    }
    public int GetHealth()
    {
        return player.health;
    }
    public void UpdateHearts(int currentHealth)
    {
        for (int i = 0; i < hearts.Count; i++)
        {
            // Activate hearts starting from the RIGHT side instead of LEFT
            hearts[i].SetActive(i >= hearts.Count - currentHealth);
        }
    }
}
