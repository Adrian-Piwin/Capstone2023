using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationPlayer : MonoBehaviour
{
    public bool isAnimationPlaying;

    public static AnimationPlayer Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    public IEnumerator PlayAnimation(Animator animator, string animation, bool overwrite = false)
    {
        if (isAnimationPlaying && !overwrite) yield break;
        isAnimationPlaying = true;
        animator.Play(animation);
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        isAnimationPlaying = false;
    }
}
