using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("What to spawn")]
    public GameObject enemyPrefab;

    [Header("Where")]
    public Transform[] spawnPoints;

    [Header("Wave timing")]
    public float firstWaveDelay = 3f;
    public float timeBetweenWaves = 15f;
    public int enemiesPerWave = 2;
    public float timeBetweenSpawns = 0.5f;
    public int enemiesAddedPerWave = 1;

    [Header("Limits")]
    public int maxAlive = 8;
    public bool spawnOnStart = true;

    private int nextSpawnIndex;
    private int waveNumber;
    private readonly List<GameObject> alive = new List<GameObject>();

    void Start() {
        if (enemyPrefab == null) {
            Debug.LogError($"{name}: Enemy Prefab is not assigned. Nothing will spawn.");
            return;
        }

        if (spawnPoints == null || spawnPoints.Length == 0) {
            Debug.LogError($"{name}: No spawn points assigned. Nothing will spawn.");
            return;
        }

        if (spawnOnStart) StartCoroutine(RunWaves());
    }

    IEnumerator RunWaves() {
        yield return new WaitForSeconds(firstWaveDelay);

        while (true) {
            yield return StartCoroutine(SpawnWave());
            yield return new WaitForSeconds(timeBetweenWaves);
        }
    }

    IEnumerator SpawnWave() {
        waveNumber++;
        int count = enemiesPerWave + (waveNumber - 1) * enemiesAddedPerWave;
        Debug.Log($"Wave {waveNumber}: spawning {count}");

        for (int i = 0; i < count; i++) {
            alive.RemoveAll(e => e == null);

            if (alive.Count >= maxAlive) {
                Debug.Log($"Max alive ({maxAlive}) reached, skipping rest of wave.");
                yield break;
            }

            SpawnOne();
            yield return new WaitForSeconds(timeBetweenSpawns);
        }
    }

    void SpawnOne() {
        Transform point = spawnPoints[nextSpawnIndex];
        nextSpawnIndex = (nextSpawnIndex + 1) % spawnPoints.Length;

        if (point == null) {
            Debug.LogWarning($"{name}: a spawn point slot is empty, skipping.");
            return;
        }

        GameObject e = Instantiate(enemyPrefab, point.position, point.rotation);
        e.name = $"{enemyPrefab.name}_w{waveNumber}";
        alive.Add(e);
    }

    public void SpawnWaveNow() {
        StartCoroutine(SpawnWave());
    }
}