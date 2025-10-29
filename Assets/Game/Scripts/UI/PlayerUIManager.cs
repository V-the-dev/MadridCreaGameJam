using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerUIManager : MonoBehaviour
{
    [SerializeField] private SceneTransitions sceneTransitioner;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject settingsPanel;

    [SerializeField] private PlayerInput playerInput;

    private InputAction pauseUI;
    private InputAction pausePlayer;

    private GameObject activePanel;

    private bool pausedByMenu = false;

    private void Awake()
    {
        if (sceneTransitioner == null)
            sceneTransitioner = GetComponent<SceneTransitions>();

        if (playerInput == null)
            playerInput = GetComponent<PlayerInput>();

        pauseUI = playerInput.actions.FindActionMap("UI")?.FindAction("PauseUI");
        pausePlayer = playerInput.actions.FindActionMap("Player")?.FindAction("PausePlayer");
    }

    private void OnEnable()
    {
        if (pauseUI != null)
        {
            pauseUI.performed += OnPausePressed;
            pauseUI.Enable();
        }

        if (pausePlayer != null)
        {
            pausePlayer.performed += OnPausePressed;
            pausePlayer.Enable();
        }
    }

    private void OnDisable()
    {
        if (pauseUI != null)
        {
            pauseUI.performed -= OnPausePressed;
        }
            
        if (pausePlayer != null)
        {
            pausePlayer.performed -= OnPausePressed;
        }

    }

    private void OnPausePressed(InputAction.CallbackContext context)
    {
        TogglePause();

    }

    public void TogglePause()
    {
        bool isPaused = GameManager.Instance.isPaused; // asumiendo que GameManager lo expone

        // Solo pausar si el juego no estaba ya pausado por otro sistema
        if (!isPaused)
        {
            GameManager.Instance.PauseGame();
            pausedByMenu = true;
            TogglePanel(pausePanel);
        }
        else 
        {
            if (pausedByMenu)
            {
                GameManager.Instance.ResumeGame();
                pausedByMenu = false;
            }
                
            TogglePanel(pausePanel);
            if(settingsPanel.activeSelf)
                TogglePanel(settingsPanel);
        }

    }

 
    public void TogglePanel(GameObject panel)
    {
        if (panel == null)
        {
            Debug.LogWarning("[PlayerUIManager] TogglePanel was called with a null GameObject.");
            return;
        }

        bool newState = !panel.activeSelf;
        panel.SetActive(newState);
        activePanel = newState ? panel : null;
    }

    public void LoadMainMenu()
    {
        if (sceneTransitioner != null)
        {
            sceneTransitioner.LoadScene("MainMenu");
            TogglePause();
        }   
        else
            Debug.LogWarning("[PlayerUIManager] No SceneTransitions component assigned.");
    }
    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
