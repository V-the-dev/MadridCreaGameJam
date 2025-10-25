using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerUIManager : MonoBehaviour
{
    private SceneTransitions sceneTransitioner;

    public GameObject pausePanel;

    private GameObject activePanel;
    
    public PlayerInput playerInput;
    
    private void Start()
    {
        sceneTransitioner = GetComponent<SceneTransitions>();
    }

    private void Update()
    {
        if (pausePanel.activeInHierarchy)
            GameManager.Instance.PauseGame();
        else
            GameManager.Instance.ResumeGame();
        
        if (playerInput.actions["OpenPauseMenu"].WasPressedThisFrame())
        {
            if(activePanel == null)
                TogglePanel(pausePanel);
            else
            {
                TogglePanel(activePanel);
                activePanel = pausePanel;
            }
        }
    }

    public void LoadMainMenu()
    {
        sceneTransitioner.LoadScene("MainMenu");
    }
    
    public void TogglePanel(GameObject panel)
    {
        if (panel == null)
        {
            Debug.LogWarning("TogglePanel was called with a null GameObject.");
            return;
        }
        panel.SetActive(!panel.activeSelf);
        
        activePanel = panel;

        //EventSystem.current.firstSelectedGameObject = panel;
    }
    
    public void ExitGame()
    {
        Application.Quit();
    }
}
