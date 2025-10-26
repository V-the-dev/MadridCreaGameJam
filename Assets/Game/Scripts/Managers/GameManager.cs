using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private MessageManager messageManager = null;
    [SerializeField] private bool defaultStart = true;

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
    }

    public void DebugMessage()
    {
        Debug.Log("Prueba");
    }

    private void Update()
    {
        
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;

        if (playerInput != null)
        {
            Debug.Log("disable");
            playerInput.actions.FindActionMap("Player")?.Disable();
        }
        SoundManager.ToggleFilters();

    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;

        if (playerInput != null)
        {
            Debug.Log("enable");
            playerInput.actions.FindActionMap("Player")?.Enable();
        }
        SoundManager.ToggleFilters();
    }
}
