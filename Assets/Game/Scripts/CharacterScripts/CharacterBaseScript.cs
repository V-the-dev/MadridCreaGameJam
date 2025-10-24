using System;
using UnityEngine;

[Serializable]
public class CharacterBaseScript : MonoBehaviour
{
    private bool deactivateTrigger = false;
    
    public virtual void BucleReset()
    {
        deactivateTrigger = false;
        gameObject.SetActive(true);
    }

    public virtual void DeactivateCharacter()
    {
        deactivateTrigger = true;
    }

    private void OnBecameInvisible()
    {
        if (deactivateTrigger)
        {
            gameObject.SetActive(false);
        }
    }
}
