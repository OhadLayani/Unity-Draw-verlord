using System.Collections;
using UnityEngine;

/// <summary>
/// A single ink orb that spawns on enemy death, waits a moment, then flies to the player.
/// Each orb grants 1 ink on arrival.
/// The number of orbs spawned equals the enemy's InkValue.
/// </summary>
public class InkOrb : MonoBehaviour
{
    private Transform target;
    private PlayerController player;
    private float moveSpeed = 6f;
    private bool homing = false;

    private const float MAX_LIFETIME = 8f;

    public void Init(PlayerController playerController, float delay)
    {
        player = playerController;
        target = playerController.transform;
        StartCoroutine(DelayThenHome(delay));
        Destroy(gameObject, MAX_LIFETIME);
    }

    private IEnumerator DelayThenHome(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        homing = true;
    }

    private void Update()
    {
        if (!homing) return;

        // Target lost (player died) — clean up
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.unscaledDeltaTime);

        if (Vector3.Distance(transform.position, target.position) < 0.3f)
        {
            player.ModifyInkCount(1);
            Destroy(gameObject);
        }
    }
}
