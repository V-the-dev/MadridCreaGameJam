using System;
using UnityEngine;

public class CharacterChildScript : MonoBehaviour
{
    private void OnBecameInvisible()
    {
        GetComponentInParent<CharacterBaseScript>()?.OnBecameInvisible();
    }
}
