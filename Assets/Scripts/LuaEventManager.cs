using System;
using UnityEngine;
using XLua;
using System.Collections.Generic;

using DG.Tweening;

public class LuaEventManager : MonoBehaviour
{
    public static LuaEventManager Instance { get; private set; }
    
    public TextAsset EventConfig;
    public TextAsset EventsTree;
    private LuaEnv luaEnv = new LuaEnv();
    
    private Dictionary<string, LuaTable> events = new Dictionary<string, LuaTable>();
    private List<LuaTable> currentEvents = new List<LuaTable>();
    private List<LuaTable> currentActions = new List<LuaTable>();
    private float timer = 0;

    void Awake()
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
    }
    
    void Start()
    {
        events = new Dictionary<string, LuaTable>();
        currentEvents = new List<LuaTable>();
        currentActions = new List<LuaTable>();
        
        DOTween.Init();
        if(EventConfig != null)
        {
            luaEnv.DoString(EventConfig.text);
            var func = luaEnv.Global.Get<Action<LuaEventManager>>("SetupEvents");
            func?.Invoke(this);
        }
        if (EventsTree != null)
        {
            luaEnv.DoString(EventsTree.text);
            LuaTable allEvents = luaEnv.Global.Get<LuaTable>("EventsTree");
            for (int j = 0; j < allEvents.Length; j++)
            {
                LuaTable e = allEvents.Get<int, LuaTable>(j + 1);
                string name = e.Get<string>("eventName");
                events.Add(name, e);
            }
        }
        
        // LuaTable event1 = events.Get<int, LuaTable>(1);
        // LuaTable actions = event1.Get<LuaTable>("actions");
        // LuaTable action1 = actions.Get<int, LuaTable>(1);
        // LuaTable parameters = action1.Get<LuaTable>("actionParams");
        // luaEnv.Global.Get<Action<LuaTable>>("Move")?.Invoke(parameters);
        AddEvent("StartGame");
    }
    
    void Update()
    {
        if (currentActions.Count > 0 && timer <= 0)
        {
            ExcuteAction(currentActions[0]);
            currentActions.RemoveAt(0);
        }
        else
        {
            timer -= Time.deltaTime;
        }
    }
    
    private bool AddEvent(string eventName)
    {
        if (events.ContainsKey(eventName))
        {
            LuaTable currentEvent = events[eventName];
            currentEvents.Add(currentEvent);
            return true;
        }
        else
        {
            Debug.Log($"Event {eventName} not found!");
            return false;
        }
    }
    
    private bool RemoveEvent(string eventName)
    {
        if (events.ContainsKey(eventName))
        {
            LuaTable currentEvent = events[eventName];
            currentEvents.Remove(currentEvent);
            return true;
        }
        else
        {
            Debug.Log($"Event {eventName} not found!");
            return false;
        }
    }

    public void TriggerEvent(string triggerType, string objectName)
    {
        for (int i = 0; i < currentEvents.Count; i++)
        {
            LuaTable currentEvent = currentEvents[i];
            if(currentEvent.Get<string>("triggerType") == triggerType || triggerType == "Any")
            {
                LuaTable triggerParams = currentEvent.Get<LuaTable>("triggerParams");
                if (triggerParams.Get<string>("objectName") == objectName || objectName == "Any")
                {
                    //触发事件
                    Debug.Log($"Event triggered: {triggerType} {objectName}");
                    LuaTable actions = currentEvent.Get<LuaTable>("actions");
                    for (int j = 0; j < actions.Length; j++)
                    {
                        LuaTable action = actions.Get<int, LuaTable>(j + 1);
                        currentActions.Add(action);
                    }
                    currentEvents.Remove(currentEvent);
                    break;
                }
            }
        }
    }

    private bool ExcuteAction(LuaTable action)
    {
        string actionType = action.Get<string>("actionType");
        LuaTable actionParams = action.Get<LuaTable>("actionParams");
        if (actionType == "RegisterEvent")
        {
            string eventName = actionParams.Get<string>("eventName");
            AddEvent(eventName);
        }
        else if (actionType == "DeregisterEvent")
        {
            string eventName = actionParams.Get<string>("eventName");
            RemoveEvent(eventName);
        }
        else if (actionType == "Wait")
        {
            float duration = actionParams.Get<float>("duration");
            timer = duration;
        }
        else
        {
            var func = luaEnv.Global.Get<Action<LuaTable>>(actionType);
            if (func != null)
            {
                // Debug.Log($"Action {actionType} excuted!");
                func(actionParams);
            }
            else
            {
                Debug.Log($"Action {actionType} not found!");
                return false;
            }
        }

        return true;
    }
}


