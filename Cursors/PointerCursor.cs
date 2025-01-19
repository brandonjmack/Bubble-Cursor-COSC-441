using UnityEngine;
using UnityEngine.UI;

public class PointerCursor : MonoBehaviour {
    
    private RectTransform crosshair;

    // hide the default cursor and use custom one
    void Start() {
        crosshair = GetComponent<RectTransform>();
        Cursor.visible = false;
    }

    // make sure crosshair is on the mouse
    void Update() {
        Vector2 mousePos = Input.mousePosition;
        crosshair.position = mousePos;
    }
}