using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Script que controla el tiempo global de la escena, divide la noche en etapas controlables que contienen
/// ciertos eventos vinculados a ciertas horas de la noche.
/// 
/// Martin Pérez Villabrille - 22/10/25
/// </summary>
public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }

    public List<TimeStage> etapasTemporales = new List<TimeStage>();

    private bool playerIsInterating = false;

    private int currentStage = 0;
    private float currentTime = 0;

    private float totalTime = 0;
    private float timeMultipliyer = 1;

    public bool DebugTime;
    
    [HideInInspector] public bool timeManipulated = false;
    private bool gameOver = false;
    
    [SerializeField] private PlayerMovement player;
    [Header("NPCs & puertas")]
    [SerializeField] private List<CharacterBaseScript> restartLoopList = new List<CharacterBaseScript>();
    
    [Header("RestartBucle vars")]
    [SerializeField] private Transform playerFirstPosition;
    
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

        GetTotalTime();
    }

    public float GetTotalTime()
    {
        for (int i = 0; i < etapasTemporales.Count; i++)
        {
            totalTime += etapasTemporales[i].duracion;
        }
        
        return totalTime;
    }

    public void SlowTime(float percentageSlowed)
    {
        timeMultipliyer = percentageSlowed;
        timeManipulated = true;
    }
    
    private void Update()
    {
        if (gameOver)
            return;
        
        if(!playerIsInterating)
            currentTime += Time.deltaTime * timeMultipliyer;
        
        if(totalTime <= 0)
            return;
        
        if(DebugTime)
            Debug.Log("Etapa: " + currentStage + " Tiempo: " + currentTime);
        
        if (currentTime >= etapasTemporales[currentStage].duracion)
        {
            //Lanza todos los eventos que se tuvieran que activar al final de la etapa
            for (int i = 0; i < etapasTemporales[currentStage].eventos.Count; i++)
            {
                //Consigue la variable actual del evento del inventory manager y si existe ese evento se la devuelve al revés para triggerearlo
                bool? eventValue = InventoryManager.Instance.GetEventValue(etapasTemporales[currentStage].eventos[i]);
                    
                if (eventValue != null)
                {
                    InventoryManager.Instance.SetEventValue(etapasTemporales[currentStage].eventos[i], !eventValue.Value);
                }
                else
                {
                    Debug.LogError("Evento no encontrado en la lista del Inventory Manager. Posiblemente el nombre del evento este mal escrito");
                }
            }
                
            //Lanzar todas las funciones que se tengan que ejecutar
            for (int i = 0; i < etapasTemporales[currentStage].funciones.Count; i++)
            {
                etapasTemporales[currentStage].funciones[i].Invoke();
            }

            if (etapasTemporales[currentStage].dialogoFinalEtapa != null)
            {
                MessageManager.Instance.dialogueActivator.UpdateDialogueObject(etapasTemporales[currentStage].dialogoFinalEtapa);
                MessageManager.Instance.Interactuar();
            }
            
            if (currentStage + 1 >= etapasTemporales.Count)
            {
                //Se acabó el juego
                Debug.Log("Game Over");
                
                gameOver = true;
                
                PlayBellSound(4);
            }
            else
            {
                Debug.Log("Cambio de etapa");
            
                currentTime = 0;
                currentStage++;
            }
            
            PlayBellSound(currentStage);
        }
    }

    private void PlayBellSound(int i)
    {
        switch (i)
        {
            case 1:
                SoundManager.PlaySound(SoundType.BELL1);
                break;
            
            case 2:
                SoundManager.PlaySound(SoundType.BELL2);
                break;
            
            case 3:
                SoundManager.PlaySound(SoundType.BELL3);
                break;
            
            case 4:
                SoundManager.PlaySound(SoundType.BELL4);
                break;
        }
    }

    public void RestartLoop()
    {
        player.RestartAnimator();
        
        if(playerFirstPosition) player.gameObject.transform.position = playerFirstPosition.position;
        
        InventoryManager.Instance.RestartLoopInventory();
        
        foreach (CharacterBaseScript character in restartLoopList)
        {
            character.BucleReset();
        }

        currentStage = 0;
        currentTime = 0;

        gameOver = false;
        
        GameManager.Instance.RestartLoopInfo();

        /// TODO: Terminar de implementar la lógica del reinicio del bucle
    }
}

[Serializable]
public class TimeStage
{
    public float duracion;
    [Description("Eventos que se lanzaran al finalizar la duracion de la etapa")]
    public List<string> eventos;
    public List<UnityEvent> funciones = new List<UnityEvent>();
    public DialogueObject dialogoFinalEtapa;
}