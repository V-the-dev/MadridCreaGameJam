using System;
using UnityEngine;

public class PerroScript : CharacterBaseScript
{
    private Animator animator;
    private RuntimeAnimatorController animatorController;
    private SpriteRenderer spriteRenderer;

    [SerializeField] private Sprite perroMuerto;
    [SerializeField] private AnimatorOverrideController perroFeliz;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        animatorController = animator.runtimeAnimatorController;
    }

    public override void BucleReset()
    {
        base.BucleReset();
        animator.enabled = true;
    }

    public void DeadDog()
    {
        if (animator.enabled)
        {
            animator.enabled = false;
        }
        spriteRenderer.sprite = perroMuerto;
    }

    public void HappyDog()
    {
        animator.runtimeAnimatorController = perroFeliz;
        if (animator.enabled)
        {
            animator.enabled = true;
        }
    }
}
