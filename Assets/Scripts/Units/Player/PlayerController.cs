using UnityEngine;
using System;

public class PlayerController : UnitBase
{
    public static event Action<Vector2> OnDoodleChargeCommand;
    public static event Action<Hurtbox> OnEnemyMarked;

    [Header("Ink")]
    public int InkCount { get; private set; }
    public int maxInkCount = 10;

    [Header("Marking")]
    [SerializeField] private GameObject markingProjectilePrefab;

    [Header("Bounds")]
    [SerializeField] private Vector2 minBounds;
    [SerializeField] private Vector2 maxBounds;

    [Header("UI")]

    private Rigidbody2D rb;
    [SerializeField] private Transform graphicsTransform;
    private Vector2 input;
    private SpriteRenderer markedEnemyRenderer;
    [SerializeField] private GameOverUI gameOverUI;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        
    }

    void Update()
    {
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");

        input.Normalize();

        Camera mainCam = Camera.main;
        Vector3 mouseWorldPos = mainCam.ScreenToWorldPoint(Input.mousePosition);

        if (mouseWorldPos.x < transform.position.x)
        {
            graphicsTransform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            graphicsTransform.localScale = new Vector3(1, 1, 1);
        }

        if (Input.GetMouseButton(0))
        {
            TriggerAttack(mouseWorldPos);
        }

        if (Input.GetMouseButtonDown(1))
        {
            Vector2 clickedPosition = new Vector2(mouseWorldPos.x, mouseWorldPos.y);
            Hurtbox hurtbox = GetClosestEnemyHurtbox(clickedPosition, 1f);

            if (hurtbox != null)
            {
                Debug.Log($"Projectile fired at: {hurtbox.transform.parent.name}");
                GameObject proj = Instantiate(markingProjectilePrefab, transform.position, Quaternion.identity);
                proj.GetComponent<MarkingProjectile>().Init(hurtbox, this);
            }
            else
            {
                Debug.Log($"Charge command sent to {clickedPosition}");
                OnDoodleChargeCommand?.Invoke(clickedPosition);
            }
        }
    }
    public override void Die()
    {
        base.Die();
        //Game Over Screen.
        if (gameOverUI != null)
        {
            gameOverUI.ShowGameOver();
        }

        gameObject.SetActive(false);
    }
    private void FixedUpdate()
    {
        rb.linearVelocity = input * Speed;

        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, minBounds.x, maxBounds.x);
        pos.y = Mathf.Clamp(pos.y, minBounds.y, maxBounds.y);
        transform.position = pos;
    }

   

    private Hurtbox GetClosestEnemyHurtbox(Vector2 position, float radius)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(position, radius);
        Hurtbox closest = null;
        float closestDist = float.MaxValue;

        foreach (Collider2D hit in hits)
        {
            Hurtbox hb = hit.GetComponent<Hurtbox>();
            if (hb == null || hb.HurtboxIsFriendly) continue;

            float dist = Vector2.Distance(position, hit.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                closest = hb;
            }
        }

        return closest;
    }

    public void MarkEnemy(Hurtbox hurtbox)
    {
        if (markedEnemyRenderer != null)
            markedEnemyRenderer.color = Color.white;

        markedEnemyRenderer = hurtbox.transform.parent.GetComponentInChildren<SpriteRenderer>();
        if (markedEnemyRenderer != null)
            markedEnemyRenderer.color = Color.black;

        Debug.Log($"Enemy marked: {hurtbox.transform.parent.name}");
        OnEnemyMarked?.Invoke(hurtbox);
    }

    public void ModifyInkCount(int inkDelta)
    {
        InkCount = (InkCount + inkDelta);
    }
}