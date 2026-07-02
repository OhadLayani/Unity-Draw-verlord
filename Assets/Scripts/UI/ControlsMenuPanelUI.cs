using UnityEngine;

public class ControlsMenuPanelUI : MonoBehaviour
{


    public GameObject controlPanel; // Reference to the Control Panel
    public GameObject pauseMenuContainer;
    public void OpenControlPanel()
    {
        pauseMenuContainer.SetActive(false);
        controlPanel.SetActive(true);
        
    }

    public void CloseControlPanel()
    {
        controlPanel.SetActive(false);
        pauseMenuContainer.SetActive(true);
      
    }

}
