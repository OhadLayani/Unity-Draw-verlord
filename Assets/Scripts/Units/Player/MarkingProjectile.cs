using UnityEngine;

public class MarkingProjectile : MonoBehaviour
{
    [SerializeField] private float speed = 8f;
    [SerializeField] private float scale = 0.3f;
    [SerializeField] private bool flipSprite = false;

    private Hurtbox target;
    private PlayerController owner;

    public void Init(Hurtbox target, PlayerController owner)
    {
        this.target = target;
        this.owner = owner;

        Vector2 direction = (target.transform.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        float flip = flipSprite ? -1f : 1f;
        transform.localScale = new Vector3(scale * flip, scale, 1f);
    }

    private void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        transform.position = Vector2.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, target.transform.position) < 0.1f)
        {
            owner.MarkEnemy(target);
            Destroy(gameObject);
        }
    }
}
