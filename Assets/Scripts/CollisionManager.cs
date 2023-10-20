using System;
using UnityEngine;
using System.Collections.Generic;

public class CollisionManager : MonoBehaviour
{
    // 单例实例
    public static CollisionManager Instance { get; private set; }

    // 存储所有碰撞信息的列表
    public List<string> Collisions { get; private set; }

    private void Awake()
    {
        // 确保只有一个实例存在
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        Collisions = new List<string>();
    }

    private void Update()
    {
        int state = 0;
        if (Input.GetMouseButton(0))
        {
            state = 1;
        }
        //用射线检测检测鼠标位置与box2d collider的碰撞
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
        if(hit.collider != null)
        {
            // Debug.Log(hit.collider.gameObject.name);
            HandleMouse(hit.collider.gameObject.name, state);
        }
        else
        {
            HandleMouse(null, state);
        }
        

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            // 通知 EventManager
            // Debug.Log("RightArrow");
            EventManager.Instance.TriggerEvent("Any", "Any");
        }
    }

    public void RegisterCollision(string objectName)
    {
        Collisions.Add(objectName);
    }

    public void UnregisterCollision(string objectName)
    {
        // 假设 Collision2DInfo 实现了相等性检查
        Collisions.Remove(objectName);
        // 通知 EventManager
        // Debug.Log($"物体{objectName}被点击了!");
        EventManager.Instance.TriggerEvent("Click", objectName);
    }
    
    public void HandleMouse(string objectName, int state)
    {
        EventManager.Instance.customCursor.HandleMouse(objectName, state);
    }
}

public struct Collision2DInfo
{
    public string objectName;

    public Collision2DInfo(string objectName)
    {
        this.objectName = objectName;
    }
}