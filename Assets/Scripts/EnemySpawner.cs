using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("What to spawn")]
    public GameObject[] enemyPrefabs;

    [Header("Where")]
    public Transform[] spawnPoints;
    public bool avoidRepeatingPoint = true;

    [Header("Wave timing")]
    public float firstWaveDelay = 3f;
    public float timeBetweenWaves = 15f;
    public int enemiesPerWave = 2;
    public float timeBetweenSpawns = 0.5f;
    public int enemiesAddedPerWave = 1;

    [Header("Limits")]
    public int maxAlive = 15;
    public bool spawnOnStart = true;

    private int waveNumber;
    private int lastSpawnIndex = -1;
    private readonly List<GameObject> alive = new List<GameObject>();

    void Start() {
        if (enemyPrefabs == null || enemyPrefabs.Length == 0) {
            Debug.LogError($"{name}: no enemy prefabs assigned. Nothing will spawn.");
            return;
        }

        if (spawnPoints == null || spawnPoints.Length == 0) {
            Debug.LogError($"{name}: no spawn points assigned. Nothing will spawn.");
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

            if (maxAlive > 0 && alive.Count >= maxAlive) {
                Debug.Log($"Max alive ({maxAlive}) reached, skipping rest of wave {waveNumber}.");
                yield break;
            }

            SpawnOne();
            yield return new WaitForSeconds(timeBetweenSpawns);
        }
    }

    void SpawnOne() {
        Transform point = PickSpawnPoint();
        if (point == null) {
            Debug.LogWarning($"{name}: no usable spawn point found, skipping.");
            return;
        }

        GameObject prefab = PickPrefab();
        if (prefab == null) {
            Debug.LogWarning($"{name}: an enemy prefab slot is empty, skipping.");
            return;
        }

        GameObject e = Instantiate(prefab, point.position, point.rotation);
        e.name = $"{prefab.name}_w{waveNumber}";
        alive.Add(e);
    }

    Transform PickSpawnPoint() {
        if (spawnPoints.Length == 1) return spawnPoints[0];

        for (int attempt = 0; attempt < 10; attempt++) {
            int index = Random.Range(0, spawnPoints.Length);

            if (avoidRepeatingPoint && index == lastSpawnIndex) continue;
            if (spawnPoints[index] == null) continue;

            lastSpawnIndex = index;
            return spawnPoints[index];
        }

        foreach (Transform t in spawnPoints)
            if (t != null) return t;

        return null;
    }

    GameObject PickPrefab() {
        for (int attempt = 0; attempt < 10; attempt++) {
            GameObject p = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
            if (p != null) return p;
        }
        return null;
    }

    public void SpawnWaveNow() {
        StartCoroutine(SpawnWave());
    }
}