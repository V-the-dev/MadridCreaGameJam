using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private MessageManager messageManager = null;
    [SerializeField] private bool defaultStart = true;
    [SerializeField] private GameObject postProDeath = null;
    [SerializeField] private GameObject postProFaint = null;
    [SerializeField] private GameObject postProFadeOut = null;
    [SerializeField] private GameObject videoVictory = null;
    [SerializeField] private GameObject videoVictoryTexture = null;

    private bool isEndingGame = false;

    private PlayerInput playerInput;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        playerInput = FindAnyObjectByType<UnityEngine.InputSystem.PlayerInput>();
    }

    private void Start()
    {
        if(defaultStart)
            messageManager.Interactuar();
        if (postProDeath != null)
        {
            postProDeath.SetActive(false);
        }

        if (postProFaint != null)
        {
            postProFaint.SetActive(false);
        }

        if (postProFadeOut != null)
        {
            postProFadeOut.SetActive(false);
        }

        if (videoVictory != null)
        {
            videoVictory.SetActive(false);
            videoVictoryTexture.SetActive(false);
        }
    }

    //private void Update()
    //{
    //    Debug.Log($"Active map: {playerInput.currentActionMap?.name}");
    //}

    public void PauseGame()
    {
        Time.timeScale = 0f;

        if (playerInput != null)
        {
            //Debug.Log("disable");
            playerInput.SwitchCurrentActionMap("UI");
        }
        SoundManager.ToggleFilters();

    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;

        if (playerInput != null)
        {
            //Debug.Log("enable");
            playerInput.SwitchCurrentActionMap("Player");

        }
        SoundManager.ToggleFilters();
    }

    public void ActivateDeathEffect()
    {
        if (postProDeath != null)
        {
            postProDeath.SetActive(true);
        }
    }

    public void ActivateFaintEffect()
    {
        if (postProFaint != null)
        {
            postProFaint.SetActive(true);
        }
    }

    public void ActivateFadeOutEffect()
    {
        if (postProFadeOut != null)
        {
            postProFadeOut.SetActive(true);
        }
    }
    
    [ContextMenu("EndGame")]
    public void EndGame()
    {
        PauseGame();
        isEndingGame = true;
        ActivateFadeOutEffect();
    }

    public void FinishedFadeOut()
    {
        if (isEndingGame)
        {
            if (videoVictory)
            {
                VideoPlayer videoPlayer = videoVictory.GetComponent<VideoPlayer>();
                videoVictory.SetActive(true);
                videoVictoryTexture.SetActive(true);
                videoPlayer.Play();
                videoPlayer.loopPointReached += GoToMainMenu;
                postProFadeOut.SetActive(false);
            }
        }
    }

    private void GoToMainMenu(VideoPlayer vp)
    {
        SceneManager.LoadScene("MainMenu");
    }
}
