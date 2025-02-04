using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class RhythmChartParser : MonoBehaviour
{
    [Serializable]
    public class ChartData
    {
        public string title;
        public string artist;
        public float waveoffset;
        public float bpm;
        public int tpb;
        public int lastmeasure;
        public int[] measure;
        public string[] chart;
    }

    [Serializable]
    public class NoteData
    {
        public float time; // Tick -> s
    }

    public float offset = 0.0f;  // WAV Offset (sec)
    public float bpm = 140f;     // default BPM
    public int tpb = 480;        // Tick Per Beat
    public List<NoteData> notes = new List<NoteData>();

    void Start()
    {
        
    }

    public void LoadChart(string jsonContent)
    {
        ChartData chartData = JsonUtility.FromJson<ChartData>(jsonContent);

        if (chartData == null)
        {
            Debug.LogError("Failed to parse JSON file.");
            return;
        }

        bpm = chartData.bpm;
        tpb = chartData.tpb;
        offset = chartData.waveoffset;

        foreach (string entry in chartData.chart)
        {
            string[] parts = entry.Split('#');
            if (parts.Length != 2) continue;
            if (int.TryParse(parts[0], out int tick))
            {
                float tickToTime = ConvertTickToTime(tick);
                notes.Add(new NoteData { time = tickToTime });
            }
        }

        Debug.Log("Chart loading completed. Total combo: " + notes.Count);
    }

    public List<float> ConvertNoteDataToTimes()
    {
        List<float> noteTimes = new List<float>();
        foreach (var note in notes)
        {
            noteTimes.Add(note.time);
        }
        return noteTimes;
    }

    public float ConvertTickToTime(int tick)
    {
        float seconds = tick * 60 / (bpm * tpb);
        return seconds + offset;
    }
}