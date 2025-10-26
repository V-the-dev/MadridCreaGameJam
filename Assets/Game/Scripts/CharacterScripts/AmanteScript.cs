using System;
using UnityEngine;

public class AmanteScript : CharacterBaseScript
{
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite amanteDecepcionado;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        animator.enabled = true;
    }

    public override void BucleReset()
    {
        base.BucleReset();
        animator.enabled = true;
    }

    public void ChangeAnimation()
    {
        animator.enabled = false;
        spriteRenderer.sprite = amanteDecepcionado;
    }
}
