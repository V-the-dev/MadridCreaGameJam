using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerUIManager : MonoBehaviour
{
    private SceneTransitions sceneTransitioner;

    public GameObject pausePanel;

    public PlayerInput playerInput;
    
    private void Start()
    {
        sceneTransitioner = GetComponent<SceneTransitions>();
    }

    private void Update()
    {
        if (playerInput.actions["OpenPauseMenu"].WasPressedThisFrame())
        {
            TogglePanel(pausePanel);
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
    }
    
    public void ExitGame()
    {
        Application.Quit();
    }
}
