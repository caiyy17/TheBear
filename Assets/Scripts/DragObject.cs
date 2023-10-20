using System;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;

[ExecuteAlways]
public class DragObject : MonoBehaviour
{
    public string objectName;
    
    public Vector2 dragDir = new Vector2(1, 0);
    public float dragDistance = 0.1f;
    
    private Animator animator;  // Animator组件的引用
    private bool isMouseDown = false;
    private Vector3 mouseDownPosition;

    // 在这里定义动画的总帧数
    public int triggerFrame = 60;
    public int totalFrames = 60; // 假设动画有60帧
    private int currentFrame = 0; // 当前的帧

    public string aniName;
    public string aniNameR;
    
    void Start()
    {
        animator = GetComponent<Animator>();
        // 设置动画的播放模式
        animator.speed = 0; // 初始速度为0，这样我们可以手动控制动画的帧
    }
    
    private void OnMouseDown()
    {
        isMouseDown = true;
        mouseDownPosition = Camera.main.ScreenToViewportPoint(Input.mousePosition);
    }

    void Update()
    {
        if (isMouseDown)
        {
            animator.speed = 0;
            // 获取鼠标位置并计算对应的动画帧
            Vector3 diff = Camera.main.ScreenToViewportPoint(Input.mousePosition) - mouseDownPosition;
            float mousePosition = Vector2.Dot(diff, dragDir) / dragDistance;
            currentFrame = Mathf.FloorToInt(mousePosition * triggerFrame);
            //clamp between 0 and total frames
            currentFrame = Mathf.Clamp(currentFrame, 0, triggerFrame);
            Debug.Log(currentFrame);
            // 设置动画的播放时间（根据当前帧）
            animator.Play(aniName, -1, currentFrame / (float)totalFrames);
        }

        if (Input.GetMouseButtonUp(0))
        {
            // 当鼠标释放时
            isMouseDown = false;

            if (currentFrame >= triggerFrame)
            {
                // 如果超过阈值，则播放剩下的帧
                animator.speed = 1.0f; // 设置正常播放速度
                string _name;
                if (objectName == "")
                {
                    _name = gameObject.name;
                }
                else
                {
                    _name = objectName;
                }
                EventManager.Instance.TriggerEvent("Drag", _name);
            }
            else
            {
                animator.Play(aniNameR, -1, 1 - currentFrame / (float)totalFrames);
                animator.speed = 1.0f;
            }
        }
    }
}