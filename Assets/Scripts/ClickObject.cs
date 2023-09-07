using System;
using UnityEngine;

[ExecuteAlways]
public class ClickObject : MonoBehaviour
{
    public string objectName;
    private void OnMouseDown()
    {
        HandleClickDown();
    }

    private void OnMouseUpAsButton()
    {
        HandleClickUp();
    }

    void HandleClickDown()
    {
        string _name;
        if (objectName == "")
        {
            _name = gameObject.name;
        }
        else
        {
            _name = objectName;
        }
        CollisionManager.Instance.RegisterCollision(_name);
    }
    
    void HandleClickUp()
    {
        string _name;
        if (objectName == "")
        {
            _name = gameObject.name;
        }
        else
        {
            _name = objectName;
        }
        CollisionManager.Instance.UnregisterCollision(_name);
    }
}
