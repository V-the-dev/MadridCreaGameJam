using Unity.VisualScripting;
using UnityEngine;

public abstract class InteractableObject : MonoBehaviour
{
    protected GameObject exclamation;
    protected MessageManager messageManager;

    public void NearestIndicator(bool activate)
    {
        exclamation.SetActive(activate);
    }

    public abstract void Trigger();

    //public abstract void AutoTrigger();

}
