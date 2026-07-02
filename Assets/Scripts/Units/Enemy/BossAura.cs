using System.Collections.Generic;
using UnityEngine;

public class BossAura : MonoBehaviour
{
    [SerializeField] private float radius = 10f;
    [SerializeField] private float speedMultiplier = 1.5f;
    [SerializeField] private float damageMultiplier = 1.5f;
    [SerializeField] private float checkInterval = 0.5f;

    private readonly HashSet<UnitBase> superchargedEnemies = new();
    private float timer;

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer > 0f) return;
        timer = checkInterval;

        RefreshAura();
    }

    private void RefreshAura()
    {
        HashSet<UnitBase> inRange = new();

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);
        foreach (Collider2D hit in hits)
        {
            UnitBase unit = hit.GetComponentInParent<UnitBase>();
            if (unit == null || unit.IsFriendly || unit.gameObject == gameObject) continue;
            inRange.Add(unit);
        }

        foreach (UnitBase unit in inRange)
        {
            if (!superchargedEnemies.Contains(unit))
            {
                unit.Supercharge(speedMultiplier, damageMultiplier);
                superchargedEnemies.Add(unit);
            }
        }

        superchargedEnemies.RemoveWhere(unit =>
        {
            if (unit == null || !inRange.Contains(unit))
            {
                unit?.RemoveSupercharge();
                return true;
            }
            return false;
        });
    }

    private void OnDestroy()
    {
        foreach (UnitBase unit in superchargedEnemies)
            unit?.RemoveSupercharge();
        superchargedEnemies.Clear();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.3f, 0f, 0.3f);
        Gizmos.DrawSphere(transform.position, radius);
    }
}
