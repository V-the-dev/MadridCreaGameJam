using UnityEngine;

public class MendigoScript : CharacterBaseScript
{
    private Animator animator;
    private RuntimeAnimatorController firstAnimator;
    [SerializeField] private AnimatorOverrideController mendigoDeadAnimator;
    
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

    public void KillMendigo()
    {
        animator.runtimeAnimatorController = mendigoDeadAnimator;
    }
}
