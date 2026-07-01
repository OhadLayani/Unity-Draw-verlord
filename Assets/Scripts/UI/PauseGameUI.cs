using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseGameUI : MonoBehaviour
{
    public GameObject Container;
    public ControlsMenuPanelUI ControlPanelUI; // Reference to ControlPanelUI
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Container.SetActive(true);
            Time.timeScale = 0;
        }
    }


    public void ResumeButton()
    {
        Container.SetActive(false);
        Time.timeScale = 1;
    }

    public void ControlsButoon()
    {
        Container.SetActive(false); // hiding the Pause Menu 
        ControlPanelUI.OpenControlPanel(); // Open the control panel
    }

    public void QuitButton()
    {
        SceneManager.LoadScene("Main Menu");
    }
}
