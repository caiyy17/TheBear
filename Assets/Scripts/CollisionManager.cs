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
        //检测方向键右
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            // 通知 LuaEventManager
            LuaEventManager.Instance.TriggerEvent("Any", "Any");
        }
    }

    public void RegisterCollision(string objectName)
    {
        Collisions.Add(objectName);
        // Debug.Log($"物体{objectName}被点击了!");
        // 通知 LuaEventManager
        LuaEventManager.Instance.TriggerEvent("Click", objectName);
    }

    public void UnregisterCollision(string objectName)
    {
        // 假设 Collision2DInfo 实现了相等性检查
        Collisions.Remove(objectName);
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