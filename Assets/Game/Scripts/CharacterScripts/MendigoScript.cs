using UnityEngine;

public class MendigoScript : CharacterBaseScript
{
    private SpriteRenderer spriteRenderer;
    private Sprite firstSprite;
    [SerializeField] private Sprite mendigoDead;
    
    public override void BucleReset()
    {
        base.BucleReset();
        spriteRenderer.sprite = firstSprite;
    }

    private void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        firstSprite = spriteRenderer.sprite;
    }

    public void KillMendigo()
    {
        spriteRenderer.sprite = mendigoDead;
    }
}
