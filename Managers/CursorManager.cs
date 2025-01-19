using UnityEngine;

public class CursorManager : MonoBehaviour {
    public GameObject bubbleCursor;
    public GameObject pointerCursor;

    // get the users cursor type
    void Start() {
        int cursorType = PlayerPrefs.GetInt("Cursor Type");
        
        if(cursorType == 1) {
            ActivateBubbleCursor();
        } else {
            ActivatePointerCursor();
        }
    }

    // set the bubble cursor
    void ActivateBubbleCursor() {
        bubbleCursor.SetActive(true);
        pointerCursor.SetActive(true);
    }

    // set the pointer cursor
    void ActivatePointerCursor() {
        bubbleCursor.SetActive(false);
        pointerCursor.SetActive(true);
    }
}