using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class RhythmChartParser : MonoBehaviour
{
    public class NoteData
    {
        public int measure;
        public int channel;
        public string data;
    }

    public float offset = 0.0f;  // WAV offset (in sec)
    public Dictionary<int, float> bpmMap = new Dictionary<int, float>(); // BPM mapping
    public List<NoteData> notes = new List<NoteData>(); // note list

    public void LoadChart(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Debug.LogError("파일을 찾을 수 없습니다: " + filePath);
            return;
        }

        string[] lines = File.ReadAllLines(filePath);
        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line) || !line.StartsWith("#"))
                continue;

            // WAV offset
            if (line.StartsWith("#WAVEOFFSET"))
            {
                string[] parts = line.Split(' ');
                if (parts.Length > 1 && float.TryParse(parts[1], out float offsetValue))
                {
                    offset = offsetValue;
                }
            }

            // BPM
            else if (line.StartsWith("#BPM"))
            {
                string bpmID = line.Substring(4, 2);
                string bpmValue = line.Substring(7);

                if (int.TryParse(bpmID, System.Globalization.NumberStyles.HexNumber, null, out int id) &&
                    float.TryParse(bpmValue, out float bpm))
                {
                    bpmMap[id] = bpm;
                }
            }

            // (Note data example - #00216:1312
            else if (line.Contains(":"))
            {
                string[] parts = line.Split(':');
                string header = parts[0].Substring(1);
                string data = parts[1];

                if (header.Length >= 5 &&
                    int.TryParse(header.Substring(0, 3), out int measure) &&
                    int.TryParse(header.Substring(3, 2), out int channel))
                {
                    notes.Add(new NoteData { measure = measure, channel = channel, data = data });
                }
            }
        }

        Debug.Log("Chart loading completed.");
        Debug.Log("Total Combo: " + notes.Count);
    }

    public List<float> ConvertNoteDataToTimes(NoteData noteData)
    {
        // TO-DO
        return null;
    }

}