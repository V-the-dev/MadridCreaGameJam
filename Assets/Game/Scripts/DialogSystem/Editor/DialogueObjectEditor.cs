using UnityEngine;
using UnityEditor;
using System.Linq;

[CustomEditor(typeof(DialogueObject))]
public class DialogueObjectEditor : Editor
{
    private SerializedProperty charactersProp = null;
    private SerializedProperty charactersNameProp = null;
    private SerializedProperty charactersTitleProp = null;
    private SerializedProperty charactersSideProp = null;
    private SerializedProperty conversationLineProp = null;
    private SerializedProperty responsesProp = null;

    private void OnEnable()
    {
        charactersProp = serializedObject.FindProperty("characters");
        charactersNameProp = serializedObject.FindProperty("charactersName");
        charactersTitleProp = serializedObject.FindProperty("charactersTitle");
        charactersSideProp = serializedObject.FindProperty("charactersSide");
        conversationLineProp = serializedObject.FindProperty("conversationLine");
        responsesProp = serializedObject.FindProperty("responses");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Sección: Sprites y nombres
        EditorGUILayout.LabelField("Character Sprites", EditorStyles.boldLabel);

        for (int i = 0; i < charactersProp.arraySize; i++)
        {
            GUILayout.BeginHorizontal();

            SerializedProperty nameProp = charactersNameProp.GetArrayElementAtIndex(i);
            SerializedProperty titleProp = charactersTitleProp.GetArrayElementAtIndex(i);
            SerializedProperty spriteProp = charactersProp.GetArrayElementAtIndex(i);
            SerializedProperty sideProp = charactersSideProp.GetArrayElementAtIndex(i);

            // Campo para el nombre
            EditorGUILayout.PropertyField(nameProp, GUIContent.none, GUILayout.Width(100));

            // Campo para el titulo
            EditorGUILayout.PropertyField(titleProp, GUIContent.none, GUILayout.Width(100));
            
            // Campo para el sprite
            EditorGUILayout.PropertyField(spriteProp, GUIContent.none, GUILayout.Width(100));

            // Campo para el lado del sprite
            EditorGUILayout.PropertyField(sideProp, GUIContent.none, GUILayout.Width(75));

            // Botón para eliminar
            if (GUILayout.Button("-", GUILayout.Width(20)))
            {
                charactersNameProp.DeleteArrayElementAtIndex(i);
                charactersTitleProp.DeleteArrayElementAtIndex(i);
                charactersProp.DeleteArrayElementAtIndex(i);
                charactersSideProp.DeleteArrayElementAtIndex(i);
                GUILayout.EndHorizontal();
                serializedObject.ApplyModifiedProperties();
                return;
            }

            GUILayout.EndHorizontal();
        }

        // Botón para agregar un nuevo personaje
        if (GUILayout.Button("Add Character"))
        {
            charactersNameProp.arraySize++;
            charactersTitleProp.arraySize++;
            charactersProp.arraySize++;
            charactersSideProp.arraySize++;

            SerializedProperty newName = charactersNameProp.GetArrayElementAtIndex(charactersNameProp.arraySize - 1);
            SerializedProperty newTitle = charactersTitleProp.GetArrayElementAtIndex(charactersTitleProp.arraySize - 1);
            SerializedProperty newSprite = charactersProp.GetArrayElementAtIndex(charactersProp.arraySize - 1);
            SerializedProperty newSide = charactersSideProp.GetArrayElementAtIndex(charactersSideProp.arraySize - 1);

            newName.stringValue = "New Character";
            newTitle.stringValue = "New Title";
            newSprite.objectReferenceValue = null;
            newSide.enumValueIndex = 0; // Izquierda
        }

        EditorGUILayout.Space();

        // Sección: Conversation Lines
        EditorGUILayout.LabelField("Conversation Lines", EditorStyles.boldLabel);

        for (int i = 0; i < conversationLineProp.arraySize; i++)
        {
            SerializedProperty lineProp = conversationLineProp.GetArrayElementAtIndex(i);
            SerializedProperty dialogueProp = lineProp.FindPropertyRelative("dialogue");
            SerializedProperty spriteIndexProp = lineProp.FindPropertyRelative("spriteIndex");

            EditorGUILayout.LabelField($"Line {i + 1}", EditorStyles.boldLabel);

            // Text Area para el texto
            dialogueProp.stringValue = EditorGUILayout.TextArea(dialogueProp.stringValue, GUILayout.Height(60));

            // Popup para seleccionar el sprite
            if (charactersNameProp.arraySize > 0)
            {
                // Crear array de nombres para el popup
                string[] characterNames = new string[charactersNameProp.arraySize];
                for (int j = 0; j < charactersNameProp.arraySize; j++)
                {
                    characterNames[j] = charactersNameProp.GetArrayElementAtIndex(j).stringValue;
                }

                spriteIndexProp.intValue = EditorGUILayout.Popup(
                    "Character",
                    spriteIndexProp.intValue,
                    characterNames
                );
            }
            else
            {
                EditorGUILayout.HelpBox("Add characters first", MessageType.Warning);
            }

            // Botón para eliminar esta línea
            if (GUILayout.Button("Remove This Line"))
            {
                conversationLineProp.DeleteArrayElementAtIndex(i);
                serializedObject.ApplyModifiedProperties();
                return;
            }

            EditorGUILayout.Space();
        }

        if (GUILayout.Button("Add Conversation Line"))
        {
            conversationLineProp.arraySize++;
            SerializedProperty newLine = conversationLineProp.GetArrayElementAtIndex(conversationLineProp.arraySize - 1);
            SerializedProperty newDialogue = newLine.FindPropertyRelative("dialogue");
            SerializedProperty newSpriteIndex = newLine.FindPropertyRelative("spriteIndex");
            
            newDialogue.stringValue = "";
            newSpriteIndex.intValue = 0;
        }

        EditorGUILayout.Space();

        // Sección: Responses
        EditorGUILayout.LabelField("Responses", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(responsesProp, true);

        serializedObject.ApplyModifiedProperties();
    }
}