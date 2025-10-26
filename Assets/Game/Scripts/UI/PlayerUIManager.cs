using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerUIManager : MonoBehaviour
{
    [SerializeField] private SceneTransitions sceneTransitioner;
    [SerializeField] private GameObject pausePanel;

    [SerializeField] private PlayerInput playerInput;

    private GameObject activePanel;
    private InputAction pauseAction;

    private void Awake()
    {
        if (sceneTransitioner == null)
            sceneTransitioner = GetComponent<SceneTransitions>();

        if (playerInput == null)
            playerInput = GetComponent<PlayerInput>();
    }

    private void OnEnable()
    {
        pauseAction = playerInput.actions.FindActionMap("UI")?.FindAction("PauseUI");
        if (pauseAction != null)
        {
            pauseAction.performed += OnPausePressed;
            pauseAction.Enable();
        }
    }

    private void OnDisable()
    {
        if (pauseAction != null)
            pauseAction.performed -= OnPausePressed;
    }

    private void OnPausePressed(InputAction.CallbackContext context)
    {
        TogglePause();
    }

    public void TogglePause()
    {
        if (activePanel == null)
        {
            TogglePanel(pausePanel);
            GameManager.Instance.PauseGame();
        }
        else
        {
            TogglePanel(activePanel);
            GameManager.Instance.ResumeGame();
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
            sceneTransitioner.LoadScene("MainMenu");
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
