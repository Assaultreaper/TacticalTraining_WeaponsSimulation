using UnityEngine;

public class AnimationController : MonoBehaviour
{
    public Animator animator;
    private static readonly int VelocityXHash = Animator.StringToHash("XVelocity");
    private static readonly int VelocityZHash = Animator.StringToHash("ZVelocity");
    private static readonly int isArmedHash = Animator.StringToHash("isArmed");

    public void UpdateLocomotionBlend(float VelocityX, float VelocityZ)
    {
        animator.SetFloat(VelocityXHash, VelocityX);
        animator.SetFloat(VelocityZHash, VelocityZ);
    }

    public void UpdateWeaponState(bool isArmed = false)
    {
        animator.SetBool(isArmedHash, isArmed);
    }
}
