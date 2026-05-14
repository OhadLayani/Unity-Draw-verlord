using UnityEngine;
using System;

public class PlayerController : UnitBase
{
    public static event Action<Vector2> OnDoodleChargeCommand;

    [Header("Ink")]
    public int InkCount { get; private set; }
    public int maxInkCount = 10;

    [Header("UI")]

    private Rigidbody2D rb;
    [SerializeField] private Transform graphicsTransform;
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
    }
}