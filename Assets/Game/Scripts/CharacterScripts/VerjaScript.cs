using UnityEngine;

public class VerjaScript : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Sprite verjaCerrada;
    [SerializeField] private Sprite verjaAbierta;
    [SerializeField] private GameObject barrier;

    public void BucleReset()
    {
        spriteRenderer.sprite = verjaCerrada;
        barrier.SetActive(true);
    }
    
    private void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        verjaCerrada = spriteRenderer.sprite;
    }

    public void OpenPasadizo()
    {
        spriteRenderer.sprite = verjaAbierta;
        barrier.SetActive(false);
    }
}
