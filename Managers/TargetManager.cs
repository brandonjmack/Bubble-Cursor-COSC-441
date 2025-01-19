using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class TargetManager : MonoBehaviour {

    [SerializeField] public List<float> targetSizes;
    [SerializeField] public List<float> targetAmplitudes;
    [SerializeField] public List<float> effectiveWidthRatio;
    [SerializeField] private int numberTargets;
    [SerializeField] private int trialCount;
    
    
    public GameObject targetDotPrefab;
    public GameObject distractorPrefab;
    public GameObject circlePrefab;

    public bool isIntermediateTargetActive = true;
    private Vector2 containerPosition;
    private float containerRadius;
    private int totalTrials;

    public BubbleCursor bubbleCursor;
    private float effectiveWidth;

    // call start target and get the trial count
    void Start() {
        LoadStarterTarget();
        trialCount = PlayerPrefs.GetInt("Trial Count") * 2 - 1;
        Debug.Log("Total trials: " + trialCount);
    }

    // load the starter target (ceter dot)
    void LoadStarterTarget() {
        ClearAllTargets();

        Vector2 screenCenter = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, Camera.main.nearClipPlane));
        GameObject target = Instantiate(targetDotPrefab, screenCenter, Quaternion.identity);
        target.tag = "Target";
        bubbleCursor.FindAllTargets();
    }

    // load the target with distractors screen
    public void LoadTargetWithDistractors() {
        ClearAllTargets();

        // made container for the target and distractoers -> placed randomly on the screen
        containerPosition = GetRandomPositionOnScreen();
        GameObject container = new GameObject("TargetContainer");
        container.transform.position = containerPosition;

        int randIndex = Random.Range(0, targetSizes.Count);
        float targetWidth = targetSizes[randIndex];

        // create the target
        GameObject target = Instantiate(targetDotPrefab, containerPosition, Quaternion.identity, container.transform);
        target.transform.localScale = new Vector3(targetWidth, targetWidth, 1f);
        target.tag = "Target";

        float randomEffectiveWidthRatio = effectiveWidthRatio[Random.Range(0, effectiveWidthRatio.Count)];

        effectiveWidth = randomEffectiveWidthRatio * targetWidth;
        
        // create the distractors and set random ew from the list
        Vector2[] distractorPositions = CalculateDistractorPositions(containerPosition, effectiveWidth);
        foreach(Vector2 position in distractorPositions) {
            GameObject distractor = Instantiate(distractorPrefab, position, Quaternion.identity, container.transform);
            distractor.transform.localScale = new Vector3(targetWidth, targetWidth, 1f);
            distractor.tag = "Distractor";
        }

        // make the container be a random angle
        float randAngle = Random.Range(0f, 360f);
        container.transform.Rotate(0f, 0f, randAngle);

        containerRadius = effectiveWidth * Mathf.Sqrt(2) * 1.5f;

        spawnRandomCircles();

        bubbleCursor.FindAllTargets();
    }

    public void OnTargetClicked() {

        GameManager gameManager = FindObjectOfType<GameManager>();

        // load page with distractors or the target, also counts the number of trials left
        if(trialCount != 0) {
            if(isIntermediateTargetActive) {
                LoadTargetWithDistractors(); 
            } else {
                gameManager.OnTargetClicked();
                LoadStarterTarget();
            }

        isIntermediateTargetActive = !isIntermediateTargetActive;
        trialCount--;
        } else {
            // end game and save the data
            EndGame(gameManager);
        }
    }

    // end the game and save the data
    void EndGame(GameManager gameManager) {
        gameManager.SaveData();
        SceneManager.LoadScene("EndScene");
    }

    // get the distractor positions around the target
    Vector2[] CalculateDistractorPositions(Vector2 centerPosition, float effectiveWidth) {

        float randDistance = targetAmplitudes[Random.Range(0, targetAmplitudes.Count)];

        Vector2 topLeft = new Vector2(centerPosition.x - randDistance, centerPosition.y + randDistance);
        Vector2 topRight = new Vector2(centerPosition.x + randDistance, centerPosition.y + randDistance);
        Vector2 bottomLeft = new Vector2(centerPosition.x - randDistance, centerPosition.y - randDistance);
        Vector2 bottomRight = new Vector2(centerPosition.x + randDistance, centerPosition.y - randDistance);

        return new Vector2[] {topLeft, topRight, bottomLeft, bottomRight};
    }

    // spawns the random circles around the screen
    void spawnRandomCircles() {
        List<Vector2> circlePositions = new List<Vector2>();
        float minDistBetCircles = 1f;

        // spawn the specified number of circles and make sure they are not set to the same position
        for(int i = 0; i < numberTargets; i++) {
            Vector2 randomPosition;
            float randomSize = targetSizes[Random.Range(0, targetSizes.Count)];
            bool isPositionValid = false;
    
            do {
                randomPosition = GetRandomPositionOnScreen();
                isPositionValid = true;
            
                foreach(Vector2 position in circlePositions) {
                    if(Vector2.Distance(randomPosition, position) < minDistBetCircles + randomSize) {
                        isPositionValid = false;
                        break;
                    }
                }
            } while (!isPositionValid || IsInsideMainTargetArea(randomPosition));

            circlePositions.Add(randomPosition);

            // spawn the circle
            GameObject circle = Instantiate(circlePrefab, randomPosition, Quaternion.identity);
            circle.transform.localScale = new Vector3(randomSize, randomSize, 1f);
            circle.tag = "RandomCircle";
        }
        bubbleCursor.FindAllTargets();
    }

    // check if a random circle is inside the target container
    bool IsInsideMainTargetArea(Vector2 position) {
        return Vector2.Distance(position, containerPosition) < containerRadius;
    }

    // get a random position on the screen
    Vector2 GetRandomPositionOnScreen() {
        Vector2 screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.nearClipPlane));

        float randomEffectiveWidthRatio = effectiveWidthRatio[Random.Range(0, effectiveWidthRatio.Count)];
        
        float safeXMin = -screenBounds.x + randomEffectiveWidthRatio * targetSizes[0];
        float safeXMax = screenBounds.x - randomEffectiveWidthRatio * targetSizes[0];
        float safeYMin = -screenBounds.y + randomEffectiveWidthRatio * targetSizes[0];
        float safeYMax = screenBounds.y - randomEffectiveWidthRatio * targetSizes[0];

        return new Vector2(Random.Range(safeXMin, safeXMax), Random.Range(safeYMin, safeYMax));
    }

    // clears all the targets that were on the screen
    void ClearAllTargets() {
        foreach (string tag in new string[] { "Target", "Distractor", "RandomCircle" }) {
            foreach (GameObject target in GameObject.FindGameObjectsWithTag(tag)) {
                Destroy(target);
            }
        }
    }
}