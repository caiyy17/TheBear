using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine.Serialization;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance { get; private set; }
    public string startEventName = "StartGame";
    public string[] eventTreeList = { "EventsTree1" };
    
    private Dictionary<string, Event> events = new Dictionary<string, Event>();
    private List<Event> currentEvents = new List<Event>();
    private List<Action> currentActions = new List<Action>();
    private float timer = 0;
    
    public CustomCursor customCursor;
    int DEBUG = 0;

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
        DOTween.Init();
        customCursor = GetComponent<CustomCursor>();
        // 加载所有事件
        for (int j = 0; j < eventTreeList.Length; j++)
        {
            TextAsset json = Resources.Load<TextAsset>(eventTreeList[j]);
            if (json != null)
            {
                EventsTree eventsTree = JsonConvert.DeserializeObject<EventsTree>(json.text);
                for (int i = 0; i < eventsTree.events.Count; i++)
                {
                    Event e = eventsTree.events[i];
                    string eventName = e.eventName;
                    events.Add(eventName, e);
                }
            }
        }
        
        Debug.Log($"EventManager: {events.Count} events loaded.");
        AddEvent(startEventName);
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
            Event currentEvent = events[eventName];
            currentEvents.Add(currentEvent);
            List<Action> preactions = currentEvent.preactions;
            for (int j = 0; j < preactions.Count; j++)
            {
                Action action = preactions[j];
                currentActions.Add(action);
            }
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
            Event currentEvent = events[eventName];
            currentEvents.Remove(currentEvent);
            return true;
        }
        else
        {
            Debug.Log($"Event {eventName} not found!");
            return false;
        }
    }

    public bool TriggerEvent(string triggerType, string objectName)
    {
        for (int i = 0; i < currentEvents.Count; i++)
        {
            Event currentEvent = currentEvents[i];
            if(currentEvent.triggerType == triggerType || triggerType == "Any")
            {
                Dictionary<string, object> triggerParams = currentEvent.triggerParams;
                if (triggerParams["objectName"] as string == objectName || objectName == "Any")
                {
                    //触发事件
                    // Debug.Log($"Event triggered: {triggerType} {objectName}");
                    List<Action> actions = currentEvent.actions;
                    for (int j = 0; j < actions.Count; j++)
                    {
                        Action action = actions[j];
                        currentActions.Add(action);
                    }
                    currentEvents.Remove(currentEvent);
                    return true;
                }
            }
        }
        return false;
    }

    private bool ExcuteAction(Action action)
    {
        string actionType = action.actionType;
        Dictionary<string, object> actionParams = action.actionParams;
        // Case switch: actionType
        switch (actionType)
        {
            case "RegisterEvent":
            {
                string eventName = actionParams["eventName"] as string;
                Debug.Log($"RegisterEvent {eventName}");
                AddEvent(eventName);
                break;
            }
            case "DeregisterEvent":
            {
                string eventName = actionParams["eventName"] as string;
                Debug.Log($"DeregisterEvent {eventName}");
                RemoveEvent(eventName);
                break;
            }
            case "AddHover":
            {
                string objectName = actionParams["objectName"] as string;
                customCursor.RegisterHover(objectName);
                break;
            }
            case "RemoveHover":
            {
                string objectName = actionParams["objectName"] as string;
                customCursor.UnregisterHover(objectName);
                break;
            }
            case "Wait":
            {
                float duration = objectToFloat(actionParams["duration"]);
                timer = duration;
                if(DEBUG == 1)
                {
                    timer = 0;
                }
                break;
            }
            case "Move":
            {
                string objectName = actionParams["objectName"] as string;
                object startPosition = actionParams["startpoint"];
                float[] startArray = ((JArray)startPosition).Select(jv => (float)jv).ToArray();
                object endPosition = actionParams["endpoint"];
                float[] endArray = ((JArray)endPosition).Select(jv => (float)jv).ToArray();
                float duration = objectToFloat(actionParams["duration"]);
                GameObject gameObject = GameObject.Find(objectName);
                if (gameObject)
                {
                    gameObject.transform.position = new Vector3(startArray[0], startArray[1], startArray[2]);
                    gameObject.transform.DOMove(new Vector3(endArray[0], endArray[1], endArray[2]), duration);
                }
                break;
            }
            case "HideObject":
            {
                string objectName = actionParams["objectName"] as string;
                GameObject gameObject = GameObject.Find(objectName);
                if (gameObject)
                {
                    gameObject.SetActive(false);
                }
                break;
            }
            case "ShowObject":
            {
                string objectName = actionParams["objectName"] as string;
                object startPosition = actionParams["startpoint"];
                float[] startArray = ((JArray)startPosition).Select(jv => (float)jv).ToArray();
                object endPosition = actionParams["endpoint"];
                float[] endArray = ((JArray)endPosition).Select(jv => (float)jv).ToArray();
                float duration = objectToFloat(actionParams["duration"]);
                GameObject gameObject = GameObject.Find(objectName);
                if (gameObject)
                {
                    //slowly move from startpoint to endpoint
                    //set alpha from 0 to 1
                    if(duration > 0 && gameObject.GetComponent(typeof(SpriteRenderer)))
                    {
                        SpriteRenderer spriteRenderer = gameObject.GetComponent(typeof(SpriteRenderer)) as SpriteRenderer;
                        spriteRenderer.color = new Color(1, 1, 1, 0);
                        spriteRenderer.DOFade(1, duration);
                    }
                    gameObject.transform.position = new Vector3(startArray[0], startArray[1], startArray[2]);
                    gameObject.transform.DOMove(new Vector3(endArray[0], endArray[1], endArray[2]), duration);
                }
                break;
            }
            case "PlayAnimation":
            {
                string objectName = actionParams["objectName"] as string;
                string animationName = actionParams["animationName"] as string;
                GameObject gameObject = GameObject.Find(objectName);
                Animator animator = gameObject.GetComponent<Animator>();
                if (animator)
                {
                    animator.Play(animationName);
                }
                break;
            }
            case "quitGame":
            {
                Application.Quit();
                break;
            }
            default:
            {
                Debug.Log($"Action {actionType} not implemented!");
                return false;
            }
        }
        return true;
    }
    
    public float objectToFloat(object obj)
    {
        if (obj is float)
        {
            return (float)obj;
        }
        else if (obj is double)
        {
            return (float)(double)obj;
        }
        else if (obj is int)
        {
            return (float)(int)obj;
        }
        else if (obj is long)
        {
            return (float)(long)obj;
        }
        else
        {
            Debug.Log(obj.GetType());
            return 0;
        }
    }
}


