using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(InventarioObject))]
public class InventarioObjectEditor : Editor
{
    private SerializedProperty objetosProp;
    private SerializedProperty eventosProp;
    
    private bool mostrarMonedas = true;
    private bool mostrarObjetosClave = true;
    private bool mostrarEventos = true;
    private Dictionary<int, bool> eventosExpandidos = new Dictionary<int, bool>();

    private void OnEnable()
    {
        objetosProp = serializedObject.FindProperty("objetos");
        eventosProp = serializedObject.FindProperty("eventos");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("INVENTARIO", EditorStyles.boldLabel);
        EditorGUILayout.Space(5);

        // OBJETOS
        EditorGUILayout.LabelField("════════════════ OBJETOS ════════════════", EditorStyles.boldLabel);
        
        // Monedas
        DibujarSeccionMonedas();
        EditorGUILayout.Space(10);

        // Objetos Clave
        DibujarSeccionObjetosClave();
        EditorGUILayout.Space(15);

        // EVENTOS
        EditorGUILayout.LabelField("════════════════ EVENTOS ════════════════", EditorStyles.boldLabel);
        DibujarSeccionEventos();

        serializedObject.ApplyModifiedProperties();
    }

    private void DibujarSeccionMonedas()
    {
        SerializedProperty monedasProp = objetosProp.FindPropertyRelative("monedas");
        
        mostrarMonedas = EditorGUILayout.Foldout(mostrarMonedas, $"Monedas ({monedasProp.arraySize})", true, EditorStyles.foldoutHeader);
        
        if (mostrarMonedas)
        {
            EditorGUI.indentLevel++;
            
            // Botón para añadir
            if (GUILayout.Button("+ Añadir Moneda"))
            {
                monedasProp.InsertArrayElementAtIndex(monedasProp.arraySize);
            }

            for (int i = 0; i < monedasProp.arraySize; i++)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.BeginHorizontal();
                
                SerializedProperty moneda = monedasProp.GetArrayElementAtIndex(i);
                SerializedProperty nombre = moneda.FindPropertyRelative("nombre");
                SerializedProperty sprite = moneda.FindPropertyRelative("sprite");
                SerializedProperty valor = moneda.FindPropertyRelative("valor");

                EditorGUILayout.LabelField($"#{i}", GUILayout.Width(30));
                EditorGUILayout.PropertyField(nombre, GUIContent.none, GUILayout.Width(150));
                EditorGUILayout.PropertyField(sprite, GUIContent.none, GUILayout.Width(130));
                EditorGUILayout.PropertyField(valor, GUIContent.none, GUILayout.Width(120));

                if (GUILayout.Button("×", GUILayout.Width(25)))
                {
                    monedasProp.DeleteArrayElementAtIndex(i);
                }

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
            }
            
            EditorGUI.indentLevel--;
        }
    }

    private void DibujarSeccionObjetosClave()
    {
        SerializedProperty objetosClaveProp = objetosProp.FindPropertyRelative("objetosClave");
        
        mostrarObjetosClave = EditorGUILayout.Foldout(mostrarObjetosClave, $"Objetos Clave ({objetosClaveProp.arraySize})", true, EditorStyles.foldoutHeader);
        
        if (mostrarObjetosClave)
        {
            EditorGUI.indentLevel++;
            
            if (GUILayout.Button("+ Añadir Objeto Clave"))
            {
                objetosClaveProp.InsertArrayElementAtIndex(objetosClaveProp.arraySize);
            }

            for (int i = 0; i < objetosClaveProp.arraySize; i++)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.BeginHorizontal();
                
                SerializedProperty objetoClave = objetosClaveProp.GetArrayElementAtIndex(i);
                SerializedProperty nombre = objetoClave.FindPropertyRelative("nombre");
                SerializedProperty sprite = objetoClave.FindPropertyRelative("sprite");
                SerializedProperty valor = objetoClave.FindPropertyRelative("valor");

                EditorGUILayout.LabelField($"#{i}", GUILayout.Width(30));
                EditorGUILayout.PropertyField(nombre, GUIContent.none, GUILayout.Width(150));
                EditorGUILayout.PropertyField(sprite, GUIContent.none, GUILayout.Width(130));
                EditorGUILayout.PropertyField(valor, GUIContent.none, GUILayout.Width(120));

                if (GUILayout.Button("×", GUILayout.Width(25)))
                {
                    objetosClaveProp.DeleteArrayElementAtIndex(i);
                }

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
            }
            
            EditorGUI.indentLevel--;
        }
    }

    private void DibujarSeccionEventos()
    {
        mostrarEventos = EditorGUILayout.Foldout(mostrarEventos, $"Lista de Eventos ({eventosProp.arraySize})", true, EditorStyles.foldoutHeader);
        
        if (mostrarEventos)
        {
            EditorGUI.indentLevel++;
            
            if (GUILayout.Button("+ Añadir Evento"))
            {
                eventosProp.InsertArrayElementAtIndex(eventosProp.arraySize);
                SerializedProperty nuevoEvento = eventosProp.GetArrayElementAtIndex(eventosProp.arraySize - 1);
                SerializedProperty nombre = nuevoEvento.FindPropertyRelative("nombre");
                eventosExpandidos[eventosProp.arraySize - 1] = true;
            }

            for (int i = 0; i < eventosProp.arraySize; i++)
            {
                SerializedProperty evento = eventosProp.GetArrayElementAtIndex(i);

                // Obtener el nombre para usarlo como título
                string nombreEvento = "Evento sin nombre";
                SerializedProperty nombre = evento.FindPropertyRelative("nombre");
                if (!string.IsNullOrEmpty(nombre.stringValue))
                {
                    nombreEvento = nombre.stringValue;
                }

                // Asegurar que existe la entrada en el diccionario
                if (!eventosExpandidos.ContainsKey(i))
                {
                    eventosExpandidos[i] = false;
                }

                EditorGUILayout.BeginVertical(GUI.skin.box);
                
                EditorGUILayout.BeginHorizontal();
                
                // Foldout para plegar/desplegar
                eventosExpandidos[i] = EditorGUILayout.Foldout(eventosExpandidos[i], nombreEvento, true);
                
                if (GUILayout.Button("×", GUILayout.Width(25)))
                {
                    eventosProp.DeleteArrayElementAtIndex(i);
                    eventosExpandidos.Remove(i);
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndVertical();
                    break;
                }
                EditorGUILayout.EndHorizontal();

                // Mostrar contenido solo si está expandido
                if (eventosExpandidos[i])
                {
                    SerializedProperty isLoopPersistent = evento.FindPropertyRelative("isLoopPersistent");
                    SerializedProperty startValue = evento.FindPropertyRelative("startValue");

                    // Mostrar el único elemento del diccionario
                    SerializedProperty nombreDic = evento.FindPropertyRelative("nombre");

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Nombre:", GUILayout.Width(60));
                    EditorGUILayout.PropertyField(nombreDic, GUIContent.none);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.Space(5);

                    EditorGUILayout.PropertyField(isLoopPersistent, new GUIContent("Is Loop Persistent"));
                    EditorGUILayout.PropertyField(startValue, new GUIContent("Start Value"));
                }

                EditorGUILayout.EndVertical();
                EditorGUILayout.Space(5);
            }
            
            EditorGUI.indentLevel--;
        }
    }
}