using UnityEngine;
using TMPro;

public class BossIntroUI : MonoBehaviour
{
    public static BossIntroUI Instance { get; private set; }

    public GameObject bossCamera;
    public GameObject playerCamera;
    public GameObject nameBanner;
    public TMP_Text nameText;

    private void Awake()
    {
        Instance = this;
    }
}
