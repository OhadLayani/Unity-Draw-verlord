using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [Header("Prefab")]
    public GameObject enemyPrefab;

    [Header("Wave Settings")]
    public int maxEnemies = 5;
    public float groupDelay = 5f;
    [Header("Pattern Odds")]
    public float basePatternChance = 0.1f;
    public float patternChanceIncrement = 0.15f;

    [Header("Spawn Delay")]
    public float spawnDelay = 3f;
    public float minSpawnDelay = 0.5f;
    public float spawnDelayReduction = 0.15f;

    [Header("Pattern")]
    public float rapidSpawnDelay = 0.1f;
    public float spawnOffset = 2f;

    private Camera mainCamera;
    private int waveCount = 0;
    private int enemiesSpawnedThisWave = 0;
    private int wavesSinceLastPattern = 0;

    private SpawnerState currentState;
    private SpawningState spawningState;
    private WaveBreakState waveBreakState;
    private PatternSpawningState patternSpawningState;

    private void Start()
    {
        mainCamera = Camera.main;

        spawningState = new SpawningState(this);
        waveBreakState = new WaveBreakState(this);
        patternSpawningState = new PatternSpawningState(this);

        ChangeState(spawningState);
    }

    private void Update()
    {
        if (enemyPrefab == null || mainCamera == null) return;
        currentState?.Tick();
    }

    private void ChangeState(SpawnerState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }

    private void OnWaveComplete()
    {
        waveCount++;
        spawnDelay = Mathf.Max(minSpawnDelay, spawnDelay - spawnDelayReduction);
        maxEnemies++;
        ChangeState(waveBreakState);
    }

    private void OnWaveBreakComplete()
    {
        enemiesSpawnedThisWave = 0;

        float chance = Mathf.Clamp01(basePatternChance + wavesSinceLastPattern * patternChanceIncrement);
        bool isPatternWave = waveCount > 0 && Random.value < chance;

        if (isPatternWave)
        {
            wavesSinceLastPattern = 0;
            ChangeState(patternSpawningState);
        }
        else
        {
            wavesSinceLastPattern++;
            ChangeState(spawningState);
        }
    }

    private void SpawnAt(Vector3 position)
    {
        Instantiate(enemyPrefab, position, Quaternion.identity, transform);
    }

    private Vector3 GetEdgePosition(int side, float t)
    {
        float camHeight = 2f * mainCamera.orthographicSize;
        float camWidth = camHeight * mainCamera.aspect;
        Vector3 cam = mainCamera.transform.position;

        float left   = cam.x - camWidth  / 2f;
        float right  = cam.x + camWidth  / 2f;
        float bottom = cam.y - camHeight / 2f;
        float top    = cam.y + camHeight / 2f;

        return side switch
        {
            0 => new Vector3(left  - spawnOffset, Mathf.Lerp(bottom, top,   t), 0f),
            1 => new Vector3(right + spawnOffset, Mathf.Lerp(bottom, top,   t), 0f),
            2 => new Vector3(Mathf.Lerp(left, right, t), top    + spawnOffset, 0f),
            _ => new Vector3(Mathf.Lerp(left, right, t), bottom - spawnOffset, 0f),
        };
    }

    private Vector3 GetRandomSpawnPosition()
    {
        return GetEdgePosition(Random.Range(0, 4), Random.value);
    }

    // -------------------------------------------------------------------------
    // States
    // -------------------------------------------------------------------------

    private abstract class SpawnerState
    {
        protected EnemySpawner spawner;
        protected SpawnerState(EnemySpawner spawner) { this.spawner = spawner; }
        public virtual void Enter() { }
        public virtual void Tick()  { }
        public virtual void Exit()  { }
    }

    private class SpawningState : SpawnerState
    {
        private float timer;
        public SpawningState(EnemySpawner spawner) : base(spawner) { }

        public override void Enter() => timer = spawner.spawnDelay;

        public override void Tick()
        {
            timer -= Time.deltaTime;
            if (timer > 0f) return;

            spawner.SpawnAt(spawner.GetRandomSpawnPosition());
            spawner.enemiesSpawnedThisWave++;
            timer = spawner.spawnDelay;

            if (spawner.enemiesSpawnedThisWave >= spawner.maxEnemies)
                spawner.OnWaveComplete();
        }
    }

    private class WaveBreakState : SpawnerState
    {
        private float timer;
        public WaveBreakState(EnemySpawner spawner) : base(spawner) { }

        public override void Enter() => timer = spawner.groupDelay;

        public override void Tick()
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
                spawner.OnWaveBreakComplete();
        }
    }

    private class PatternSpawningState : SpawnerState
    {
        private enum Pattern { CONCENTRATED, LINE }

        private readonly List<Vector3> spawnPositions = new List<Vector3>();
        private float timer;
        private int index;

        public PatternSpawningState(EnemySpawner spawner) : base(spawner) { }

        public override void Enter()
        {
            spawnPositions.Clear();
            index = 0;
            timer = 0f;

            int side = Random.Range(0, 4);
            Pattern pattern = (Pattern)Random.Range(0, 2);

            Debug.Log($"Pattern wave: {pattern} from side {side}");

            if (pattern == Pattern.CONCENTRATED)
                BuildConcentrated(side);
            else
                BuildLine(side);
        }

        public override void Tick()
        {
            timer -= Time.deltaTime;
            if (timer > 0f) return;

            spawner.SpawnAt(spawnPositions[index]);
            index++;
            timer = spawner.rapidSpawnDelay;

            if (index >= spawnPositions.Count)
                spawner.OnWaveComplete();
        }

        private void BuildConcentrated(int side)
        {
            for (int i = 0; i < spawner.maxEnemies; i++)
            {
                float t = 0.5f + Random.Range(-0.15f, 0.15f);
                spawnPositions.Add(spawner.GetEdgePosition(side, t));
            }
        }

        private void BuildLine(int side)
        {
            int count = spawner.maxEnemies;
            for (int i = 0; i < count; i++)
            {
                float t = (float)i / (count - 1);
                spawnPositions.Add(spawner.GetEdgePosition(side, t));
            }
        }
    }
}
