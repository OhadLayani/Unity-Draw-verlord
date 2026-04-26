using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public int maxEnemies = 5;
    public float spawnDelay = 3f;
    public float spawnOffset = 2f; // how far outside camera bounds to spawn

    private float timer;
    private Camera mainCamera;

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
            

        int currentEnemies = GameObject.FindGameObjectsWithTag("Enemy").Length;

        if (currentEnemies >= maxEnemies)
            return;

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            SpawnEnemy();
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
            case 0: // Left
                x = camPos.x - camWidth / 2f - spawnOffset;
                y = Random.Range(camPos.y - camHeight / 2f, camPos.y + camHeight / 2f);
                break;

            case 1: // Right
                x = camPos.x + camWidth / 2f + spawnOffset;
                y = Random.Range(camPos.y - camHeight / 2f, camPos.y + camHeight / 2f);
                break;

            case 2: // Top
                x = Random.Range(camPos.x - camWidth / 2f, camPos.x + camWidth / 2f);
                y = camPos.y + camHeight / 2f + spawnOffset;
                break;

            case 3: // Bottom
                x = Random.Range(camPos.x - camWidth / 2f, camPos.x + camWidth / 2f);
                y = camPos.y - camHeight / 2f - spawnOffset;
                break;
        }

        return new Vector3(x, y, 0f);
    }
}