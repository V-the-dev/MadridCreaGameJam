using System;
using UnityEngine;

public class AmanteScript : CharacterBaseScript
{
    private Animator animator;
    private RuntimeAnimatorController firstAnimator;
    [SerializeField] private AnimatorOverrideController anotherAnimator;

    public override void BucleReset()
    {
        base.BucleReset();
        animator.runtimeAnimatorController = firstAnimator;
    }

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        firstAnimator = animator.runtimeAnimatorController;
    }

    public void ChangeAnimation()
    {
        animator.runtimeAnimatorController = anotherAnimator;
    }
}
