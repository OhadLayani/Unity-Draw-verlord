using UnityEngine;
using UnityEngine.LowLevel;
using System;

public class PlayerController : UnitBase
{
    public static event Action<Vector2> OnDoodleChargeCommand;

    public int InkCount { get; private set; }
    public int maxInkCount = 10;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    private Vector2 input;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");

        input.Normalize();

        Camera mainCam = Camera.main;
        Vector3 mouseWorldPos = mainCam.ScreenToWorldPoint(Input.mousePosition);

        // Flip sprite depending on mouse position
        if (mouseWorldPos.x < transform.position.x)
        {
            spriteRenderer.flipX = true;
        }
        else
        {
            spriteRenderer.flipX = false;
        }

        if (Input.GetMouseButton(0))
        {
            TriggerAttack(mouseWorldPos);
        }

        if (Input.GetMouseButtonDown(1))
        {
            Vector2 clickedPosition = new Vector2(mouseWorldPos.x, mouseWorldPos.y);

            OnDoodleChargeCommand?.Invoke(clickedPosition);
        }
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = input * Speed;
    }

    public void ModifyInkCount(int inkDelta)
    {
        InkCount = Mathf.Clamp(InkCount + inkDelta, 0, maxInkCount);
        //Debug.Log("Current count: " + InkCount);
    }
}