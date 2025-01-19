using UnityEngine;

public class Destractor : MonoBehaviour {
    
    private SpriteRenderer sprite;
    private Color originalColor;

    // get sprite and get original color
    void Start() {
        sprite = GetComponent<SpriteRenderer>();
        originalColor = sprite.color;
    }

    // hover change it to yellow
    void OnMouseEnter() {
        sprite.color = Color.yellow;
    }

    // not hover set it back to white
    void OnMouseExit() {
        sprite.color = originalColor;
    }
}