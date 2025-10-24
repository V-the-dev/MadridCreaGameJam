using UnityEngine;

public class LadronScript : CharacterBaseScript
{
    public override void DeactivateCharacter()
    {
        gameObject.SetActive(false);
    }
}
