using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class DialogueResponseEventType
{
    public DialogueObject dialogueObject = null;
    public ResponseEvent[] events;

    public DialogueObject DialogueObject => dialogueObject;
    public ResponseEvent[] Events => events;
}

public class DialogueResponseEvents : MonoBehaviour
{
    public List<DialogueResponseEventType> DREvents = new List<DialogueResponseEventType>();

    public void OnValidate()
    {
        foreach (DialogueResponseEventType drEvent in DREvents)
        {
            if (!drEvent.dialogueObject)
                continue;
            if (drEvent.dialogueObject.Responses == null)
                continue;
            if (drEvent.events != null && drEvent.events.Length == drEvent.dialogueObject.Responses.Length + 1)
                continue;

            if (drEvent.events == null)
                drEvent.events = new ResponseEvent[drEvent.dialogueObject.Responses.Length + 1];
            else
                Array.Resize(ref drEvent.events, drEvent.dialogueObject.Responses.Length + 1);

            for (int i = 0; i < drEvent.dialogueObject.Responses.Length + 1; ++i)
            {
                if (i == drEvent.dialogueObject.Responses.Length)
                {
                    if (drEvent.events[i] == null)
                        drEvent.events[i] = new ResponseEvent() { name = "END_CONVERSATION" };
                    else
                        drEvent.events[i].name = "END_CONVERSATION";
                    continue;
                }

                Response response = drEvent.dialogueObject.Responses[i];
                if (drEvent.events[i] == null)
                    drEvent.events[i] = new ResponseEvent() { name = response.ResponseText };
                else
                    drEvent.events[i].name = response.ResponseText;
            }
        }
    }
}