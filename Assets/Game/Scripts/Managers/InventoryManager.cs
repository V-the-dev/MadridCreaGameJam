using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }
    
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private GameObject inventoryList;
    [SerializeField] private GameObject inventoryItem;
    [SerializeField] private InventarioObject inventarioInicial;
    
    [SerializeField] private RectTransform hiddenInventoryPosition;
    [SerializeField] private RectTransform visibleInventoryPosition;
    [SerializeField, Range(0.1f, 10f)] private float inventoryMovementSpeed = 5f;

    private Dictionary<string, GameObject> inventoryDictionary = new Dictionary<string, GameObject>();
    private Dictionary<string, Sprite> objectsSprite = new Dictionary<string, Sprite>();
    private Dictionary<string, bool> eventsDictionary = new Dictionary<string, bool>();

    private InputAction _showInventoryAction;
    private bool inputBlocked = false;

    private bool inventoryVisible = false;
    private bool inventoryMoving = true;
    
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

    private void Start()
    {
        _showInventoryAction = InputSystem.actions.FindAction("UI/ShowInventory");
        if (inventoryDictionary.Count == 0)
        {
            InitializeInventory();
        }
    }

    private void Update()
    {
        if (!inputBlocked)
        {
            if (_showInventoryAction.WasPressedThisFrame())
            {
                ShowInventoryPanel();
            }
            if (_showInventoryAction.WasReleasedThisFrame())
            {
                HideInventoryPanel();
            }
        }
        if (inventoryMoving)
        {
            RectTransform inventoryPanelPosition = inventoryPanel.GetComponent<RectTransform>();
            Vector2 posicionActual = inventoryPanelPosition.anchoredPosition;
            Vector2 posicionObjetivo;
            if (!inventoryVisible)
            {
                posicionObjetivo = hiddenInventoryPosition.anchoredPosition;
            }
            else
            {
                posicionObjetivo = visibleInventoryPosition.anchoredPosition;
            }
            // Interpolación suave
            Vector2 nuevaPosicion = Vector2.Lerp(posicionActual, posicionObjetivo, Time.unscaledDeltaTime * inventoryMovementSpeed);

            // Asignar nueva posición
            inventoryPanelPosition.anchoredPosition = nuevaPosicion;

            // Si está suficientemente cerca, detener el movimiento
            if (Vector2.Distance(inventoryPanelPosition.anchoredPosition, posicionObjetivo) < 0.1f)
            {
                inventoryPanelPosition.anchoredPosition = posicionObjetivo;
                inventoryMoving = false;
            }
        }
    }

    public void RestartLoopInventory()
    {
        foreach (GameObject inventoryItemVar in inventoryDictionary.Values)
        {
            Destroy(inventoryItemVar);
        }
        inventoryDictionary.Clear();
        
        foreach (Moneda moneda in inventarioInicial.objetos.monedas)
        {
            objectsSprite.Add(moneda.nombre, moneda.sprite);
            if (moneda.valor > 0)
            {
                AddItemToInventory(moneda.nombre, moneda.valor);
            }
        }

        foreach (ObjetoClave objetoClave in inventarioInicial.objetos.objetosClave)
        {
            objectsSprite.Add(objetoClave.nombre, objetoClave.sprite);
            if (objetoClave.valor > 0)
            {
                AddItemToInventory(objetoClave.nombre, objetoClave.valor);
            }
        }

        foreach (Evento evento in inventarioInicial.eventos)
        {
            if (!evento.isLoopPersistent)
            {
                eventsDictionary[evento.nombre] = evento.startValue;
            }
        }
    }

    public int AddItemToInventory(string itemName, int amount)
    {
        if (amount <= 0) return -1;
        
        // Si tiene ese item
        if (inventoryDictionary.TryGetValue(itemName, out var item))
        {
            TMP_Text itemText = item.GetComponentInChildren<TMP_Text>();
            if (itemText == null) return -1;
            if (int.TryParse(itemText.text.Substring(1), out int previousAmount))
            {
                itemText.text = "x"+(previousAmount + amount);
            }
            else
            {
                return -1;
            }
        }
        // No tiene el item, lo creamos
        else
        {
            GameObject inventoryItemVar =  Instantiate(inventoryItem, inventoryList.transform);
            inventoryItemVar.GetComponentInChildren<Image>().sprite = objectsSprite[itemName];
            inventoryItemVar.GetComponentInChildren<TMP_Text>().text = "x"+amount;
            inventoryDictionary.Add(itemName, inventoryItemVar);
        }

        return 0;
    }

    public int RemoveItemFromInventory(string itemName, int amount)
    {
        if (amount >= 0) return -1;
        
        if (inventoryDictionary.TryGetValue(itemName, out var item))
        {
            TMP_Text itemText = item.GetComponentInChildren<TMP_Text>();
            if (int.TryParse(itemText.text.Substring(1), out int previousAmount))
            {
                if(previousAmount - amount <= 0)
                {
                    Destroy(inventoryDictionary[itemName]);
                    inventoryDictionary.Remove(itemName);
                }
                else
                {
                    itemText.text = "x"+(previousAmount - amount);
                }
                
                return 0;
            }
        }
        return -1;
    }

    public int GetItemValue(string itemName)
    {
        if (eventsDictionary.Count == 0)
        {
            InitializeInventory();
        }
        if (inventoryDictionary.TryGetValue(itemName, out GameObject item))
        {
            TMP_Text itemText = item.GetComponentInChildren<TMP_Text>();
            if (itemText == null) return -1;
            if (int.TryParse(itemText.text.Substring(1), out int actualAmount))
            {
                return actualAmount;
            }
        }

        return -1;
    }

    public bool? GetEventValue(string eventName)
    {
        if (eventsDictionary.Count == 0)
        {
            InitializeInventory();
        }
        if (eventsDictionary.TryGetValue(eventName, out bool value))
        {
            return value;
        }

        return null;
    }

    public InventarioObject GetInventario()
    {
        return inventarioInicial;
    }

    public void ShowInventoryPanel()
    {
        inventoryVisible = true;
        inventoryMoving = true;
    }

    public void HideInventoryPanel()
    {
        inventoryVisible = false;
        inventoryMoving = true;
    }

    public void ShowInventoryPanelBlockingInput()
    {
        inventoryVisible = true;
        inventoryMoving = true;
        inputBlocked = true;
    }

    public void HideInventoryPanelBlockingInput()
    {
        inventoryVisible = false;
        inventoryMoving = true;
        inputBlocked = false;
    }

    public void InitializeInventory()
    {
        objectsSprite.Clear();
        inventoryDictionary.Clear();
        eventsDictionary.Clear();
        foreach (Moneda moneda in inventarioInicial.objetos.monedas)
        {
            objectsSprite.Add(moneda.nombre, moneda.sprite);
            AddItemToInventory(moneda.nombre, moneda.valor);
        }

        foreach (ObjetoClave objetoClave in inventarioInicial.objetos.objetosClave)
        {
            objectsSprite.Add(objetoClave.nombre, objetoClave.sprite);
            AddItemToInventory(objetoClave.nombre, objetoClave.valor);
        }

        foreach (Evento evento in inventarioInicial.eventos)
        {
            eventsDictionary.Add(evento.nombre, evento.startValue);
        }
    }
}
