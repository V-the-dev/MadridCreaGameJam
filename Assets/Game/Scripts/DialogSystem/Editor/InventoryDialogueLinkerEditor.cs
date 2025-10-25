using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;

[CustomEditor(typeof(InventoryDialogueLinker))]
public class InventoryDialogueLinkerEditor : Editor
{
    private SerializedProperty inventoryObjectProp = null;
    private SerializedProperty entriesProp = null;
    private bool showEntries = false;
    private Dictionary<int, bool> showEntryDetails = new Dictionary<int, bool>();
    private Dictionary<int, bool> showEventsDict = new Dictionary<int, bool>();
    private Dictionary<int, bool> showObjectsDict = new Dictionary<int, bool>();
    private Dictionary<string, bool> showResponseEventsDict = new Dictionary<string, bool>();
    private Dictionary<string, bool> showResponseObjectsDict = new Dictionary<string, bool>();
    private Dictionary<int, bool> showResponsesDict = new Dictionary<int, bool>();

    private void OnEnable()
    {
        inventoryObjectProp = serializedObject.FindProperty("inventoryObject");
        entriesProp = serializedObject.FindProperty("entries");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // ---- INVENTORY OBJECT ----
        EditorGUILayout.PropertyField(inventoryObjectProp);

        InventarioObject inventoryObject = (InventarioObject)inventoryObjectProp.objectReferenceValue;

        EditorGUILayout.Space();

        // ---- LISTA ----
        showEntries = EditorGUILayout.Foldout(showEntries, "Entradas de Diálogo", true);
        if (showEntries)
        {
            EditorGUI.indentLevel++;

            for (int i = 0; i < entriesProp.arraySize; i++)
            {
                SerializedProperty entryProp = entriesProp.GetArrayElementAtIndex(i);
                SerializedProperty dialogueObjectProp = entryProp.FindPropertyRelative("dialogueObject");
                SerializedProperty hasEventProp = entryProp.FindPropertyRelative("hasAssociatedEvent");
                SerializedProperty hasObjectProp = entryProp.FindPropertyRelative("hasAssociatedObject");
                SerializedProperty associatedEventsProp = entryProp.FindPropertyRelative("associatedEvents");
                SerializedProperty associatedObjectsProp = entryProp.FindPropertyRelative("associatedObjects");
                
                SerializedProperty eventResponseAssociatedProp = entryProp.FindPropertyRelative("eventResponseAssociated");
                SerializedProperty objectResponseAssociatedProp = entryProp.FindPropertyRelative("objectResponseAssociated");
                SerializedProperty eventsAssociatedPerResponseProp = entryProp.FindPropertyRelative("eventsAssociatedPerResponse");
                SerializedProperty objectsAssociatedPerResponseProp = entryProp.FindPropertyRelative("objectsAssociatedPerResponse");

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.BeginHorizontal();
                
                if (!showEntryDetails.ContainsKey(i))
                    showEntryDetails[i] = true;

                string entryLabel = dialogueObjectProp.objectReferenceValue != null
                    ? dialogueObjectProp.objectReferenceValue.name
                    : $"Entrada {i + 1}";
                
                showEntryDetails[i] = EditorGUILayout.Foldout(showEntryDetails[i], entryLabel, true, EditorStyles.foldoutHeader);
                
                GUILayout.FlexibleSpace();

                if (GUILayout.Button("▲", GUILayout.Width(25)) && i > 0)
                    entriesProp.MoveArrayElement(i, i - 1);

                if (GUILayout.Button("▼", GUILayout.Width(25)) && i < entriesProp.arraySize - 1)
                    entriesProp.MoveArrayElement(i, i + 1);

                if (GUILayout.Button("✕", GUILayout.Width(25)))
                {
                    entriesProp.DeleteArrayElementAtIndex(i);
                    break;
                }
                EditorGUILayout.EndHorizontal();

                if (showEntryDetails[i])
                {
                    EditorGUI.indentLevel++;
                    
                    EditorGUILayout.PropertyField(dialogueObjectProp);
                    DialogueObject dialogueObject = (DialogueObject)dialogueObjectProp.objectReferenceValue;

                    // ---- EVENTOS ASOCIADOS (LISTA) ----
                    if (inventoryObject)
                    {
                        EditorGUILayout.Space();
                        EditorGUILayout.PropertyField(hasEventProp, new GUIContent("Eventos asociados"));

                        if (hasEventProp.boolValue)
                        {
                            if (!showEventsDict.ContainsKey(i))
                                showEventsDict[i] = true;

                            showEventsDict[i] = EditorGUILayout.Foldout(showEventsDict[i], "Lista de Eventos", true);
                            
                            if (showEventsDict[i])
                            {
                                EditorGUI.indentLevel++;
                                
                                if (GUILayout.Button("+ Añadir Evento"))
                                {
                                    associatedEventsProp.arraySize++;
                                }

                                var eventos = inventoryObject.GetEventos() != null
                                    ? inventoryObject.GetEventos().Select(e => e.nombre).ToArray()
                                    : new string[0];

                                for (int j = 0; j < associatedEventsProp.arraySize; j++)
                                {
                                    SerializedProperty eventDataProp = associatedEventsProp.GetArrayElementAtIndex(j);
                                    SerializedProperty eventIndexProp = eventDataProp.FindPropertyRelative("eventIndex");
                                    SerializedProperty eventFlagProp = eventDataProp.FindPropertyRelative("eventFlag");

                                    EditorGUILayout.BeginHorizontal();
                                    
                                    int newIndex = EditorGUILayout.Popup(eventIndexProp.intValue, eventos, GUILayout.Width(150));
                                    if (newIndex != eventIndexProp.intValue)
                                        eventIndexProp.intValue = newIndex;

                                    EditorGUILayout.PropertyField(eventFlagProp, GUIContent.none);

                                    if (GUILayout.Button("✕", GUILayout.Width(25)))
                                    {
                                        associatedEventsProp.DeleteArrayElementAtIndex(j);
                                        break;
                                    }
                                    
                                    EditorGUILayout.EndHorizontal();
                                }
                                
                                EditorGUI.indentLevel--;
                            }
                        }
                    }
                    
                    // ---- OBJETOS ASOCIADOS (LISTA) ----
                    if (inventoryObject)
                    {
                        EditorGUILayout.Space();
                        EditorGUILayout.PropertyField(hasObjectProp, new GUIContent("Objetos asociados"));

                        if (hasObjectProp.boolValue)
                        {
                            if (!showObjectsDict.ContainsKey(i))
                                showObjectsDict[i] = true;

                            showObjectsDict[i] = EditorGUILayout.Foldout(showObjectsDict[i], "Lista de Objetos", true);
                            
                            if (showObjectsDict[i])
                            {
                                EditorGUI.indentLevel++;
                                
                                if (GUILayout.Button("+ Añadir Objeto"))
                                {
                                    associatedObjectsProp.arraySize++;
                                }

                                var objetos = inventoryObject.GetObjetos() != null
                                    ? inventoryObject.GetObjetos().Select(e => e.nombre).ToArray()
                                    : new string[0];

                                for (int j = 0; j < associatedObjectsProp.arraySize; j++)
                                {
                                    SerializedProperty objectDataProp = associatedObjectsProp.GetArrayElementAtIndex(j);
                                    SerializedProperty objectIndexProp = objectDataProp.FindPropertyRelative("objectIndex");
                                    SerializedProperty objectQuantityProp = objectDataProp.FindPropertyRelative("quantity");

                                    EditorGUILayout.BeginHorizontal();
                                    
                                    int newIndex = EditorGUILayout.Popup(objectIndexProp.intValue, objetos, GUILayout.Width(150));
                                    if (newIndex != objectIndexProp.intValue)
                                        objectIndexProp.intValue = newIndex;

                                    EditorGUILayout.PropertyField(objectQuantityProp, GUIContent.none);

                                    if (GUILayout.Button("✕", GUILayout.Width(25)))
                                    {
                                        associatedObjectsProp.DeleteArrayElementAtIndex(j);
                                        break;
                                    }
                                    
                                    EditorGUILayout.EndHorizontal();
                                }
                                
                                EditorGUI.indentLevel--;
                            }
                        }
                    }

                    // ---- RESPUESTAS ASOCIADAS ----
                    if (dialogueObject)
                    {
                        int responseCount = dialogueObject.Responses.Length;
                        
                        // Ajustar tamaños de arrays
                        eventResponseAssociatedProp.arraySize = responseCount;
                        objectResponseAssociatedProp.arraySize = responseCount;
                        eventsAssociatedPerResponseProp.arraySize = responseCount;
                        objectsAssociatedPerResponseProp.arraySize = responseCount;

                        if (!showResponsesDict.ContainsKey(i))
                            showResponsesDict[i] = false;

                        showResponsesDict[i] = EditorGUILayout.Foldout(showResponsesDict[i], "Respuestas", true);
                        
                        if (showResponsesDict[i])
                        {
                            EditorGUI.indentLevel++;
                            for (int j = 0; j < responseCount; j++)
                            {
                                SerializedProperty eventResponseVar = eventResponseAssociatedProp.GetArrayElementAtIndex(j);
                                SerializedProperty objectResponseVar = objectResponseAssociatedProp.GetArrayElementAtIndex(j);
                                SerializedProperty eventsListWrapperProp = eventsAssociatedPerResponseProp.GetArrayElementAtIndex(j);
                                SerializedProperty eventsListProp = eventsListWrapperProp.FindPropertyRelative("events");
                                SerializedProperty objectsListWrapperProp = objectsAssociatedPerResponseProp.GetArrayElementAtIndex(j);
                                SerializedProperty objectsListProp = objectsListWrapperProp.FindPropertyRelative("objects");

                                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                                EditorGUILayout.LabelField(dialogueObject.Responses[j].ResponseText, EditorStyles.boldLabel);

                                EditorGUILayout.PropertyField(eventResponseVar, new GUIContent("¿Eventos asociados?"));

                                if (eventResponseVar.boolValue)
                                {
                                    if (inventoryObject && inventoryObject.eventos != null && inventoryObject.eventos.Count > 0)
                                    {
                                        string responseEventKey = $"{i}_{j}_events";
                                        if (!showResponseEventsDict.ContainsKey(responseEventKey))
                                            showResponseEventsDict[responseEventKey] = true;

                                        showResponseEventsDict[responseEventKey] = EditorGUILayout.Foldout(
                                            showResponseEventsDict[responseEventKey], "Lista de Eventos", true);

                                        if (showResponseEventsDict[responseEventKey])
                                        {
                                            EditorGUI.indentLevel++;

                                            if (GUILayout.Button("+ Añadir Evento"))
                                            {
                                                eventsListProp.arraySize++;
                                            }

                                            string[] nombresEventos = inventoryObject.GetEventos()
                                                .Select(e => e != null ? e.nombre : "(Evento nulo)")
                                                .ToArray();

                                            for (int k = 0; k < eventsListProp.arraySize; k++)
                                            {
                                                SerializedProperty eventDataProp = eventsListProp.GetArrayElementAtIndex(k);
                                                SerializedProperty eventIndexProp = eventDataProp.FindPropertyRelative("eventIndex");
                                                SerializedProperty eventFlagProp = eventDataProp.FindPropertyRelative("eventFlag");

                                                int currentIndex = eventIndexProp.intValue;
                                                if (currentIndex < 0 || currentIndex >= nombresEventos.Length)
                                                {
                                                    currentIndex = 0;
                                                    eventIndexProp.intValue = 0;
                                                }

                                                EditorGUILayout.BeginHorizontal();
                                                
                                                int newIndex = EditorGUILayout.Popup(currentIndex, nombresEventos, GUILayout.Width(150));
                                                if (newIndex != currentIndex)
                                                {
                                                    eventIndexProp.intValue = newIndex;
                                                }

                                                EditorGUILayout.PropertyField(eventFlagProp, GUIContent.none);

                                                if (GUILayout.Button("✕", GUILayout.Width(25)))
                                                {
                                                    eventsListProp.DeleteArrayElementAtIndex(k);
                                                    break;
                                                }

                                                EditorGUILayout.EndHorizontal();
                                            }

                                            EditorGUI.indentLevel--;
                                        }
                                    }
                                    else
                                    {
                                        EditorGUILayout.LabelField("Sin eventos disponibles");
                                    }
                                }

                                EditorGUILayout.Space(3);
                                
                                EditorGUILayout.PropertyField(objectResponseVar, new GUIContent("¿Objetos asociados?"));

                                if (objectResponseVar.boolValue)
                                {
                                    if (inventoryObject && 
                                        inventoryObject.objetos.monedas != null && 
                                        inventoryObject.objetos.objetosClave != null)
                                    {
                                        string responseObjectKey = $"{i}_{j}_objects";
                                        if (!showResponseObjectsDict.ContainsKey(responseObjectKey))
                                            showResponseObjectsDict[responseObjectKey] = true;

                                        showResponseObjectsDict[responseObjectKey] = EditorGUILayout.Foldout(
                                            showResponseObjectsDict[responseObjectKey], "Lista de Objetos", true);

                                        if (showResponseObjectsDict[responseObjectKey])
                                        {
                                            EditorGUI.indentLevel++;

                                            if (GUILayout.Button("+ Añadir Objeto"))
                                            {
                                                objectsListProp.arraySize++;
                                            }

                                            string[] nombresObjetos = inventoryObject.GetObjetos()
                                                .Select(e => e != null ? e.nombre : "(Objeto nulo)")
                                                .ToArray();

                                            for (int k = 0; k < objectsListProp.arraySize; k++)
                                            {
                                                SerializedProperty objectDataProp = objectsListProp.GetArrayElementAtIndex(k);
                                                SerializedProperty objectIndexProp = objectDataProp.FindPropertyRelative("objectIndex");
                                                SerializedProperty objectQuantityProp = objectDataProp.FindPropertyRelative("quantity");

                                                int currentIndex = objectIndexProp.intValue;
                                                if (currentIndex < 0 || currentIndex >= nombresObjetos.Length)
                                                {
                                                    currentIndex = 0;
                                                    objectIndexProp.intValue = 0;
                                                }

                                                EditorGUILayout.BeginHorizontal();
                                                
                                                int newIndex = EditorGUILayout.Popup(currentIndex, nombresObjetos, GUILayout.Width(150));
                                                if (newIndex != currentIndex)
                                                {
                                                    objectIndexProp.intValue = newIndex;
                                                }

                                                EditorGUILayout.PropertyField(objectQuantityProp, GUIContent.none);

                                                if (GUILayout.Button("✕", GUILayout.Width(25)))
                                                {
                                                    objectsListProp.DeleteArrayElementAtIndex(k);
                                                    break;
                                                }

                                                EditorGUILayout.EndHorizontal();
                                            }

                                            EditorGUI.indentLevel--;
                                        }
                                    }
                                    else
                                    {
                                        EditorGUILayout.LabelField("Sin objetos disponibles");
                                    }
                                }

                                EditorGUILayout.EndVertical();
                                EditorGUILayout.Space(3);
                            }
                            EditorGUI.indentLevel--;
                        }
                    }
                    EditorGUI.indentLevel--;
                }


                EditorGUILayout.EndVertical();
                EditorGUILayout.Space();
            }

            EditorGUI.indentLevel--;
        }

        EditorGUILayout.Space();

        // ---- BOTÓN PARA AÑADIR ELEMENTOS ----
        if (GUILayout.Button("+ Añadir entrada de diálogo"))
        {
            int newIndex = entriesProp.arraySize;
            entriesProp.arraySize++;

            SerializedProperty newEntry = entriesProp.GetArrayElementAtIndex(newIndex);

            // Limpia las referencias y valores básicos
            newEntry.FindPropertyRelative("dialogueObject").objectReferenceValue = null;
            newEntry.FindPropertyRelative("hasAssociatedEvent").boolValue = false;
            newEntry.FindPropertyRelative("hasAssociatedObject").boolValue = false;

            // Vacía las listas internas
            newEntry.FindPropertyRelative("associatedEvents").ClearArray();
            newEntry.FindPropertyRelative("associatedObjects").ClearArray();
            newEntry.FindPropertyRelative("eventResponseAssociated").ClearArray();
            newEntry.FindPropertyRelative("objectResponseAssociated").ClearArray();
            newEntry.FindPropertyRelative("eventsAssociatedPerResponse").ClearArray();
            newEntry.FindPropertyRelative("objectsAssociatedPerResponse").ClearArray();
        }

        serializedObject.ApplyModifiedProperties();
    }
}