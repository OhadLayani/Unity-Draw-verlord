using UnityEngine;

public class Hitbox : MonoBehaviour
{
    [SerializeField]
    private UnitBase attackSource;

    public bool HitboxIsFriendly { get; private set; }
    public float HitDamage { get; private set; }

    private Collider2D col;

    private void Start()
    {
        if (attackSource == null)
        {
            Debug.LogError("No attack source Unit component asssigned on hitbox, falling back on default value");
            HitDamage = 5f;
            HitboxIsFriendly = false;
            return;
        }

        col = GetComponentInParent<Collider2D>();

        HitDamage = attackSource.Damage;
        HitboxIsFriendly = attackSource.IsFriendly;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"Hitbox touched: {other.gameObject.name}");
        Debug.Log($"Detected parent: {(other.transform.parent != null ? other.transform.parent.gameObject.name : "No parent")}");
        if (other.TryGetComponent<Hurtbox>(out var hurtbox))
        {
            Debug.Log("Touched hurtbox");

            Debug.Log($"Hitbox object: {gameObject.name}");
            Debug.Log($"Attack source object: {attackSource.gameObject.name}");
            Debug.Log($"Attack source live IsFriendly: {attackSource.IsFriendly}");
            Debug.Log($"Cached HitboxIsFriendly: {HitboxIsFriendly}");
            Debug.Log($"Hurtbox object: {hurtbox.gameObject.name}");
            Debug.Log($"Cached HurtboxIsFriendly: {hurtbox.HurtboxIsFriendly}");

            if (HitboxIsFriendly == hurtbox.HurtboxIsFriendly)
            {
                Debug.Log("Same team, ignoring");
                return;
            }

            Debug.Log("Damage applied");
            hurtbox.TriggerDamageTaken(HitDamage);
        }
    }

    private void OnDrawGizmos() //AI generated function to see if it visualizes hitbox
    {
        if (col == null)
            col = GetComponent<Collider2D>();

        if (col == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.matrix = transform.localToWorldMatrix;

        // BoxCollider2D
        if (col is BoxCollider2D box)
        {
            Gizmos.DrawWireCube(box.offset, box.size);
        }

        // CircleCollider2D
        else if (col is CircleCollider2D circle)
        {
            Gizmos.DrawWireSphere(circle.offset, circle.radius);
        }

        // CapsuleCollider2D
        else if (col is CapsuleCollider2D capsule)
        {
            Vector2 size = capsule.size;
            Gizmos.DrawWireCube(capsule.offset, size);
        }

        // PolygonCollider2D
        else if (col is PolygonCollider2D poly)
        {
            for (int p = 0; p < poly.pathCount; p++)
            {
                Vector2[] path = poly.GetPath(p);
                for (int i = 0; i < path.Length; i++)
                {
                    Vector2 a = path[i];
                    Vector2 b = path[(i + 1) % path.Length];
                    Gizmos.DrawLine(a, b);
                }
            }
        }
    }
}
