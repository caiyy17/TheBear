using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayAnimationOnHover : MonoBehaviour
{
    public string animationName;
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnMouseEnter()
    {
        animator.Play(animationName);
    }
    
    private void OnMouseExit()
    {
        animator.Play("New State");
    }
}