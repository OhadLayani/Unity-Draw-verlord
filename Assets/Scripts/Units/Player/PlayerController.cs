using UnityEngine;

public class PlayerController : UnitBase
{
    public int InkCount { get; private set; }
    private int maxInkCount = 10;

    private Rigidbody2D rb;
    private Vector2 input;

   
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");

        input.Normalize();
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = input * Speed;
    }

    public void ModifyInkCount(int inkDelta)
    {
        InkCount = Mathf.Clamp(InkCount + inkDelta, 0, maxInkCount);
    }
}
