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
        SpriteRenderer renderer = GetComponentInChildren<SpriteRenderer>();
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
        return GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
    }
}
