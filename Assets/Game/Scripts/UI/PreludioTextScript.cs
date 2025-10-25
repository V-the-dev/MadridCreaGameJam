using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PreludioTextScript : MonoBehaviour
{
    [SerializeField] private List<string> textList = new List<string>();
    [SerializeField] private float fadeInTime = 1f;
    [SerializeField] private float fadeOutTime = 1f;
    [SerializeField] private float textTime = 1f;
    [SerializeField] private float textDelay = 1f;
    
    [Header("EscenaMainMenu")]
    [SerializeField] private string mainMenuSceneName;

    private TextMeshProUGUI texto;
    
    [Header("Animaciones FadeIn FadeOut")]
    private Animator animator;
    [SerializeField] private AnimationClip fadeIn;
    private bool isFadeIn = false;
    private bool isFadeInWaiting = false;
    [SerializeField] private AnimationClip fadeOut;
    private bool isFadeOut = false;
    private bool isFadeOutWaiting = false;

    private float timeToFunction = 0f;
        
    private void Start()
    {
        animator = GetComponent<Animator>();
        texto = GetComponent<TextMeshProUGUI>();
        if (textList != null && textList.Count <= 0)
        {
            FinishPreludio();
            return;
        }

        texto.text = textList[0];
        textList.RemoveAt(0);
        animator.speed = 1/fadeInTime;
        animator.Play(fadeIn.name);
    }

    private void Update()
    {
        // FadeIn terminado: esperamos a que el texto termine de esperar
        if (isFadeIn)
        {
            timeToFunction = textTime;
            isFadeIn = false;
            isFadeInWaiting = true;
        }

        // Cuando se acaba la espera del texto
        if (isFadeInWaiting && timeToFunction <= 0f)
        {
            isFadeInWaiting = false;
            animator.speed = 1/fadeOutTime;
            animator.Play(fadeOut.name);
        }

        // FadeOut terminado: esperamos al delay del siguiente texto
        if (isFadeOut)
        {
            timeToFunction = textDelay;
            isFadeOut = false;
            isFadeOutWaiting = true;
        }

        // Cuando se acaba el delay del texto
        if (isFadeOutWaiting && timeToFunction <= 0f)
        {
            isFadeOutWaiting = false;
            if (textList.Count <= 0)
            {
                FinishPreludio();
                return;
            }

            texto.text = textList[0];
            textList.RemoveAt(0);
            animator.speed = 1/fadeInTime;
            animator.Play(fadeIn.name);
        }
        
        timeToFunction -= Time.deltaTime;
    }

    // Cuando se termina el FadeIn
    public void FinishedFadeIn()
    {
        isFadeIn = true;
    }

    // Cuando se termina el FadeOut
    public void FinishedFadeOut()
    {
        isFadeOut = true;
    }

    public void FinishPreludio()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
