using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private void Awake()
    {
        // Makes this object persistent across scenes
        DontDestroyOnLoad(gameObject);
    }
}
