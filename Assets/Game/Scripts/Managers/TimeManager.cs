using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

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
    
    [SerializeField] private PlayerMovement player;
    [Header("NPCs & puertas")]
    [SerializeField] private List<CharacterBaseScript> restartLoopList = new List<CharacterBaseScript>();
    
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
    }

    public void AddTime(float addedTime)
    {
        currentTime -= addedTime;
    }
    
    private void Update()
    {
        if(!playerIsInterating)
            currentTime += Time.deltaTime;
        
        Debug.Log("Etapa: " + currentStage + " Tiempo: " + currentTime);
        
        if (currentTime >= etapasTemporales[currentStage].duracion)
        {
            //Se acabó el juego
            if (currentStage + 1 >= etapasTemporales.Count)
            {
                Debug.Log("Game Over");

                //TODO: Función de finalizar el juego (Fundido a negro, sonido de disparo...)
                
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

                if (etapasTemporales[currentStage].dialogoFinalEtapa != null)
                {
                    MessageManager.Instance.dialogueActivator.UpdateDialogueObject(etapasTemporales[currentStage].dialogoFinalEtapa);
                    MessageManager.Instance.Interactuar();
                }
                
                // Reiniciar información al completo
                RestartLoopInfo();
            }
            else
            {
                //Si hay más etapas resetear chrono y avanzar.

                Debug.Log("Cambio de etapa");
            
                currentTime = 0;
                currentStage++;
            }
            
            //TODO: Lanzar sonidos de campanadas cuando termine una etapa
        }
    }

    public void RestartLoopInfo()
    {
        player.RestartAnimator();
        InventoryManager.Instance.RestartLoopInventory();
        foreach (CharacterBaseScript character in restartLoopList)
        {
            character.BucleReset();
        }
    }
}

[Serializable]
public class TimeStage
{
    public float duracion;
    [Description("Eventos que se lanzaran al finalizar la duracion de la etapa")]
    public List<string> eventos;
    public DialogueObject dialogoFinalEtapa;
}