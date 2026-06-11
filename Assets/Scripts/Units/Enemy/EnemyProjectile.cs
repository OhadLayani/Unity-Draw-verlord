using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [SerializeField] private float speed = 6f;
    [SerializeField] private float lifetime = 5f;
    private float damage;
    [SerializeField] private float scale = 0.3f;
    [SerializeField] private bool flipSprite = false;

    private Vector2 direction;

    public void Init(Vector2 direction, UnitBase source)
    {
        this.direction = direction.normalized;
        this.damage = source.Damage;

        float angle = Mathf.Atan2(this.direction.y, this.direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        float flip = flipSprite ? -1f : 1f;
        transform.localScale = new Vector3(scale * flip, scale, 1f);

        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Hurtbox>(out var hurtbox))
        {
            if (hurtbox.HurtboxIsFriendly)
            {
                hurtbox.TriggerDamageTaken(damage);
                Destroy(gameObject);
            }
        }
    }
}
