using UnityEngine;

public class VerjaScript : CharacterBaseScript
{
    [SerializeField] private SpriteRenderer verjaCerrada;
    [SerializeField] private SpriteRenderer verjaAbierta_Left;
    [SerializeField] private SpriteRenderer verjaAbierta_Right;
    [SerializeField] private GameObject barrier;

    public override void BucleReset()
    {
        base.BucleReset();
        verjaCerrada.enabled = true;
        verjaAbierta_Left.enabled = false;
        verjaAbierta_Right.enabled = false;
        barrier.SetActive(true);
    }
    
    private void Start()
    {
        verjaCerrada.enabled = true;
        verjaAbierta_Left.enabled = false;
        verjaAbierta_Right.enabled = false;
        barrier.SetActive(true);
    }

    public void OpenVerja()
    {
        verjaCerrada.enabled = false;
        verjaAbierta_Left.enabled = true;
        verjaAbierta_Right.enabled = true;
        barrier.SetActive(false);
    }
}
