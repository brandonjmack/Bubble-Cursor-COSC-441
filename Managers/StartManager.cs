using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartManager : MonoBehaviour {
    public InputField idInput;
    public InputField numTrialsInput;
    public Button startButton;
    public Toggle cursorToggle;
    
    // listens for cursor choice and start button
    void Start() {
        cursorToggle.onValueChanged.AddListener(OnCursorToggleChanged);
        startButton.onClick.AddListener(OnStartButtonClicked);
    }

    void OnStartButtonClicked() {
        string participantId = idInput.text;
        string trialCountText = numTrialsInput.text;

        // set part id
        PlayerPrefs.SetString("Participant ID", participantId);
        
        // set trial count
        if(int.TryParse(trialCountText, out int trialCount)) {
            PlayerPrefs.SetInt("Trial Count", trialCount);   
        }
        
        PlayerPrefs.Save();

        // load game scene
        SceneManager.LoadScene("GameScene");
    } 

    // change the cursor depending on selection
    void OnCursorToggleChanged(bool isBubbleCursor) {
        PlayerPrefs.SetInt("Cursor Type", isBubbleCursor ? 1 : 0);
        PlayerPrefs.Save();
    }
}