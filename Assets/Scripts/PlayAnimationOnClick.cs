using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayAnimationOnClick : MonoBehaviour
{
    public string animationName;
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnMouseDown()
    {
        animator.Play(animationName);
    }
}