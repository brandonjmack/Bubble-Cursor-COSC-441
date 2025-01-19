using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BubbleCursor : MonoBehaviour {

    public float resizeSpeed = 0.2f;
    
    private float minSize;
    private float maxSize;

    private TargetManager targetManager;

    private SpriteRenderer highlightedTarget;
    private Dictionary<SpriteRenderer, Color> originalColors = new Dictionary<SpriteRenderer, Color>();

    private Transform[] targets;

    // set min and max size of bubble, find all targets on screen
    void Start() {
        Cursor.visible = false;
        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");

        targetManager = FindObjectOfType<TargetManager>();

        if (targetManager != null) {
            minSize = targetManager.targetSizes[1] * 2;
            maxSize = targetManager.effectiveWidthRatio[1] * 2;
        } 

        FindAllTargets();
    }

    // find all the targets on the screen
    public void FindAllTargets() {
        GameObject[] targetObjects = GameObject.FindGameObjectsWithTag("Target");
        GameObject[] distractorObjects = GameObject.FindGameObjectsWithTag("Distractor");
        GameObject[] randomCircleObjects = GameObject.FindGameObjectsWithTag("RandomCircle");

        List<Transform> targetList = new List<Transform>();

        // add each of the target types to the list
        foreach (var target in targetObjects) {
            targetList.Add(target.transform);
        }
        foreach (var distractor in distractorObjects) {
            targetList.Add(distractor.transform);
        } 

        foreach (var randomCircle in randomCircleObjects) {
            targetList.Add(randomCircle.transform);
        }

        targets = targetList.ToArray();
    }

    void Update() {
        Vector3 cursorPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        cursorPosition.z = 0f;

        // bubble position is the cursor position
        transform.position = cursorPosition;

        ResizeCursor(cursorPosition);
        DetectSingleCircle();

        // click the bubble cursor
        if (Input.GetMouseButtonDown(0)) {
            BubbleClick(cursorPosition);
        }
        
        // send missed click to game manager
        if(Input.GetMouseButtonDown(0)) {
            if(!BubbleClick(cursorPosition)) {
                FindObjectOfType<GameManager>().OnMissedClick();
            }
        }
    }

    // resize bubble by distance from target
    void ResizeCursor(Vector3 cursorPosition) {
        float closestDist = float.MaxValue;

        // find closest target
        foreach (var target in targets) {
            if (target != null) {
                float distance = Vector2.Distance(cursorPosition, target.position);
                if (distance < closestDist) {
                    closestDist = distance;
                }
            }
        }

        // resize bubble depending on the distance
        if (closestDist != float.MaxValue) {
            float newSize = Mathf.Lerp(minSize, maxSize, closestDist / 5f);
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(newSize, newSize, 1f), resizeSpeed);
        }
    } 

    // highlighting targets (moved target.cs to here)
    void DetectSingleCircle() {

        // find any targets in the bubble
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, transform.localScale.x / 2f);
        SpriteRenderer closestSprite = null;
        float closestDist = float.MaxValue;

        // if bubble over the target highlight it
        if (highlightedTarget != null) {
            highlightedTarget.color = originalColors[highlightedTarget];
            highlightedTarget = null;
        }

        // find closest target 
        foreach (var collider in colliders) {
            if (collider.CompareTag("Target") || collider.CompareTag("Distractor") || collider.CompareTag("RandomCircle")) {
                SpriteRenderer sprite = collider.GetComponent<SpriteRenderer>();
                float dist = Vector2.Distance(transform.position, collider.transform.position);

                // store original color so it can reset
                if (sprite != null) {
                    if (!originalColors.ContainsKey(sprite)) {
                        originalColors[sprite] = sprite.color;
                    }

                    if (dist < closestDist) {
                        closestDist = dist;
                        closestSprite = sprite;
                    }
                }
            }
        }

        // if its the closest sprite highlight
        if (closestSprite != null) {
            closestSprite.color = Color.yellow;
            highlightedTarget = closestSprite;
        }
    }

    // clicking the bubble cursor
    bool BubbleClick(Vector3 cursorPosition) {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, transform.localScale.x / 2f);

        // check for target in the bubble
        foreach (var collider in colliders) {
            if (collider.CompareTag("Target") || collider.CompareTag("Distractor") || collider.CompareTag("RandomCircle")) {
                collider.gameObject.SendMessage("OnMouseDown", SendMessageOptions.DontRequireReceiver);
                return true;
            }
        }
        return false;
    }  
}