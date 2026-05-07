    using UnityEngine;
    using System;

public class PlayerController : UnitBase
    {
    public static event Action<Vector2> OnDoodleChargeCommand;

    public int InkCount { get; private set; }
        private int maxInkCount = 10;

        private Rigidbody2D rb;
        private Vector2 input;

   
        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            InkCount = 0;
        }

        void Update()
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            input.Normalize();
        if (Input.GetMouseButtonDown(1)) // Right click
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
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
