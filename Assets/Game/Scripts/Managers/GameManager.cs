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
    [SerializeField] private GameObject rainEffect = null;
    [SerializeField] private GameObject dawnEffect = null;
    [SerializeField] private GameObject postProDeath = null;
    [SerializeField] private GameObject postProFaint = null;
    [SerializeField] private GameObject postProFadeOut = null;
    [SerializeField] private GameObject videoVictory = null;
    [SerializeField] private GameObject videoVictoryTexture = null;
    [SerializeField] private WatchUI watch = null;

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

        if (dawnEffect != null)
        {
            dawnEffect.SetActive(true);
        }

        if (rainEffect != null)
        {
            rainEffect.SetActive(false);
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
            Debug.Log("disable");
            playerInput.SwitchCurrentActionMap("UI");
            SoundManager.ToggleFilters(true);
        }


    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;

        if (playerInput != null)
        {
            Debug.Log("enable");
            playerInput.SwitchCurrentActionMap("Player");
            SoundManager.ToggleFilters(false);
        }

    }

    public void ActivateDawnEffect()
    {
        if (dawnEffect != null)
        {
            dawnEffect.SetActive(true);
        }
    }
    
    public void ActivateRainEffect()
    {
        if (rainEffect != null)
        {
            rainEffect.SetActive(true);
        }
    }
    
    public void DeactivateRainEffect()
    {
        if (rainEffect != null)
        {
            rainEffect.GetComponent<Animator>().SetTrigger("ExitRain");
        }
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

    public void RestartLoopInfo()
    {
        if (postProDeath != null)
        {
            postProDeath.SetActive(false);
        }

        if (postProFaint != null)
        {
            postProFaint.SetActive(false);
        }

        if (dawnEffect != null)
        {
            dawnEffect.SetActive(true);
        }

        if (rainEffect != null)
        {
            rainEffect.SetActive(false);
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

        if (watch != null)
        {
            watch.ResetTimer();
            watch.SetVisibility(false);
        }
    }

    public void PlayShootSound()
    {
        SoundManager.PlaySound(SoundType.SHOT);
    }

    public void PlayPayMoney()
    {
        SoundManager.PlaySound(SoundType.MONEYPAY);
    }

    public void PlayEarnMoney()
    {
        SoundManager.PlaySound(SoundType.MONEYEARN);
    }

    public void PlayOpenDoor()
    {
        SoundManager.PlaySound(SoundType.FENCEOPEN);
    }

    public void PlayLicorBottle()
    {
        SoundManager.PlaySound(SoundType.GLASSBOTTLE);
    }

    public void PlayKnockKnock()
    {
        SoundManager.PlaySound(SoundType.KNOCKLOUD);
    }

    public void PlayDogBark()
    {
        SoundManager.PlaySound(SoundType.DOGBARK);
    }

    public void PlayDogGrowl()
    {
        SoundManager.PlaySound(SoundType.DOGGROWL);
    }

    public void PlayCasino()
    {
        SoundManager.PlaySound(SoundType.CASINO);
    }
}
