using System;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;

[ExecuteAlways]
public class ClickObject : MonoBehaviour
{
    public string objectName;
    private void OnMouseDown()
    {
        ;
    }

    private void OnMouseUpAsButton()
    {
        HandleClick();
    }

    void HandleClick()
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
        EventManager.Instance.TriggerEvent("Click", _name);
    }
}
