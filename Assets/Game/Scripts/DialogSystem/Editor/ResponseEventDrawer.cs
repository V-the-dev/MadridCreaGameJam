using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ResponseEvent))]
public class ResponseEventDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (!property.isExpanded)
            return EditorGUIUtility.singleLineHeight + 4;

        float height = EditorGUIUtility.singleLineHeight * 2 + 6;

        if (property.FindPropertyRelative("showVoidEvent").boolValue)
        {
            height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("onPickedResponse"), true) + 4;
        }

        if (property.FindPropertyRelative("showStringEvent").boolValue)
        {
            var stringEvents = property.FindPropertyRelative("customStringEvents");
            for (int i = 0; i < stringEvents.arraySize; i++)
            {
                var element = stringEvents.GetArrayElementAtIndex(i);
                height += EditorGUIUtility.singleLineHeight + 4; // String field
                height += EditorGUI.GetPropertyHeight(element.FindPropertyRelative("unityEvent"), true) + 4;
                height += EditorGUIUtility.singleLineHeight + 4; // Remove button
            }
            height += EditorGUIUtility.singleLineHeight + 4; // Add button
        }

        if (property.FindPropertyRelative("showStringIntEvent").boolValue)
        {
            var stringIntEvents = property.FindPropertyRelative("customStringIntEvents");
            for (int i = 0; i < stringIntEvents.arraySize; i++)
            {
                var element = stringIntEvents.GetArrayElementAtIndex(i);
                height += EditorGUIUtility.singleLineHeight * 2 + 8; // String + Int fields
                height += EditorGUI.GetPropertyHeight(element.FindPropertyRelative("unityEvent"), true) + 4;
                height += EditorGUIUtility.singleLineHeight + 4; // Remove button
            }
            height += EditorGUIUtility.singleLineHeight + 4; // Add button
        }

        return height;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        var nameProp = property.FindPropertyRelative("name");
        var showVoidProp = property.FindPropertyRelative("showVoidEvent");
        var showStringProp = property.FindPropertyRelative("showStringEvent");
        var showStringIntProp = property.FindPropertyRelative("showStringIntEvent");

        position.y += 2;

        // Foldout
        property.isExpanded = EditorGUI.Foldout(
            new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight),
            property.isExpanded,
            string.IsNullOrEmpty(nameProp.stringValue) ? "Response Event" : nameProp.stringValue,
            true
        );

        if (!property.isExpanded)
        {
            EditorGUI.EndProperty();
            return;
        }

        EditorGUI.indentLevel++;

        // Dropdown button
        Rect addButtonRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + 2, 
            position.width, EditorGUIUtility.singleLineHeight);
        if (EditorGUI.DropdownButton(addButtonRect, new GUIContent("+ Add Event Type"), FocusType.Keyboard))
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("void Foo()"), showVoidProp.boolValue, 
                () => ToggleEventType(property, "showVoidEvent"));
            menu.AddItem(new GUIContent("void Foo(string)"), showStringProp.boolValue, 
                () => ToggleEventType(property, "showStringEvent"));
            menu.AddItem(new GUIContent("void Foo(string, int)"), showStringIntProp.boolValue, 
                () => ToggleEventType(property, "showStringIntEvent"));
            menu.ShowAsContext();
        }

        float y = addButtonRect.y + EditorGUIUtility.singleLineHeight + 4;

        // Void events
        if (showVoidProp.boolValue)
        {
            SerializedProperty evt = property.FindPropertyRelative("onPickedResponse");
            float h = EditorGUI.GetPropertyHeight(evt, true);
            EditorGUI.PropertyField(new Rect(position.x, y, position.width, h), evt, 
                new GUIContent("On Picked Response ()"), true);
            y += h + 4;
        }

        // String events
        if (showStringProp.boolValue)
        {
            var stringEvents = property.FindPropertyRelative("customStringEvents");
            
            for (int i = 0; i < stringEvents.arraySize; i++)
            {
                var element = stringEvents.GetArrayElementAtIndex(i);
                var stringValue = element.FindPropertyRelative("stringValue");
                var unityEvent = element.FindPropertyRelative("unityEvent");

                // String parameter field
                EditorGUI.PropertyField(new Rect(position.x, y, position.width, EditorGUIUtility.singleLineHeight),
                    stringValue, new GUIContent("Nombre"));
                y += EditorGUIUtility.singleLineHeight + 4;

                // Unity event
                float h = EditorGUI.GetPropertyHeight(unityEvent, true);
                EditorGUI.PropertyField(new Rect(position.x, y, position.width, h), unityEvent, 
                    new GUIContent($"Event (string) [{i}]"), true);
                y += h + 4;

                // Remove button
                if (GUI.Button(new Rect(position.x, y, position.width, EditorGUIUtility.singleLineHeight), 
                    $"Remove Event [{i}]"))
                {
                    stringEvents.DeleteArrayElementAtIndex(i);
                    property.serializedObject.ApplyModifiedProperties();
                    break;
                }
                y += EditorGUIUtility.singleLineHeight + 4;
            }

            // Add new event button
            if (GUI.Button(new Rect(position.x, y, position.width, EditorGUIUtility.singleLineHeight), 
                "+ Add String Event"))
            {
                stringEvents.arraySize++;
                property.serializedObject.ApplyModifiedProperties();
            }
            y += EditorGUIUtility.singleLineHeight + 4;
        }

        // String + Int events
        if (showStringIntProp.boolValue)
        {
            var stringIntEvents = property.FindPropertyRelative("customStringIntEvents");
            
            for (int i = 0; i < stringIntEvents.arraySize; i++)
            {
                var element = stringIntEvents.GetArrayElementAtIndex(i);
                var stringValue = element.FindPropertyRelative("stringValue");
                var intValue = element.FindPropertyRelative("intValue");
                var unityEvent = element.FindPropertyRelative("unityEvent");

                // String parameter field
                EditorGUI.PropertyField(new Rect(position.x, y, position.width, EditorGUIUtility.singleLineHeight),
                    stringValue, new GUIContent("Nombre"));
                y += EditorGUIUtility.singleLineHeight + 4;

                // Int parameter field
                EditorGUI.PropertyField(new Rect(position.x, y, position.width, EditorGUIUtility.singleLineHeight),
                    intValue, new GUIContent("Valor"));
                y += EditorGUIUtility.singleLineHeight + 4;

                // Unity event
                float h = EditorGUI.GetPropertyHeight(unityEvent, true);
                EditorGUI.PropertyField(new Rect(position.x, y, position.width, h), unityEvent, 
                    new GUIContent($"Event (string, int) [{i}]"), true);
                y += h + 4;

                // Remove button
                if (GUI.Button(new Rect(position.x, y, position.width, EditorGUIUtility.singleLineHeight), 
                    $"Remove Event [{i}]"))
                {
                    stringIntEvents.DeleteArrayElementAtIndex(i);
                    property.serializedObject.ApplyModifiedProperties();
                    break;
                }
                y += EditorGUIUtility.singleLineHeight + 4;
            }

            // Add new event button
            if (GUI.Button(new Rect(position.x, y, position.width, EditorGUIUtility.singleLineHeight), 
                "+ Add String-Int Event"))
            {
                stringIntEvents.arraySize++;
                property.serializedObject.ApplyModifiedProperties();
            }
            y += EditorGUIUtility.singleLineHeight + 4;
        }

        EditorGUI.indentLevel--;
        EditorGUI.EndProperty();
    }

    private void ToggleEventType(SerializedProperty property, string flagName)
    {
        var flag = property.FindPropertyRelative(flagName);
        bool newValue = !flag.boolValue;
        flag.boolValue = newValue;

        if (!newValue)
        {
            if (flagName == "showVoidEvent")
            {
                SerializedProperty eventProp = property.FindPropertyRelative("onPickedResponse");
                eventProp.FindPropertyRelative("m_PersistentCalls.m_Calls").ClearArray();
            }
            else if (flagName == "showStringEvent")
            {
                property.FindPropertyRelative("customStringEvents").ClearArray();
            }
            else if (flagName == "showStringIntEvent")
            {
                property.FindPropertyRelative("customStringIntEvents").ClearArray();
            }
        }

        property.serializedObject.ApplyModifiedProperties();
    }
}