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

    public IEnumerator PlayAnimation(Animation animator, string animation, string defaultAnimation, float animationDuration)
    {
        if (isAnimationPlaying) yield break;
        isAnimationPlaying = true;
        animator.Play(animation);
        yield return new WaitForSeconds(animationDuration);
        animator.Play(defaultAnimation);
        isAnimationPlaying = false;
    }
}
