using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EventsTree
{
    public List<Event> events;
}

[System.Serializable]
public class Event
{
    public string eventName;
    public string triggerType;
    public Dictionary<string, object> triggerParams;
    public List<Action> preactions;
    public List<Action> actions;
}

[System.Serializable]
public class Action
{
    public string actionType;
    public Dictionary<string, object> actionParams;
}