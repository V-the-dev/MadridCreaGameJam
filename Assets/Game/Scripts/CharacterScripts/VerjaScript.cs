using UnityEngine;

public class VerjaScript : CharacterBaseScript
{
    private SpriteRenderer spriteRenderer;
    private Sprite verjaCerrada;
    [SerializeField] private Sprite verjaAbierta;
    [SerializeField] private GameObject barrier;

    public override void BucleReset()
    {
        base.BucleReset();
        spriteRenderer.sprite = verjaCerrada;
        barrier.SetActive(true);
    }
    
    private void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        verjaCerrada = spriteRenderer.sprite;
    }

    public void OpenVerja()
    {
        spriteRenderer.sprite = verjaAbierta;
        barrier.SetActive(false);
    }
}
