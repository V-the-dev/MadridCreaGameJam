using System;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using Slider = UnityEngine.UI.Slider;

using UnityEngine.EventSystems;

/// <summary>
/// Script que controla el comportamiento de todos los botones del main menu
/// Martin Pérez - 21/10/25
/// </summary>
public class MainMenuManager : MonoBehaviour
{
    private SceneTransitions sceneTransitioner;
    private bool firstSelected = true;

    private void Start()
    {
        sceneTransitioner = GetComponent<SceneTransitions>();
        //Invoke("FirstSelect", 3f);
    }

    public void PlayGame()
    {
        sceneTransitioner.LoadScene("MainScene");
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

    public void FirstSelect(GameObject objectToSelect)
    {
        if(firstSelected)
        {
            if (EventSystem.current.currentSelectedGameObject == null)
                EventSystem.current.SetSelectedGameObject(objectToSelect);

            firstSelected = false;
        }
    }
}
