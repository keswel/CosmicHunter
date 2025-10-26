using UnityEngine;
using TMPro;

public class WaveTrackScript : MonoBehaviour
{
    public TextMeshProUGUI text;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        string waveNumer = (EnemySpawnerController.currentWaveIndex).ToString();
        text.text = "[" + waveNumer +"/15]";
    }

    // Update is called once per frame
    void Update()
    {
        int waveNumer = EnemySpawnerController.currentWaveIndex;
        text.text = "["+waveNumer + "/15]";
    }
}
