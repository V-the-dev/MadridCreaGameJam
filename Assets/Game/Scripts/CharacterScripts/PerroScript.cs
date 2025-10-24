using System;
using UnityEngine;

public class PerroScript : CharacterBaseScript
{
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    [SerializeField] private Sprite perroMuerto;
    [SerializeField] private Sprite perroFeliz;

    private void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public override void BucleReset()
    {
        base.BucleReset();
        animator.enabled = true;
        spriteRenderer.enabled = false;
    }

    public void DeadDog()
    {
        if (animator.enabled)
        {
            animator.enabled = false;
            spriteRenderer.enabled = true;
        }
        spriteRenderer.sprite = perroMuerto;
    }

    public void HappyDog()
    {
        if (animator.enabled)
        {
            animator.enabled = false;
            spriteRenderer.enabled = true;
        }
        spriteRenderer.sprite = perroFeliz;
    }
}
