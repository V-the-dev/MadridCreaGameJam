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
                SerializedProperty eventIndexProp = entryProp.FindPropertyRelative("selectedEventIndex");
                SerializedProperty eventFlagProp = entryProp.FindPropertyRelative("eventFlag");
                SerializedProperty eventResponseAssociatedProp = entryProp.FindPropertyRelative("eventResponseAssociated");
                SerializedProperty eventsAssociatedProp = entryProp.FindPropertyRelative("eventsAssociated");
                SerializedProperty eventsFlagAssociatedProp = entryProp.FindPropertyRelative("eventsFlagAssociated");

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

                // ---- RESPUESTAS ASOCIADAS ----
                
                if (dialogueObject)
                {
                    eventResponseAssociatedProp.arraySize = dialogueObject.Responses.Length;
                    eventsAssociatedProp.arraySize = dialogueObject.Responses.Length;
                    eventsFlagAssociatedProp.arraySize = dialogueObject.Responses.Length;
                    serializedObject.ApplyModifiedProperties();
                    showResponses = EditorGUILayout.Foldout(showResponses, "Respuestas", true);
                    if (showResponses)
                    {
                        EditorGUI.indentLevel++;
                        for (int j = 0; j < dialogueObject.Responses.Length; j++)
                        {
                            SerializedProperty eventResponseVar = eventResponseAssociatedProp.GetArrayElementAtIndex(j);
                            SerializedProperty eventsVar = eventsAssociatedProp.GetArrayElementAtIndex(j);
                            SerializedProperty eventFlagVar = eventsFlagAssociatedProp.GetArrayElementAtIndex(j);
                            
                            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField(dialogueObject.Responses[j].ResponseText, EditorStyles.boldLabel, GUILayout.Width(200));
                            EditorGUILayout.PropertyField(eventResponseVar, GUIContent.none, GUILayout.Width(50));
                            if (eventResponseVar.boolValue)
                            {
                                if (inventoryObject && inventoryObject.eventos != null && inventoryObject.eventos.Count > 0)
                                {
                                    // Aseguramos que tenga el mismo tamaño que las respuestas
                                    if (eventsAssociatedProp.arraySize != dialogueObject.Responses.Length)
                                    {
                                        eventsAssociatedProp.arraySize = dialogueObject.Responses.Length;
                                        eventsFlagAssociatedProp.arraySize = dialogueObject.Responses.Length;
                                    }

                                    // Construir lista de nombres
                                    string[] nombresEventos = inventoryObject.eventos
                                        .Select(e => e != null ? e.nombre : "(Evento nulo)")
                                        .ToArray();

                                    // Valor actual (índice)
                                    int currentIndex = eventsVar.intValue;
                                    if (currentIndex < 0 || currentIndex >= nombresEventos.Length)
                                        currentIndex = 0;

                                    // Mostrar popup con los nombres de eventos
                                    int newIndex = EditorGUILayout.Popup(currentIndex, nombresEventos, GUILayout.Width(150));

                                    // Si cambió la selección, guardarla
                                    if (newIndex != currentIndex)
                                    {
                                        eventsVar.intValue = newIndex;
                                    }
                                    
                                    EditorGUILayout.PropertyField(eventFlagVar, GUIContent.none,  GUILayout.Width(50));
                                }
                                else
                                {
                                    EditorGUILayout.LabelField("Sin eventos disponibles", GUILayout.Width(200));
                                }
                            }
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.EndVertical();
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
