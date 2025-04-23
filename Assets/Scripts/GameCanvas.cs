using UnityEngine;

public class GameCanvas : MonoBehaviour {
    void Start()
    {
        Canvas canvas = GetComponent<Canvas>();
        if (canvas == null) {
            Debug.LogError("Canvas component not found on GameCanvas");
            return;
        }

        Globals.CanvasWidth = canvas.GetComponent<RectTransform>().rect.width;
        Globals.CanvasHeight = canvas.GetComponent<RectTransform>().rect.height;
        Globals.CanvasScaleFactor = canvas.scaleFactor;
        
        Debug.Log($"Canvas Width: {Globals.CanvasWidth}, Height: {Globals.CanvasHeight}, Scale Factor: {Globals.CanvasScaleFactor}");
    }
}