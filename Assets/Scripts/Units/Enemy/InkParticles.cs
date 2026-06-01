using UnityEngine;

public class InkParticle : MonoBehaviour
{
    private Transform target;
    private Vector2 velocity;
    private float timer;

    [SerializeField] private float hoverDuration = 1f;
    [SerializeField] private float attractSpeed = 6f;

    public void Init(Transform playerTransform)
    {
        target = playerTransform;
        velocity = Random.insideUnitCircle.normalized * Random.Range(2f, 4f);
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer < hoverDuration)
        {
            // burst out then decelerate into a hover
            velocity = Vector2.Lerp(velocity, Vector2.zero, Time.deltaTime * 5f);
            transform.position += (Vector3)velocity * Time.deltaTime;
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, target.position, attractSpeed * Time.deltaTime);
            if (Vector2.Distance(transform.position, target.position) < 0.1f)
                Destroy(gameObject);
        }
    }
}