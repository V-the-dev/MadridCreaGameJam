using Unity.VisualScripting;
using UnityEngine;

public abstract class InteractableObject : MonoBehaviour
{
    [SerializeField] protected GameObject exclamation;

    public void NearestIndicator(bool activate)
    {
        exclamation.SetActive(activate);
    }

    public abstract void Trigger();

    public abstract void AutoTrigger();

}
