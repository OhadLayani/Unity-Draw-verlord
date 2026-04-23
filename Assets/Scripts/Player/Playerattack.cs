using System.Collections;
using UnityEngine;

public class Playerttack : MonoBehaviour
{
    [SerializeField] private Collider2D attackHitbox;
    [SerializeField] private float attackDuration = 0.15f;
    [SerializeField] private float attackCooldown = 0.35f;
    [SerializeField] private Transform playerRoot;

    private bool canAttack = true;
    private Camera mainCam;

    private void Awake()
    {
        mainCam = Camera.main;

        if (playerRoot == null)
            playerRoot = transform.parent;
    }
    private void Start()
    {
        if (attackHitbox != null)
            attackHitbox.enabled = false;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && canAttack)
        {
            StartCoroutine(AttackRoutine());
        }
    }
    private void AimAtMouse()
    {
        Vector3 mouseScreenPos = Input.mousePosition;
        Vector3 mouseWorldPos = mainCam.ScreenToWorldPoint(mouseScreenPos);
        mouseWorldPos.z = 0f;

        Vector2 direction = mouseWorldPos - playerRoot.position;

        if (direction.sqrMagnitude < 0.001f)
            return;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
    private IEnumerator AttackRoutine()
    {
        canAttack = false;
        AimAtMouse(); // rotate only now

        attackHitbox.enabled = true;
        yield return new WaitForSeconds(attackDuration);
        attackHitbox.enabled = false;

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }
}