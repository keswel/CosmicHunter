using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnerController : MonoBehaviour
{
    //public GameObject alienPrefab;    // assign in Inspector
    //public int enemyCount = 10;       // number of enemies to spawn
    //public float w = 20f;
    //public float h = 20f;
    //public GameObject playerObj;      // assign your player here

    //private List<GameObject> alienList = new List<GameObject>();

    //Vector2 GetRandomPosition()
    //{
    //    return new Vector2(Random.Range(-w, w), Random.Range(-h, h));
    //}

    //void Start()
    //{
    //    GameObject alienContainer = new GameObject("AlienContainer");

    //    for (int i = 0; i < enemyCount; i++)
    //    {
    //        Vector2 spawnPos = (Vector2)transform.position + GetRandomPosition();
    //        GameObject newAlien = Instantiate(alienPrefab, spawnPos, Quaternion.identity);

    //        alienList.Add(newAlien);
    //        newAlien.transform.parent = alienContainer.transform;
    //        newAlien.GetComponent<EnemyController>().playerObj = playerObj;

    //        alienList.Add(newAlien);
    //    }
    //}

    [System.Serializable]
    public class EnemyWaveEntry
    {
        public GameObject prefab;  // Enemy prefab
        public int count;          // How many to spawn
    }
    [System.Serializable]
    public class EnemyWave
    {
        public List<EnemyWaveEntry> enemies;  // Multiple enemy types per wave
    }

    [Header("Spawn Bounds")]
    public Vector2 boundMin;
    public Vector2 boundMax;

    [Header("Player Settings")]
    public Transform player;
    public float playerSafeRadius = 5f;

    [Header("Wave Settings")]
    public List<EnemyWave> waves;
    public static int currentWaveIndex = 0;

    [Header("Spawn Timing")]
    public float spawnDelay = 0.1f;

    private bool spawningWave = false;

    void Start()
    {
        if (waves.Count > 0)
            StartCoroutine(SpawnWaveCoroutine(waves[currentWaveIndex]));
    }

    void Update()
    {
        // Progress to next wave if all enemies are dead
        if (!spawningWave && currentWaveIndex < waves.Count)
        {
            if (GameObject.FindGameObjectsWithTag("Enemy").Length == 0)
            {
                currentWaveIndex++;
                if (currentWaveIndex < waves.Count)
                {
                    StartCoroutine(SpawnWaveCoroutine(waves[currentWaveIndex]));
                }
            }
        }
        if (currentWaveIndex >= waves.Count)
        {
            // All waves completed
            transform.Find("Win")?.gameObject.SetActive(true);

        }
        else
        {
            transform.Find("Win")?.gameObject.SetActive(false);

        }
    }

    private IEnumerator SpawnWaveCoroutine(EnemyWave wave)
    {
        spawningWave = true;

        foreach (var entry in wave.enemies)
        {
            for (int i = 0; i < entry.count; i++)
            {
                Vector2 spawnPos = GetRandomSpawnPosition();
                GameObject spawn = Instantiate(entry.prefab, spawnPos, Quaternion.identity);
                spawn.GetComponent<EnemyController>().playerObj = player.gameObject;
                yield return new WaitForSeconds(spawnDelay);
            }
        }

        spawningWave = false;
    }

    private Vector2 GetRandomSpawnPosition()
    {
        Vector2 pos;
        int attempts = 0;

        do
        {
            float x = Random.Range(boundMin.x, boundMax.x);
            float y = Random.Range(boundMin.y, boundMax.y);
            pos = new Vector2(x, y);
            attempts++;
        }
        while (Vector2.Distance(pos, player.position) < playerSafeRadius && attempts < 20);

        return pos;
    }

    // Optional: visualize spawn bounds in Scene view
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector3 center = (Vector3)(boundMin + boundMax) * 0.5f;
        Vector3 size = (Vector3)(boundMax - boundMin);
        Gizmos.DrawWireCube(center, size);
    }
}
