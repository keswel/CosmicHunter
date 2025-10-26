using System.Collections.Generic;
using UnityEngine;
using Image = UnityEngine.UI.Image;

public class HealthController : MonoBehaviour
{
    public int DEFAULT_HEALTH = 5;
    public int health = 3;

    [Header("Heart Settings")]
    public Sprite fullHeart;   // Assign in Inspector
    public Sprite emptyHeart;  // Assign in Inspector
    public GameObject heartPrefab; // Prefab with Image component
    public float heartSpacing = 100f; // Distance between hearts

    private List<Image> Hearts = new List<Image>();

    void Start()
    {
        // Position container in top-right
        RectTransform containerRect = GetComponent<RectTransform>();
        containerRect.anchorMin = new Vector2(1, 1); // Top-right anchor
        containerRect.anchorMax = new Vector2(1, 1);
        containerRect.pivot = new Vector2(1, 1);
        containerRect.anchoredPosition = new Vector2(-100, -50); // Offset from top-right corner
        containerRect.sizeDelta = new Vector2(200, 50); // Ensure proper size

        // Instantiate heart UI images as children
        for (int i = 0; i < DEFAULT_HEALTH; i++)
        {
            GameObject heart = Instantiate(heartPrefab, transform);
            heart.name = "Heart" + i;

            // Position using RectTransform for UI (right to left)
            RectTransform rectTransform = heart.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(-i * heartSpacing, 0);

            // Get the Image component (not SpriteRenderer!)
            Image img = heart.GetComponent<Image>();
            img.sprite = fullHeart;
            Hearts.Add(img);
        }
    }

    void Update()
    {
        // Update heart sprites to match health
        for (int i = 0; i < Hearts.Count; i++)
        {
            Hearts[i].sprite = i < health ? fullHeart : emptyHeart;
        }

        // Test controls
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TakeDamage(1);
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            Heal(1);
        }
    }

    public void TakeDamage(int damage = 1)
    {
        health -= damage;
        if (health < 0) health = 0;
    }

    public void Heal(int amount = 1)
    {
        health += amount;
        if (health > DEFAULT_HEALTH) health = DEFAULT_HEALTH;
    }
}