using System;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;

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

    private void OnMouseOver()
    {
        // HandleMouseOver();
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
    
    // void HandleMouseOver()
    // {
    //     string _name;
    //     if (objectName == "")
    //     {
    //         _name = gameObject.name;
    //     }
    //     else
    //     {
    //         _name = objectName;
    //     }
    //     CollisionManager.Instance.HandleHover(_name);
    // }
}
