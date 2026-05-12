using UnityEngine;
using TMPro;
public class DoodleSpawner : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] private GameObject minionPrefab;

    [SerializeField] private TMP_Text doodleCounterText;
    private int doodlesSpawned = 0;

    private void Awake()
    {
        if (player == null)
            player = GetComponent<PlayerController>();

        UpdateCounterUI();
    }

    private void Update()
    {
        if (player.InkCount >= player.maxInkCount)
        {
            SpawnMinion();

            player.ModifyInkCount(-player.maxInkCount);
        }
    }

    private void SpawnMinion()
    {
        Vector2 randomOffset = Random.insideUnitCircle * 2f;

        Vector3 spawnPosition = transform.position + (Vector3)randomOffset;

        Instantiate(minionPrefab, spawnPosition, Quaternion.identity);

        doodlesSpawned++;

        UpdateCounterUI();
    }

    private void UpdateCounterUI()
    {
        if (doodleCounterText != null)
        {
            doodleCounterText.text = doodlesSpawned.ToString();
        }
    }
}