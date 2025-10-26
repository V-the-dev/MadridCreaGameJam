using System;
using UnityEngine;

public class AgenteScript : CharacterBaseScript
{
    public override void DeactivateCharacter()
    {
        gameObject.SetActive(false);
    }
}
