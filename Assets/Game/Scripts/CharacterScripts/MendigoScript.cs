using UnityEngine;

public class MendigoScript : CharacterBaseScript
{
    private SpriteRenderer spriteRenderer;
    private Sprite firstSprite;
    [SerializeField] private Sprite mendigoDead;
    private bool isDead = false;
    
    public override void BucleReset()
    {
        base.BucleReset();
        spriteRenderer.sprite = firstSprite;
        isDead = false;
    }

    private void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        firstSprite = spriteRenderer.sprite;
    }

    public override void OnBecameInvisible()
    {
        base.OnBecameInvisible();
        if (isDead)
        {
            spriteRenderer.sprite = mendigoDead;
            InventoryManager.Instance.SetEventValue("Mendigo_muerto", true);
        }
    }

    public void KillMendigo()
    {
        isDead = true;
    }
}
