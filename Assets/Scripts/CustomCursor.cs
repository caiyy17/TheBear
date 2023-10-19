using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CustomCursor : MonoBehaviour
{
    [FormerlySerializedAs("cursorTexture")] public Texture2D hoverTexture; // Drag your custom cursor texture here in the inspector
    [FormerlySerializedAs("hoverTexture")] public Texture2D selectTexture; // Drag your custom cursor texture here in the inspector
    public Vector2 hotSpot = Vector2.zero; // The "active spot" of the cursor, usually the point or tip of the cursor arrow.
    public float cursorScale = 1.0f; // The scale of the cursor, in case you want to use a smaller or larger cursor graphic than the original.
    public CursorMode cursorMode = CursorMode.Auto; // Use auto to let Unity handle the cursor size based on the platform, or ForceSoftware if you want to force the cursor size.

    private List<string> hoverObjects = new List<string>();

    void Start()
    {
        // Set the custom cursor
        hoverTexture = Resize(hoverTexture, cursorScale);
        selectTexture = Resize(selectTexture, cursorScale);
        Cursor.SetCursor(hoverTexture, hotSpot, cursorMode);
    }
    
    void Update()
    {
        //find where the mouse is hover on
    }
    
    public static Texture2D Resize(Texture2D source, float scale)
    {
        int newWidth = (int)(source.width * scale);
        int newHeight = (int)(source.height * scale);
        source.filterMode = FilterMode.Bilinear;
        RenderTexture rt = RenderTexture.GetTemporary(newWidth, newHeight);
        rt.filterMode = FilterMode.Bilinear;
        RenderTexture.active = rt;
        Graphics.Blit(source, rt);
        Texture2D nTex = new Texture2D(newWidth, newHeight);
        nTex.ReadPixels(new Rect(0, 0, newWidth, newWidth), 0, 0);
        nTex.Apply();
        RenderTexture.active = null;
        return nTex;
    }
    
    public void RegisterHover(string obj)
    {
        hoverObjects.Add(obj);
    }
    
    public void UnregisterHover(string obj)
    {
        hoverObjects.Remove(obj);
    }
    
    public void HandleMouse(string obj, int state)
    {
        //if obj is in hoverObjects, change cursor to hoverTexture
        if (hoverObjects.Contains(obj))
        {
            //hover
            if (state == 0)
            {
                Cursor.SetCursor(hoverTexture, hotSpot, cursorMode);
            }
            //select
            else if (state == 1)
            {
                Cursor.SetCursor(selectTexture, hotSpot, cursorMode);
            }
        }
        else
        {
            //restore cursor to unity default
            Cursor.SetCursor(null, Vector2.zero, cursorMode);
        }
    }
}