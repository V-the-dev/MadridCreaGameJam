using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;

[CustomEditor(typeof(InventoryDialogueLinker))]
public class InventoryDialogueLinkerEditor : Editor
{
    private SerializedProperty inventoryObjectProp = null;
    private SerializedProperty entriesProp = null;
    private bool showEntries = true;
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

        // ---- BOTÓN PARA AÑADIR ELEMENTOS ----
        if (GUILayout.Button("+ Añadir entrada de diálogo"))
        {
            entriesProp.arraySize++;
        }

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

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField($"Entrada {i + 1}", EditorStyles.boldLabel);
                if (GUILayout.Button("▲", GUILayout.Width(25)) && i > 0)
                {
                    entriesProp.MoveArrayElement(i, i - 1);
                }
                if (GUILayout.Button("▼", GUILayout.Width(25)) && i < entriesProp.arraySize - 1)
                {
                    entriesProp.MoveArrayElement(i, i + 1);
                }
                if (GUILayout.Button("✕", GUILayout.Width(25)))
                {
                    entriesProp.DeleteArrayElementAtIndex(i);
                    break;
                }
                EditorGUILayout.EndHorizontal();

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

                            var objetos = inventoryObject.objetos.monedas != null
                                ? inventoryObject.objetos.monedas.Select(e => e.nombre).ToArray()
                                : new string[0];
                            objetos = Enumerable.Concat(objetos, inventoryObject.objetos.objetosClave != null
                                ? inventoryObject.objetos.objetosClave.Select(e => e.nombre).ToArray()
                                : new string[0]).ToArray();

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
                    
                    InventoryDialogueLinker linker = (InventoryDialogueLinker)target;
                    InventoryDialogueEntry entry = linker.entries[i];
                    
                    // Ajustar tamaños de arrays simples
                    eventResponseAssociatedProp.arraySize = responseCount;
                    objectResponseAssociatedProp.arraySize = responseCount;
                    
                    // Ajustar tamaños de arrays de listas directamente
                    if (entry.eventsAssociatedPerResponse == null || entry.eventsAssociatedPerResponse.Length != responseCount)
                    {
                        System.Array.Resize(ref entry.eventsAssociatedPerResponse, responseCount);
                    }
                    if (entry.objectsAssociatedPerResponse == null || entry.objectsAssociatedPerResponse.Length != responseCount)
                    {
                        System.Array.Resize(ref entry.objectsAssociatedPerResponse, responseCount);
                    }
                    
                    // Inicializar listas individuales si son null
                    for (int k = 0; k < responseCount; k++)
                    {
                        if (entry.eventsAssociatedPerResponse[k] == null)
                        {
                            entry.eventsAssociatedPerResponse[k] = new List<EventData>();
                        }
                        if (entry.objectsAssociatedPerResponse[k] == null)
                        {
                            entry.objectsAssociatedPerResponse[k] = new List<ObjectData>();
                        }
                    }

                    serializedObject.ApplyModifiedProperties();

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

                                        // CORREGIDO: Trabajar directamente con la lista
                                        List<EventData> eventsList = entry.eventsAssociatedPerResponse[j];

                                        if (GUILayout.Button("+ Añadir Evento"))
                                        {
                                            eventsList.Add(new EventData { eventIndex = 0, eventFlag = false });
                                            EditorUtility.SetDirty(target);
                                        }

                                        string[] nombresEventos = inventoryObject.eventos
                                            .Select(e => e != null ? e.nombre : "(Evento nulo)")
                                            .ToArray();

                                        for (int k = eventsList.Count - 1; k >= 0; k--)
                                        {
                                            EventData eventData = eventsList[k];

                                            int currentIndex = eventData.eventIndex;
                                            if (currentIndex < 0 || currentIndex >= nombresEventos.Length)
                                            {
                                                currentIndex = 0;
                                                eventData.eventIndex = 0;
                                            }

                                            EditorGUILayout.BeginHorizontal();
                                            
                                            int newIndex = EditorGUILayout.Popup(currentIndex, nombresEventos, GUILayout.Width(150));
                                            if (newIndex != currentIndex)
                                            {
                                                eventData.eventIndex = newIndex;
                                                EditorUtility.SetDirty(target);
                                            }

                                            bool newFlag = EditorGUILayout.Toggle(eventData.eventFlag);
                                            eventData.eventFlag = newFlag;
                                            EditorUtility.SetDirty(target);

                                            if (GUILayout.Button("✕", GUILayout.Width(25)))
                                            {
                                                eventsList.RemoveAt(k);
                                                EditorUtility.SetDirty(target);
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

                                        List<ObjectData> objectsList = entry.objectsAssociatedPerResponse[j];

                                        if (GUILayout.Button("+ Añadir Objeto"))
                                        {
                                            objectsList.Add(new ObjectData { objectIndex = 0, quantity = 0 });
                                            EditorUtility.SetDirty(target);
                                        }

                                        string[] nombresObjetos = inventoryObject.objetos.monedas
                                            .Select(e => e != null ? e.nombre : "(Objeto nulo)")
                                            .ToArray();
                                        nombresObjetos = Enumerable.Concat(nombresObjetos, inventoryObject.objetos.objetosClave
                                            .Select(e => e != null ? e.nombre : "(Objeto nulo)")
                                            .ToArray()).ToArray();

                                        for (int k = objectsList.Count - 1; k >= 0; k--)
                                        {
                                            ObjectData objectData = objectsList[k];

                                            int currentIndex = objectData.objectIndex;
                                            if (currentIndex < 0 || currentIndex >= nombresObjetos.Length)
                                            {
                                                currentIndex = 0;
                                                objectData.objectIndex = 0;
                                            }

                                            EditorGUILayout.BeginHorizontal();
                                            
                                            int newIndex = EditorGUILayout.Popup(currentIndex, nombresObjetos, GUILayout.Width(150));
                                            if (newIndex != currentIndex)
                                            {
                                                objectData.objectIndex = newIndex;
                                                EditorUtility.SetDirty(target);
                                            }

                                            int newQuantity = EditorGUILayout.IntField(0);
                                            objectData.quantity = newQuantity;
                                            EditorUtility.SetDirty(target);

                                            if (GUILayout.Button("✕", GUILayout.Width(25)))
                                            {
                                                objectsList.RemoveAt(k);
                                                EditorUtility.SetDirty(target);
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

                EditorGUILayout.EndVertical();
                EditorGUILayout.Space();
            }

            EditorGUI.indentLevel--;
        }

        serializedObject.ApplyModifiedProperties();
    }
}