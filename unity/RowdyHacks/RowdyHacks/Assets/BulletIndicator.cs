using System.Collections.Generic;
using UnityEngine;

public class BulletIndicator : MonoBehaviour
{
    private PlayerController player; // reference to the player

    void Start()
    {
        // Find the PlayerController in the scene
        player = FindFirstObjectByType<PlayerController>();
        for (int i=0; i<transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (player == null) return;

        UpdateAmmo(player.health); // use player's current health every frame
    }

    public void UpdateAmmo(int currentAmmo)
    {
        for (int i = 0; i < 6; i++)
        {

        }
    }
}
