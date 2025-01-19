using UnityEngine;

public class TargetDot : MonoBehaviour {

    private TargetManager targetManager;
    private SpriteRenderer sprite;
    private Color originalColor;

    // get sprite and get original color
    void Start() {
        targetManager = FindObjectOfType<TargetManager>();
        sprite = GetComponent<SpriteRenderer>();
        originalColor = sprite.color;
    }

    // destroy target if clicked
    void OnMouseDown() {
        if (targetManager != null) {
            targetManager.OnTargetClicked();
        }
        Destroy(gameObject);
    }

    // hover change it to green
    void OnMouseEnter() {
        sprite.color = Color.yellow;
    }
  
    // not hover set it back to white
    void OnMouseExit()
    {
        sprite.color = originalColor;
    }
}