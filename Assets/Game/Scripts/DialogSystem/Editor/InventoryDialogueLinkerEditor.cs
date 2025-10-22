using UnityEngine;
using UnityEditor;
using System.Linq;

[CustomEditor(typeof(InventoryDialogueLinker))]
public class InventoryDialogueLinkerEditor : Editor
{
    private SerializedProperty inventoryObjectProp = null;
    private SerializedProperty entriesProp = null;
    private bool showEntries = true;
    private bool showResponses = false;

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
                SerializedProperty eventIndexProp = entryProp.FindPropertyRelative("selectedEventIndex");
                SerializedProperty objectIndexProp = entryProp.FindPropertyRelative("selectedObjectIndex");
                SerializedProperty eventFlagProp = entryProp.FindPropertyRelative("eventFlag");
                SerializedProperty objectQuantityProp = entryProp.FindPropertyRelative("objectQuantity");
                SerializedProperty eventResponseAssociatedProp = entryProp.FindPropertyRelative("eventResponseAssociated");
                SerializedProperty objectResponseAssociatedProp = entryProp.FindPropertyRelative("objectResponseAssociated");
                SerializedProperty eventsAssociatedProp = entryProp.FindPropertyRelative("eventsAssociated");
                SerializedProperty objectsAssociatedProp = entryProp.FindPropertyRelative("objectsAssociated");
                SerializedProperty eventsFlagAssociatedProp = entryProp.FindPropertyRelative("eventsFlagAssociated");
                SerializedProperty objectsQuantityAssociatedProp = entryProp.FindPropertyRelative("objectsQuantityAssociated");

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

                // ---- EVENTO ASOCIADO ----
                if (inventoryObject)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.PropertyField(hasEventProp, new GUIContent("Evento asociado"));

                    if (hasEventProp.boolValue)
                    {
                        var eventos = inventoryObject.eventos != null
                            ? inventoryObject.eventos.Select(e => e.nombre).ToArray()
                            : new string[0];

                        int newIndex = EditorGUILayout.Popup("Evento", eventIndexProp.intValue, eventos);
                        if (newIndex != eventIndexProp.intValue)
                            eventIndexProp.intValue = newIndex;

                        EditorGUILayout.PropertyField(eventFlagProp, new GUIContent("Flag"));
                    }
                }
                
                // ---- OBJETO ASOCIADO ----
                if (inventoryObject)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.PropertyField(hasObjectProp, new GUIContent("Objeto asociado"));

                    if (hasObjectProp.boolValue)
                    {
                        var objetos = inventoryObject.objetos.monedas != null
                            ? inventoryObject.objetos.monedas.Select(e => e.nombre).ToArray()
                            : new string[0];
                        objetos = Enumerable.Concat(objetos, inventoryObject.objetos.objetosClave != null
                            ? inventoryObject.objetos.objetosClave.Select(e => e.nombre).ToArray()
                            : new string[0]).ToArray();

                        int newIndex = EditorGUILayout.Popup("Objeto", objectIndexProp.intValue, objetos);
                        if (newIndex != objectIndexProp.intValue)
                            objectIndexProp.intValue = newIndex;

                        EditorGUILayout.PropertyField(objectQuantityProp, new GUIContent("Value"));
                    }
                }

                // ---- RESPUESTAS ASOCIADAS ----
                
                if (dialogueObject)
                {
                    eventResponseAssociatedProp.arraySize = dialogueObject.Responses.Length;
                    objectResponseAssociatedProp.arraySize = dialogueObject.Responses.Length;
                    eventsAssociatedProp.arraySize = dialogueObject.Responses.Length;
                    objectsAssociatedProp.arraySize = dialogueObject.Responses.Length;
                    eventsFlagAssociatedProp.arraySize = dialogueObject.Responses.Length;
                    objectsQuantityAssociatedProp.arraySize = dialogueObject.Responses.Length;
                    serializedObject.ApplyModifiedProperties();

                    showResponses = EditorGUILayout.Foldout(showResponses, "Respuestas", true);
                    if (showResponses)
                    {
                        EditorGUI.indentLevel++;
                        for (int j = 0; j < dialogueObject.Responses.Length; j++)
                        {
                            SerializedProperty eventResponseVar = eventResponseAssociatedProp.GetArrayElementAtIndex(j);
                            SerializedProperty objectResponseVar = objectResponseAssociatedProp.GetArrayElementAtIndex(j);
                            SerializedProperty eventsVar = eventsAssociatedProp.GetArrayElementAtIndex(j);
                            SerializedProperty objectsVar = objectsAssociatedProp.GetArrayElementAtIndex(j);
                            SerializedProperty eventFlagVar = eventsFlagAssociatedProp.GetArrayElementAtIndex(j);
                            SerializedProperty objectQuantityVar = objectsQuantityAssociatedProp.GetArrayElementAtIndex(j);

                            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                            EditorGUILayout.LabelField(dialogueObject.Responses[j].ResponseText, EditorStyles.boldLabel);

                            EditorGUILayout.PropertyField(eventResponseVar, new GUIContent("¿Evento asociado?"));

                            if (eventResponseVar.boolValue)
                            {
                                if (inventoryObject && inventoryObject.eventos != null && inventoryObject.eventos.Count > 0)
                                {
                                    if (eventsAssociatedProp.arraySize != dialogueObject.Responses.Length)
                                    {
                                        eventsAssociatedProp.arraySize = dialogueObject.Responses.Length;
                                        eventsFlagAssociatedProp.arraySize = dialogueObject.Responses.Length;
                                    }

                                    string[] nombresEventos = inventoryObject.eventos
                                        .Select(e => e != null ? e.nombre : "(Evento nulo)")
                                        .ToArray();

                                    int currentIndex = eventsVar.intValue;
                                    if (currentIndex < 0 || currentIndex >= nombresEventos.Length)
                                        currentIndex = 0;

                                    EditorGUILayout.BeginHorizontal();
                                    int newIndex = EditorGUILayout.Popup(currentIndex, nombresEventos, GUILayout.Width(150));
                                    if (newIndex != currentIndex)
                                        eventsVar.intValue = newIndex;

                                    EditorGUILayout.PropertyField(eventFlagVar, GUIContent.none);
                                    EditorGUILayout.EndHorizontal();
                                }
                                else
                                {
                                    EditorGUILayout.LabelField("Sin eventos disponibles");
                                }
                            }

                            EditorGUILayout.Space(3); // ← Espaciado visual entre respuestas
                            
                            EditorGUILayout.PropertyField(objectResponseVar, new GUIContent("¿Objeto asociado?"));

                            if (objectResponseVar.boolValue)
                            {
                                if (inventoryObject && 
                                    inventoryObject.objetos.monedas != null && inventoryObject.eventos.Count > 0 &&
                                    inventoryObject.objetos.objetosClave != null && inventoryObject.objetos.objetosClave.Count > 0)
                                {
                                    if (objectsAssociatedProp.arraySize != dialogueObject.Responses.Length)
                                    {
                                        objectsAssociatedProp.arraySize = dialogueObject.Responses.Length;
                                        objectsQuantityAssociatedProp.arraySize = dialogueObject.Responses.Length;
                                    }

                                    string[] nombresObjetos = inventoryObject.objetos.monedas
                                        .Select(e => e != null ? e.nombre : "(Objeto nulo)")
                                        .ToArray();
                                    nombresObjetos = Enumerable.Concat(nombresObjetos, inventoryObject.objetos.objetosClave
                                        .Select(e => e != null ? e.nombre : "(Objeto nulo)")
                                        .ToArray()).ToArray();

                                    int currentIndex = objectsVar.intValue;
                                    if (currentIndex < 0 || currentIndex >= nombresObjetos.Length)
                                        currentIndex = 0;

                                    EditorGUILayout.BeginHorizontal();
                                    int newIndex = EditorGUILayout.Popup(currentIndex, nombresObjetos, GUILayout.Width(150));
                                    if (newIndex != currentIndex)
                                        objectsVar.intValue = newIndex;

                                    EditorGUILayout.PropertyField(objectQuantityVar, GUIContent.none);
                                    EditorGUILayout.EndHorizontal();
                                }
                                else
                                {
                                    EditorGUILayout.LabelField("Sin objetos disponibles");
                                }
                            }

                            EditorGUILayout.EndVertical();
                            EditorGUILayout.Space(3); // ← Espaciado visual entre respuestas
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
