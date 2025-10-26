using UnityEngine;
using UnityEngine.SceneManagement;  // Add this for scene management

public class LoseTest : MonoBehaviour
{
    [SerializeField]
    private PlayerController player;
    private GameObject losePanel;  // Reference to the "Lose" child object
    private bool isGameOver = false;  // Track game over state

    void Start()
    {
        if (player == null)
        {
            player = FindObjectOfType<PlayerController>();
        }

        // Find the "Lose" child GameObject
        losePanel = transform.Find("Lose")?.gameObject;
        if (losePanel == null)
        {
            UnityEngine.Debug.LogWarning("Could not find 'Lose' child object!");
        }

        // Disable only the Lose panel, not the whole LoseScreen
        if (losePanel != null)
        {
            losePanel.SetActive(false);
        }
    }

    void Update()
    {
        if (player == null || losePanel == null) return;

        // Use PlayerController's health variable
        bool isDead = player.health <= 0;

        // Only update if state changed
        if (losePanel.activeSelf != isDead)
        {
            losePanel.SetActive(isDead);
            isGameOver = isDead;  // Track game over state

            // Disable player controls when dead
            if (isDead)
            {
                player.enabled = false;  // Disables the PlayerController component
                if (player.GetComponent<Rigidbody2D>() != null)
                {
                    player.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;  // Stop movement
                }
            }
        }

        // Check for restart input when game is over
        if (isGameOver && Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }
    }

    public void ForceGameOver()
    {
        if (player != null && losePanel != null)
        {
            player.health = 0;
            losePanel.SetActive(true);
            isGameOver = true;
            player.enabled = false;  // Disable player controls
            if (player.GetComponent<Rigidbody2D>() != null)
            {
                player.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
            }
        }
    }

    private void RestartGame()
    {
        // Reload the current scene
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
}