using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

[System.Serializable]
public class ResponseEvent
{
    [HideInInspector] public string name;

    [SerializeField, HideInInspector] private UnityEvent onPickedResponse;
    [SerializeField, HideInInspector] private List<CustomStringEvent> customStringEvents = new List<CustomStringEvent>();
    [SerializeField, HideInInspector] private List<CustomStringIntEvent> customStringIntEvents = new List<CustomStringIntEvent>();
    [SerializeField, HideInInspector] private List<CustomStringBoolEvent> customStringBoolEvents = new List<CustomStringBoolEvent>();

    [SerializeField, HideInInspector] private bool showVoidEvent;
    [SerializeField, HideInInspector] private bool showStringEvent;
    [SerializeField, HideInInspector] private bool showStringIntEvent;
    [SerializeField, HideInInspector] private bool showStringBoolEvent;

    public UnityEvent OnPickedResponse => onPickedResponse;

    public void Invoke()
    {
        if (showVoidEvent) 
            onPickedResponse?.Invoke();
        
        if (showStringEvent)
        {
            foreach (var evt in customStringEvents)
                evt.Invoke();
        }
        
        if (showStringIntEvent)
        {
            foreach (var evt in customStringIntEvents)
                evt.Invoke();
        }
    }
}

[System.Serializable]
public class CustomStringEvent
{
    public string stringValue = "";
    public UnityEvent<string> unityEvent = new UnityEvent<string>();

    public void Invoke()
    {
        unityEvent?.Invoke(stringValue);
    }
}

[System.Serializable]
public class CustomStringIntEvent
{
    public string stringValue = "";
    public int intValue = 0;
    public UnityEvent<string, int> unityEvent = new UnityEvent<string, int>();

    public void Invoke()
    {
        unityEvent?.Invoke(stringValue, intValue);
    }
}

[System.Serializable]
public class CustomStringBoolEvent
{
    public string stringValue = "";
    public bool boolValue = false;
    public UnityEvent<string, bool> unityEvent = new UnityEvent<string, bool>();

    public void Invoke()
    {
        unityEvent?.Invoke(stringValue, boolValue);
    }
}