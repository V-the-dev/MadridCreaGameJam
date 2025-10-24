using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DialogueResponseEvents))]
public class DialogueResponseEventsEditor : Editor
{
    private SerializedProperty drEventsProp;

    private void OnEnable()
    {
        drEventsProp = serializedObject.FindProperty("DREvents");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DialogueResponseEvents responseEvents = (DialogueResponseEvents)target;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Dialogue Response Events", EditorStyles.boldLabel);

        // Botón de Refresh manual
        if (GUILayout.Button("Refresh Events"))
        {
            responseEvents.OnValidate();
            EditorUtility.SetDirty(responseEvents);
        }

        EditorGUILayout.Space();

        // Lista personalizada
        for (int i = 0; i < drEventsProp.arraySize; i++)
        {
            SerializedProperty element = drEventsProp.GetArrayElementAtIndex(i);
            SerializedProperty dialogueObjectProp = element.FindPropertyRelative("dialogueObject");
            SerializedProperty eventsProp = element.FindPropertyRelative("events");

            DialogueObject dialogueObj = dialogueObjectProp.objectReferenceValue as DialogueObject;

            // Nombre visible del elemento
            string elementName = dialogueObj != null ? dialogueObj.name : $"Elemento {i + 1} (sin DialogueObject)";

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.PropertyField(dialogueObjectProp, new GUIContent($"🎭 {elementName}"));

            // Mostrar eventos solo si hay un DialogueObject asignado
            if (dialogueObj != null)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(eventsProp, new GUIContent("Eventos"), true);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }

        // Botón para añadir nuevo elemento
        if (GUILayout.Button("+ Añadir nuevo Dialogue Event"))
        {
            drEventsProp.InsertArrayElementAtIndex(drEventsProp.arraySize);

            SerializedProperty newElement = drEventsProp.GetArrayElementAtIndex(drEventsProp.arraySize - 1);

            // Inicializa a valores por defecto
            SerializedProperty dialogueObjectProp = newElement.FindPropertyRelative("dialogueObject");
            SerializedProperty eventsProp = newElement.FindPropertyRelative("events");

            dialogueObjectProp.objectReferenceValue = null;
            eventsProp.ClearArray(); // Limpia cualquier referencia previa

            // Aplica cambios
            serializedObject.ApplyModifiedProperties();
            Repaint();
        }

        serializedObject.ApplyModifiedProperties();
    }
}
