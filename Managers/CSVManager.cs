using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class CSVManager {

    // saved in the assets folder
    private static string filePath = Application.dataPath + "/studyData.csv";

    public static void SaveToCSV(List<TrialData> trialDataList) {
        // make sure the file exists or create new one
        if (!File.Exists(filePath)) {
            using (StreamWriter sw = File.CreateText(filePath)) {
                sw.WriteLine("CursorType,Amplitude,Width,EffectiveWidthRatio,MovementTime,MissedClicks");
            }
        }

        // add all the data to the csv file
        using (StreamWriter sw = File.AppendText(filePath)) {
            foreach (var trialData in trialDataList) {
                string line = string.Format("{0},{1},{2},{3},{4},{5}",
                    trialData.CursorType,
                    trialData.Amplitude,
                    trialData.Width,
                    trialData.EffectiveWidthRatio,
                    trialData.MovementTime,
                    trialData.MissedClicks);

                sw.WriteLine(line);
            }
        }

        // make sure list is cleared
        trialDataList.Clear();

        // location of file if you need it
        Debug.Log("CSV saved at: " + filePath);
    }
}