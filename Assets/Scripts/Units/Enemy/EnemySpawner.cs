using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public int maxEnemies = 5;
    public float spawnDelay = 3f;
    public float spawnOffset = 2f;

    [SerializeField] private float groupDelay = 5f;

    private float timer;
    private Camera mainCamera;

    private int enemiesSpawnedThisWave = 0;
    private bool waitingForNextWave = false;

    void Start()
    {
        mainCamera = Camera.main;
        timer = spawnDelay;
    }

    void Update()
    {
        if (enemyPrefab == null || mainCamera == null)
        {
            Debug.LogError("camera or enemyPrefab not found");
            return;
        }

        timer -= Time.deltaTime;

        if (timer > 0f)
            return;

        if (waitingForNextWave)
        {
            maxEnemies++;
            enemiesSpawnedThisWave = 0;
            waitingForNextWave = false;

            timer = spawnDelay;
            return;
        }

        SpawnEnemy();

        enemiesSpawnedThisWave++;

        if (enemiesSpawnedThisWave >= maxEnemies)
        {
            waitingForNextWave = true;
            timer = groupDelay;
        }
        else
        {
            timer = spawnDelay;
        }
    }

    void SpawnEnemy()
    {
        Debug.Log("spawned enemy");

        Vector3 spawnPosition = GetSpawnPositionOutsideCamera();
        Instantiate(enemyPrefab, spawnPosition, Quaternion.identity, transform);
    }

    Vector3 GetSpawnPositionOutsideCamera()
    {
        float camHeight = 2f * mainCamera.orthographicSize;
        float camWidth = camHeight * mainCamera.aspect;
        Vector3 camPos = mainCamera.transform.position;

        int side = Random.Range(0, 4);

        float x = 0f;
        float y = 0f;

        switch (side)
        {
            case 0:
                x = camPos.x - camWidth / 2f - spawnOffset;
                y = Random.Range(camPos.y - camHeight / 2f, camPos.y + camHeight / 2f);
                break;

            case 1:
                x = camPos.x + camWidth / 2f + spawnOffset;
                y = Random.Range(camPos.y - camHeight / 2f, camPos.y + camHeight / 2f);
                break;

            case 2:
                x = Random.Range(camPos.x - camWidth / 2f, camPos.x + camWidth / 2f);
                y = camPos.y + camHeight / 2f + spawnOffset;
                break;

            case 3:
                x = Random.Range(camPos.x - camWidth / 2f, camPos.x + camWidth / 2f);
                y = camPos.y - camHeight / 2f - spawnOffset;
                break;
        }

        return new Vector3(x, y, 0f);
    }
}