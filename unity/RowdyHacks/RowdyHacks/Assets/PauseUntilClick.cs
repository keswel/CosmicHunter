using UnityEngine;

public class PauseUntilClick : MonoBehaviour
{
    public GameObject blackbackground; // assign in inspector
    public GameObject start;           // assign in inspector

    private bool isPaused = true;

    void Start()
    {
        // Pause the game at the start
        Time.timeScale = 0f;
    }

    void Update()
    {
        if (isPaused && Input.GetMouseButtonDown(0)) // left click
        {
            ResumeGame();
        }
    }

    void ResumeGame()
    {
        Time.timeScale = 1f;  // Resume normal time
        isPaused = false;

        // Disable the child objects
        if (blackbackground != null) blackbackground.SetActive(false);
        if (start != null) start.SetActive(false);
    }
}
