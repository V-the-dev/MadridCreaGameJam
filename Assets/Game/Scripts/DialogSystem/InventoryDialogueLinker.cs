using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

[System.Serializable]
public class EventData
{
    public int eventIndex = -1;
    public bool eventFlag = false;
}

[System.Serializable]
public class ObjectData
{
    public int objectIndex = -1;
    public int quantity = 0;
}

// Clases wrapper para hacer serializables los arrays de listas
[System.Serializable]
public class EventDataList
{
    public List<EventData> events = new List<EventData>();
}

[System.Serializable]
public class ObjectDataList
{
    public List<ObjectData> objects = new List<ObjectData>();
}

[System.Serializable]
public class InventoryDialogueEntry
{
    public DialogueObject dialogueObject;

    public bool hasAssociatedEvent;
    public List<EventData> associatedEvents = new List<EventData>();

    public bool hasAssociatedObject;
    public List<ObjectData> associatedObjects = new List<ObjectData>();

    public List<bool> eventResponseAssociated = new List<bool>();
    public List<EventDataList> eventsAssociatedPerResponse = new List<EventDataList>();
    
    public List<bool> objectResponseAssociated = new List<bool>();
    public List<ObjectDataList> objectsAssociatedPerResponse = new List<ObjectDataList>();
}

public class InventoryDialogueLinker : MonoBehaviour
{
    public InventarioObject inventoryObject;
    public List<InventoryDialogueEntry> entries = new List<InventoryDialogueEntry>();

    public InventoryDialogueEntry GetEntryWithDialogueObject(DialogueObject dialogueObject)
    {
        return entries.Find(e => e.dialogueObject == dialogueObject);
    }

    [CanBeNull]
    public List<Evento> TryGetEventosFromResponse(DialogueObject dialogueObject, int responseIndex)
    {
        if (!dialogueObject) return null;
        InventoryDialogueEntry entry = GetEntryWithDialogueObject(dialogueObject);
        if (entry == null) return null;
        if (responseIndex >= entry.eventResponseAssociated.Count || !entry.eventResponseAssociated[responseIndex]) 
            return null;
        
        if (responseIndex >= entry.eventsAssociatedPerResponse.Count || 
            entry.eventsAssociatedPerResponse[responseIndex] == null) 
            return null;

        List<Evento> eventos = new List<Evento>();
        foreach (var eventData in entry.eventsAssociatedPerResponse[responseIndex].events)
        {
            if (eventData.eventIndex >= 0 && eventData.eventIndex < inventoryObject.eventos.Count)
            {
                eventos.Add(inventoryObject.eventos[eventData.eventIndex]);
            }
        }
        return eventos.Count > 0 ? eventos : null;
    }
    
    [CanBeNull]
    public List<Objeto> TryGetObjetosFromResponse(DialogueObject dialogueObject, int responseIndex)
    {
        if (!dialogueObject) return null;
        InventoryDialogueEntry entry = GetEntryWithDialogueObject(dialogueObject);
        if (entry == null) return null;
        if (responseIndex >= entry.objectResponseAssociated.Count || !entry.objectResponseAssociated[responseIndex]) 
            return null;
        
        if (responseIndex >= entry.objectsAssociatedPerResponse.Count || 
            entry.objectsAssociatedPerResponse[responseIndex] == null) 
            return null;

        List<Objeto> objetos = new List<Objeto>();
        foreach (var objectData in entry.objectsAssociatedPerResponse[responseIndex].objects)
        {
            if (objectData.objectIndex >= 0)
            {
                if (objectData.objectIndex < inventoryObject.objetos.monedas.Count)
                {
                    objetos.Add(inventoryObject.objetos.monedas[objectData.objectIndex]);
                }
                else
                {
                    int adjustedIndex = objectData.objectIndex - inventoryObject.objetos.monedas.Count;
                    if (adjustedIndex < inventoryObject.objetos.objetosClave.Count)
                    {
                        objetos.Add(inventoryObject.objetos.objetosClave[adjustedIndex]);
                    }
                }
            }
        }
        return objetos.Count > 0 ? objetos : null;
    }

    [CanBeNull]
    public List<EventData> TryGetEventDataFromResponse(DialogueObject dialogueObject, int responseIndex)
    {
        if(!dialogueObject) return null;
        InventoryDialogueEntry entry = GetEntryWithDialogueObject(dialogueObject);
        if (entry == null) return null;
        if (responseIndex >= entry.eventResponseAssociated.Count || !entry.eventResponseAssociated[responseIndex]) 
            return null;
        
        if (responseIndex >= entry.eventsAssociatedPerResponse.Count || 
            entry.eventsAssociatedPerResponse[responseIndex] == null) 
            return null;
            
        return entry.eventsAssociatedPerResponse[responseIndex].events;
    }
    
    [CanBeNull]
    public List<ObjectData> TryGetObjectDataFromResponse(DialogueObject dialogueObject, int responseIndex)
    {
        if(!dialogueObject) return null;
        InventoryDialogueEntry entry = GetEntryWithDialogueObject(dialogueObject);
        if (entry == null) return null;
        if (responseIndex >= entry.objectResponseAssociated.Count || !entry.objectResponseAssociated[responseIndex]) 
            return null;
        
        if (responseIndex >= entry.objectsAssociatedPerResponse.Count || 
            entry.objectsAssociatedPerResponse[responseIndex] == null) 
            return null;
            
        return entry.objectsAssociatedPerResponse[responseIndex].objects;
    }

    [CanBeNull]
    public List<Evento> TryGetEventos(DialogueObject dialogueObject)
    {
        if(!dialogueObject) return null;
        InventoryDialogueEntry entry = GetEntryWithDialogueObject(dialogueObject);
        if (entry == null) return null;
        if (!entry.hasAssociatedEvent) return null;

        List<Evento> eventos = new List<Evento>();
        foreach (var eventData in entry.associatedEvents)
        {
            if (eventData.eventIndex >= 0 && eventData.eventIndex <= inventoryObject.eventos.Count)
            {
                eventos.Add(inventoryObject.GetEventos()[eventData.eventIndex]);
            }
        }
        return eventos.Count > 0 ? eventos : null;
    }
    
    [CanBeNull]
    public List<Objeto> TryGetObjetos(DialogueObject dialogueObject)
    {
        if(!dialogueObject) return null;
        InventoryDialogueEntry entry = GetEntryWithDialogueObject(dialogueObject);
        if (entry == null) return null;
        if (!entry.hasAssociatedObject) return null;

        List<Objeto> objetos = new List<Objeto>();
        foreach (var objectData in entry.associatedObjects)
        {
            if (objectData.objectIndex >= 0)
            {
                if (objectData.objectIndex < inventoryObject.objetos.monedas.Count)
                {
                    objetos.Add(inventoryObject.objetos.monedas[objectData.objectIndex]);
                }
                else
                {
                    int adjustedIndex = objectData.objectIndex - inventoryObject.objetos.monedas.Count;
                    if (adjustedIndex < inventoryObject.objetos.objetosClave.Count)
                    {
                        objetos.Add(inventoryObject.objetos.objetosClave[adjustedIndex]);
                    }
                }
            }
        }
        return objetos.Count > 0 ? objetos : null;
    }

    [CanBeNull]
    public List<EventData> TryGetEventData(DialogueObject dialogueObject)
    {
        if (!dialogueObject) return null;
        InventoryDialogueEntry entry = GetEntryWithDialogueObject(dialogueObject);
        if (entry == null) return null;
        if (!entry.hasAssociatedEvent) return null;
        
        return entry.associatedEvents;
    }
    
    [CanBeNull]
    public List<ObjectData> TryGetObjectData(DialogueObject dialogueObject)
    {
        if (!dialogueObject) return null;
        InventoryDialogueEntry entry = GetEntryWithDialogueObject(dialogueObject);
        if (entry == null) return null;
        if (!entry.hasAssociatedObject) return null;
        
        return entry.associatedObjects;
    }
    
    public bool? CanShowDialogue(DialogueObject dialogueObject)
    {
        if(dialogueObject == null) return null;
        bool shouldShowDialogue = true;
        List<Evento> eventos = TryGetEventos(dialogueObject);
        List<EventData> eventValues = TryGetEventData(dialogueObject);
        List<Objeto> objetos = TryGetObjetos(dialogueObject);
        List<ObjectData> objectQuantitys = TryGetObjectData(dialogueObject);

        if (eventos != null)
        {
            for (int i = 0; i < eventos.Count; i++)
            {
                if (eventos[i] != null && eventValues != null)
                {
                    shouldShowDialogue = InventoryManager.Instance.GetEventValue(eventos[i].nombre) ==
                                         eventValues[i].eventFlag;
                }
            }
        }

        if (objetos != null)
        {
            for (int i = 0; i < objetos.Count; i++)
            {
                if (objetos[i] != null && objectQuantitys != null && shouldShowDialogue)
                {
                    shouldShowDialogue = InventoryManager.Instance.GetItemValue(objetos[i].nombre) >= objectQuantitys[i].quantity;
                }
            }
        }
        return shouldShowDialogue;
    }
}