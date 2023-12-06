using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickStandalone : MonoBehaviour
{
    public string aniName;
    private Animator animator;  // Animator组件的引用
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnMouseDown()
    {
        animator.Play(aniName);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
