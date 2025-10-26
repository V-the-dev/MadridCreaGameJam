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
        if (!IsVisibleFrom(Camera.main))
        {
            gameObject.SetActive(false);
        }
        else
        {
            deactivateTrigger = true;
        }
    }

    public virtual void OnBecameInvisible()
    {
        if (deactivateTrigger)
        {
            gameObject.SetActive(false);
        }
    }
    
    public bool IsVisibleFrom(Camera camera)
    {
        bool result = true;
        SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>();
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
        foreach (SpriteRenderer renderer in renderers)
        {
            if (!GeometryUtility.TestPlanesAABB(planes, renderer.bounds))
            {
                result = false;
            }
        }

        return result;
    }
}
