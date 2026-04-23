using UnityEngine;

public class AttackVisualizer : MonoBehaviour
{
    [SerializeField] private Transform playerRoot;

    private Camera mainCam;

    private void Awake()
    {
        mainCam = Camera.main;

        if (playerRoot == null)
            playerRoot = transform.parent;
    }

    private void Update()
    {
        AimAtMouse();
    }

    private void AimAtMouse()
    {
        Vector3 mouseWorldPos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;

        Vector2 direction = mouseWorldPos - playerRoot.position;

        if (direction.sqrMagnitude < 0.001f)
            return;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}