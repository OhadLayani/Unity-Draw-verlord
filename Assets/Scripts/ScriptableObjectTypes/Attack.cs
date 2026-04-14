using UnityEngine;

[CreateAssetMenu(fileName = "Attack", menuName = "Scriptable Objects/Attack")]
public class Attack : ScriptableObject
{
    public Collider2D Collider;
    public Sprite Sprite;

    public float BaseDamage = 1f;
}
