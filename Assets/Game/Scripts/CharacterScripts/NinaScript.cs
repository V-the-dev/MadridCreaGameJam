using UnityEngine;

public class NinaScript : CharacterBaseScript
{
    public override void DeactivateCharacter()
    {
        gameObject.SetActive(false);
    }
}
