using System;
using UnityEngine;

public class ParedScript : CharacterBaseScript
{
    public override void DeactivateCharacter()
    {
        gameObject.SetActive(false);
    }
}
