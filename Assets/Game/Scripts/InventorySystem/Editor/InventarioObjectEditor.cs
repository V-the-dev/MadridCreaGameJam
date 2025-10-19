using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(InventarioObject))]
public class InventarioObjectEditor : Editor
{
    private SerializedProperty objetosProp;
    private SerializedProperty eventosProp;
    
    private bool mostrarMonedas = true;
    private bool mostrarEstados = true;
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
        EditorGUILayout.LabelField("═══════════ OBJETOS ═══════════", EditorStyles.boldLabel);
        
        // Monedas
        DibujarSeccionMonedas();
        EditorGUILayout.Space(10);

        // Estados
        DibujarSeccionEstados();
        EditorGUILayout.Space(10);

        // Objetos Clave
        DibujarSeccionObjetosClave();
        EditorGUILayout.Space(15);

        // EVENTOS
        EditorGUILayout.LabelField("═══════════ EVENTOS ═══════════", EditorStyles.boldLabel);
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
                SerializedProperty valor = moneda.FindPropertyRelative("valor");

                EditorGUILayout.LabelField($"#{i}", GUILayout.Width(30));
                EditorGUILayout.PropertyField(nombre, GUIContent.none, GUILayout.Width(200));
                EditorGUILayout.PropertyField(valor, GUIContent.none, GUILayout.Width(80));

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

    private void DibujarSeccionEstados()
    {
        SerializedProperty estadosProp = objetosProp.FindPropertyRelative("estados");
        
        mostrarEstados = EditorGUILayout.Foldout(mostrarEstados, $"Estados ({estadosProp.arraySize})", true, EditorStyles.foldoutHeader);
        
        if (mostrarEstados)
        {
            EditorGUI.indentLevel++;
            
            if (GUILayout.Button("+ Añadir Estado"))
            {
                estadosProp.InsertArrayElementAtIndex(estadosProp.arraySize);
            }

            for (int i = 0; i < estadosProp.arraySize; i++)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.BeginHorizontal();
                
                SerializedProperty estado = estadosProp.GetArrayElementAtIndex(i);
                SerializedProperty nombre = estado.FindPropertyRelative("nombre");
                SerializedProperty valor = estado.FindPropertyRelative("valor");

                EditorGUILayout.LabelField($"#{i}", GUILayout.Width(30));
                EditorGUILayout.PropertyField(nombre, GUIContent.none, GUILayout.Width(200));
                EditorGUILayout.PropertyField(valor, GUIContent.none, GUILayout.Width(120));

                if (GUILayout.Button("×", GUILayout.Width(25)))
                {
                    estadosProp.DeleteArrayElementAtIndex(i);
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
                SerializedProperty valor = objetoClave.FindPropertyRelative("valor");

                EditorGUILayout.LabelField($"#{i}", GUILayout.Width(30));
                EditorGUILayout.PropertyField(nombre, GUIContent.none, GUILayout.Width(200));
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
                SerializedProperty diccionario = nuevoEvento.FindPropertyRelative("diccionario");
                // Asegurar que el nuevo evento tenga exactamente un elemento en el diccionario
                diccionario.ClearArray();
                diccionario.InsertArrayElementAtIndex(0);
                eventosExpandidos[eventosProp.arraySize - 1] = true;
            }

            for (int i = 0; i < eventosProp.arraySize; i++)
            {
                SerializedProperty evento = eventosProp.GetArrayElementAtIndex(i);
                SerializedProperty diccionario = evento.FindPropertyRelative("diccionario");

                // Asegurar que siempre haya exactamente un elemento
                if (diccionario.arraySize == 0)
                {
                    diccionario.InsertArrayElementAtIndex(0);
                }
                else if (diccionario.arraySize > 1)
                {
                    while (diccionario.arraySize > 1)
                    {
                        diccionario.DeleteArrayElementAtIndex(diccionario.arraySize - 1);
                    }
                }

                // Obtener el nombre del diccionario para usarlo como título
                string nombreEvento = "Evento sin nombre";
                if (diccionario.arraySize > 0)
                {
                    SerializedProperty elemento = diccionario.GetArrayElementAtIndex(0);
                    SerializedProperty nombre = elemento.FindPropertyRelative("nombre");
                    if (!string.IsNullOrEmpty(nombre.stringValue))
                    {
                        nombreEvento = nombre.stringValue;
                    }
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

                    EditorGUILayout.PropertyField(isLoopPersistent, new GUIContent("Is Loop Persistent"));
                    EditorGUILayout.PropertyField(startValue, new GUIContent("Start Value"));

                    EditorGUILayout.Space(5);

                    // Mostrar el único elemento del diccionario
                    SerializedProperty elemento = diccionario.GetArrayElementAtIndex(0);
                    SerializedProperty nombreDic = elemento.FindPropertyRelative("nombre");
                    SerializedProperty valorDic = elemento.FindPropertyRelative("valor");

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Nombre:", GUILayout.Width(60));
                    EditorGUILayout.PropertyField(nombreDic, GUIContent.none);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Valor:", GUILayout.Width(60));
                    EditorGUILayout.PropertyField(valorDic, GUIContent.none);
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.EndVertical();
                EditorGUILayout.Space(5);
            }
            
            EditorGUI.indentLevel--;
        }
    }
}