using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {
    public TargetManager targetManager;
    public BubbleCursor bubbleCursor;
    public PointerCursor pointerCursor;

    private float startTime;
    private int missedClicks;
    private string cursorType;

    private List<TrialData> trialDataList = new List<TrialData>();

    void Start() {
        missedClicks = 0;

        // set cursor type/name
        int cursorTypeInt = PlayerPrefs.GetInt("Cursor Type");
        if(cursorTypeInt == 1) {
            cursorType = "BubbleCursor";
        } else {
            cursorType = "PointerCursor";
        }

        startTime = Time.time;
    }

    public void OnTargetClicked() {
        float movementTime = Time.time - startTime;

        // store data from that screen here
        TrialData trialData = new TrialData() {
            CursorType = cursorType,
            Amplitude = targetManager.targetAmplitudes[Random.Range(0, targetManager.targetAmplitudes.Count)], 
            Width = targetManager.targetSizes[Random.Range(0, targetManager.targetSizes.Count)],
            EffectiveWidthRatio = targetManager.effectiveWidthRatio[Random.Range(0, targetManager.effectiveWidthRatio.Count)], 
            MovementTime = movementTime * 1000f, 
            MissedClicks = missedClicks
        };

        trialDataList.Add(trialData);
        Debug.Log("data added");

        missedClicks = 0;
        startTime = Time.time; 
    }

    public void OnMissedClick() {
        missedClicks++;
    }

    public void SaveData() {
        CSVManager.SaveToCSV(trialDataList);
    }
}

// store all the data
public class TrialData {
    public string CursorType;
    public float Amplitude;
    public float Width;
    public float EffectiveWidthRatio;
    public float MovementTime;
    public int MissedClicks;
}