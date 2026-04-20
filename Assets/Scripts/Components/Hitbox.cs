using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public float HitDamage { get; private set; }

    private Collider2D col;

    private void Awake()
    {
        col = GetComponentInParent<Collider2D>();

        HitDamage = 5f; //TEMPORARY FIXME for now this is the damage but in the future include attack damage calculations that take into account the attacker's attack stat
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
