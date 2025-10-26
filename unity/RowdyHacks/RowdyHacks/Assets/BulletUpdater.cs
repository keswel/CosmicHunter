using System.Collections.Generic;
using UnityEngine;

public class BulletUpdater : MonoBehaviour
{
    private List<GameObject> bullets = new List<GameObject>();
    private WeaponController player; // reference to the player

    void Start()
    {
        // Find the PlayerController in the scene
        player = FindFirstObjectByType<WeaponController>();

        // Store all child heart objects
        foreach (Transform child in transform)
        {
            bullets.Add(child.gameObject);
        }
    }

    void Update()
    {
        if (player == null) return;

        UpdateBullets(player.ammoInCylinder); // use player's current health every frame
    }

    public void UpdateBullets(int currentAmmo)
    {
        for (int i = 0; i < bullets.Count; i++)
        {
            bullets[i].SetActive(i >= bullets.Count - currentAmmo);
        }

    }
}
