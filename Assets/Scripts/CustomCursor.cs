using UnityEngine;

public class CustomCursor : MonoBehaviour
{
    public Texture2D cursorTexture; // Drag your custom cursor texture here in the inspector
    public Texture2D hoverTexture; // Drag your custom cursor texture here in the inspector
    public Vector2 hotSpot = Vector2.zero; // The "active spot" of the cursor, usually the point or tip of the cursor arrow.
    public float cursorScale = 1.0f; // The scale of the cursor, in case you want to use a smaller or larger cursor graphic than the original.
    public CursorMode cursorMode = CursorMode.Auto; // Use auto to let Unity handle the cursor size based on the platform, or ForceSoftware if you want to force the cursor size.

    void Start()
    {
        // Set the custom cursor
        cursorTexture = Resize(cursorTexture, cursorScale);
        hoverTexture = Resize(hoverTexture, cursorScale);
        Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
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
}