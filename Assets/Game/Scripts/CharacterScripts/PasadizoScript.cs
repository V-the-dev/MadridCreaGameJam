using UnityEngine;

public class PasadizoScript : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Sprite pasadizoCerrado;
    [SerializeField] private Sprite pasadizoAbierto;
    [SerializeField] private GameObject barrier;

    public void BucleReset()
    {
        spriteRenderer.sprite = pasadizoCerrado;
        barrier.SetActive(true);
    }
    
    private void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        pasadizoCerrado = spriteRenderer.sprite;
    }

    public void OpenPasadizo()
    {
        spriteRenderer.sprite = pasadizoAbierto;
        barrier.SetActive(false);
    }
}
