using UnityEngine;
using Unity.Cinemachine;
using System.Collections;

public class BossIntro : MonoBehaviour
{
    [SerializeField] private string bossName = "Sharshabil The Horrible!!!";
    [SerializeField] private float introDuration = 3f;
    [SerializeField] private float cameraPanTime = 1f;

    private void Start()
    {
        StartCoroutine(IntroSequence());
    }

    private IEnumerator IntroSequence()
    {
        BossIntroUI ui = BossIntroUI.Instance;

        if (ui == null)
        {
            Debug.LogWarning("No BossIntroUI found in scene, skipping boss intro.");
            yield break;
        }

        if (ui.nameText != null)
            ui.nameText.text = bossName;

        if (ui.bossCamera != null)
        {
            CinemachineCamera cineCam = ui.bossCamera.GetComponent<CinemachineCamera>();
            if (cineCam != null)
            {
                cineCam.Follow = transform;
            }

            if (ui.playerCamera != null)
                ui.playerCamera.SetActive(false);

            ui.bossCamera.SetActive(true);
        }

        if (ui.nameBanner != null)
            ui.nameBanner.SetActive(true);

        // let Cinemachine actually finish panning to the boss before freezing time
        yield return new WaitForSeconds(cameraPanTime);

        Time.timeScale = 0f;

        yield return new WaitForSecondsRealtime(introDuration);

        if (ui.bossCamera != null)
            ui.bossCamera.SetActive(false);

        if (ui.playerCamera != null)
            ui.playerCamera.SetActive(true);

        if (ui.nameBanner != null)
            ui.nameBanner.SetActive(false);

        Time.timeScale = 1f;
    }
}
