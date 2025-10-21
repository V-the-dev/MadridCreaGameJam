using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

[System.Serializable]
public class InventoryDialogueEntry
{
    public DialogueObject dialogueObject;

    public bool hasAssociatedEvent;
    public int selectedEventIndex = -1;
    public bool eventFlag;

    public bool[] eventResponseAssociated = Array.Empty<bool>();
    public int[] eventsAssociated = Array.Empty<int>();
    public bool[] eventsFlagAssociated = Array.Empty<bool>();
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
    public Evento TryGetEventoFromResponse(DialogueObject dialogueObject, int responseIndex)
    {
        if (!dialogueObject) return null;
        InventoryDialogueEntry entry = GetEntryWithDialogueObject(dialogueObject);
        if (entry == null) return null;
        if (entry.eventResponseAssociated[responseIndex])
        {
            return inventoryObject.eventos[entry.eventsAssociated[responseIndex]];
        }
        return null;
    }

    public bool? TryGetEventoValueFromResponse(DialogueObject dialogueObject, int responseIndex)
    {
        if(!dialogueObject) return null;
        InventoryDialogueEntry entry = GetEntryWithDialogueObject(dialogueObject);
        if (entry == null) return null;
        if (entry.eventResponseAssociated[responseIndex])
        {
            return entry.eventsFlagAssociated[responseIndex];
        }
        return null;
    }

    [CanBeNull]
    public Evento TryGetEvento(DialogueObject dialogueObject)
    {
        if(!dialogueObject) return null;
        InventoryDialogueEntry entry = GetEntryWithDialogueObject(dialogueObject);
        if (entry == null) return null;
        if (entry.hasAssociatedEvent)
        {
            return inventoryObject.eventos[entry.selectedEventIndex];
        }

        return null;
    }

    public bool? TryGetEventValue(DialogueObject dialogueObject)
    {
        if (!dialogueObject) return null;
        InventoryDialogueEntry entry = GetEntryWithDialogueObject(dialogueObject);
        if (entry == null) return null;
        if (entry.hasAssociatedEvent)
        {
            return entry.eventFlag;
        }

        return null;
    }
}