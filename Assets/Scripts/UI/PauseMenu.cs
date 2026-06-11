using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject Containeer;
    public GameObject ControlsMenu;
    void Update()
    {
       if (Input.GetKeyDown(KeyCode.Escape))
        {
            Containeer.SetActive(true);
            Time.timeScale = 0;
        }
    }

    public void ResumeButton()
    {
        Containeer.SetActive(false);
        ControlsMenu.SetActive(false);
        Time.timeScale = 1;
    }

    public void ControlsButton ()
    {
        Containeer.SetActive(false);
        ControlsMenu.SetActive(true);
        Time.timeScale = 0;
    }

    public void BackButton ()
    {
        Containeer.SetActive(true);
        ControlsMenu.SetActive(false);
    }

    public void QuitButton()
    {

    }
}
